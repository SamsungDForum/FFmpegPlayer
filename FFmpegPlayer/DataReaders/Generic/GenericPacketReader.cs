/*!
 * https://github.com/SamsungDForum/JuvoPlayer
 * Copyright 2020, Samsung Electronics Co., Ltd
 * Licensed under the MIT license
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Demuxer.Common;
using FFmpegPlayer.DataPresenters;
using FFmpegPlayer.DataProviders;

namespace FFmpegPlayer.DataReaders.Generic
{
    public class GenericDataReader : DataReader
    {
        private static readonly TimeSpan ResubmitDelay = TimeSpan.FromMilliseconds(150);
        private CancellationTokenSource _readSessionCts;
        private Task<Task> _readLoopTaskTask;

        public override Task SessionDisposal
        {
            get { return _readLoopTaskTask?.GetAwaiter().GetResult() ?? Task.CompletedTask; }
        }

        private static async Task ReadLoop(DataProvider dataProvider, PresentPacketDelegate presentPacket, CancellationToken token)
        {
            Log.Enter();

            var scheduler = new GenericScheduler();
            Packet packet = default;
            try
            {
                Log.Info("Started");

                while (!token.IsCancellationRequested)
                {
                    // TODO: Add EOS support
                    Log.Verbose("Reading packet");
                    packet = await dataProvider.NextPacket(token);
                    if (packet == default)
                    {
                        Log.Info("No data");
                        continue;
                    }

                    var scheduleAfter = scheduler.Schedule(packet);
                    if (scheduleAfter != default)
                    {
                        Log.Verbose($"{packet.StreamType}: {packet.Pts} delaying by {scheduleAfter}");
                        await Task.Delay(scheduleAfter, token);
                    }

                    PresentPacketResult result = presentPacket(packet);
                    while (result != PresentPacketResult.Success)
                    {
                        // packet will be disposed in finally block
                        if (result == PresentPacketResult.Fail)
                            return;

                        await Task.Delay(ResubmitDelay, token);
                        result = presentPacket(packet);
                    }

                    packet.Dispose();
                    packet = default;
                }
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancelled");
            }
            finally
            {
                // Dispose abandoned packets.
                if (packet != default)
                {
                    // Last chance scenario. Try pushing it. If cancellation was requested, we'll loose one packet less
                    if (presentPacket(packet) != PresentPacketResult.Success)
                        Log.Warn($"{packet.StreamType}: Abandoned packet {packet.Pts}");

                    packet.Dispose();
                }

                Log.Info("Disposing packet scheduler");
                scheduler.Dispose();

                Log.Exit();
            }
        }

        public override IDisposable NewSession(DataProvider dataProvider, PresentPacketDelegate presentPacket)
        {
            _readSessionCts = new CancellationTokenSource();

            _readLoopTaskTask = Task.Factory.StartNew(
                () => ReadLoop(dataProvider, presentPacket, _readSessionCts.Token),
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);

            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            _readSessionCts?.Cancel();
            _readSessionCts?.Dispose();
            _readSessionCts = null;
            _readLoopTaskTask = null;

            Log.Exit();
        }
    }
}
