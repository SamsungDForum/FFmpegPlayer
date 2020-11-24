/*!
 * https://github.com/SamsungDForum/FFmpegPlayer
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
using FFmpegPlayer.Common;
using FFmpegPlayer.DataProviders;

namespace FFmpegPlayer.DataReaders.Generic
{
    public sealed class GenericPacketReader : DataReader
    {
        private static readonly TimeSpan ResubmitDelay = TimeSpan.FromMilliseconds(150);
        private CancellationTokenSource _readSessionCts;
        private Task<Task> _readLoopTaskTask;

        private event ErrorDelegate ErrorHandler;

        private async Task ReadLoop(DataProvider dataProvider, PresentPacketDelegate presentPacket, CancellationToken token)
        {
            Log.Enter();

            var scheduler = new GenericScheduler();
            Packet packet = null;
            try
            {
                Log.Info("Started");

                while (!token.IsCancellationRequested)
                {
                    packet = await dataProvider.NextPacket(token);
                    if (packet == null)
                    {
                        // null packet indicates end of all streams.
                        presentPacket(null);
                        return;
                    }

                    var scheduleAfter = scheduler.Schedule(packet);
                    if (scheduleAfter != default)
                    {
                        Log.Verbose($"{packet.Pts} {packet.StreamType} delaying {scheduleAfter}");
                        await Task.Delay(scheduleAfter, token);
                    }

                    PresentPacketResult result = presentPacket(packet);
                    while (result != PresentPacketResult.Success)
                    {
                        if (result == PresentPacketResult.Fail)
                            throw new Exception($"PresentPacket failed {packet.Pts} {packet.StreamType}");

                        // PresentPacketResult.Retry
                        Log.Warn($"{packet.Pts} {packet.StreamType} Resubmit {ResubmitDelay}");
                        await Task.Delay(ResubmitDelay, token);
                        result = presentPacket(packet);
                    }

                    packet.Dispose();
                    packet = null;
                }
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancelled");
            }
            catch (Exception e)
            {
                var errorMsg = e.ToString();
                Log.Fatal(errorMsg);

                // Not much point in trying to resubmit a packet..
                packet?.Dispose();
                packet = null;

                ErrorHandler?.Invoke(errorMsg);
            }
            finally
            {
                // Abandoned packet
                if (packet != null)
                {
                    // Last chance scenario. Try pushing it. If cancellation was requested, we'll loose one packet less
                    if (presentPacket(packet) != PresentPacketResult.Success)
                        Log.Warn($"{packet.Pts} {packet.StreamType} Abandoned");

                    packet.Dispose();
                }

                Log.Info("Disposing packet scheduler");
                scheduler.Dispose();

                Log.Exit();
            }
        }

        public override DataReader With(DataProvider dataProvider, PresentPacketDelegate presentPacket)
        {
            Log.Enter($"{dataProvider.GetType()}, {presentPacket.GetType()}");

            if (ErrorHandler == null)
                Log.Warn($"{nameof(ErrorHandler)} not set before start");

            _readSessionCts = new CancellationTokenSource();
            _readLoopTaskTask = Task.Factory.StartNew(
                () => ReadLoop(dataProvider, presentPacket, _readSessionCts.Token),
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);
            Log.Info("DataReader starting");

            Log.Exit();
            return this;
        }

        public override DataReader AddHandler(ErrorDelegate errorHandler)
        {
            Log.Enter(nameof(ErrorDelegate));

            ErrorHandler += errorHandler;

            Log.Exit();
            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            Log.Info($"Terminating ReadLoop: {_readLoopTaskTask != null}");
            _readSessionCts?.Cancel();
            _readLoopTaskTask = null;

            _readSessionCts?.Dispose();
            _readSessionCts = null;

            ErrorHandler = null;

            Log.Exit();
        }

        public override Task DisposeAsync()
        {
            Log.Enter();

            var disposalComplete = _readLoopTaskTask?.GetAwaiter().GetResult() ?? Task.CompletedTask;
            Dispose();

            Log.Exit();
            return disposalComplete;
        }
    }
}
