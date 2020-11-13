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
using FFmpegPlayer.DataProviders;


namespace FFmpegPlayer.DataReaders.Generic
{
    public class GenericDataReader : DataReader
    {
        private CancellationTokenSource _readSessionCts;
        private Task<Task> _readLoopTaskTask;

        private static async Task ReadLoop(DataProvider dataProvider, PresentPacketDelegate presentPacket, CancellationToken token)
        {
            Log.Enter();
            var scheduler = new GenericScheduler();

            try
            {
                Log.Info("Started");

                while (!token.IsCancellationRequested)
                {
                    var p = await dataProvider.NextPacket(token);
                    if (p == default)
                    {
                        Log.Info("No data");
                        continue;
                    }

                    // TODO: EOS processing.
                    await presentPacket(p, scheduler.Schedule(p), token);
                }
            }
            catch (OperationCanceledException)
            {
                // silent ignore
            }
            finally
            {
                Log.Info("Disposing scheduler");
                scheduler.Dispose();

                Log.Exit();
            }
        }

        public override IDisposable Create(DataProvider dataProvider, PresentPacketDelegate presentDelegate)
        {
            _readSessionCts = new CancellationTokenSource();

            _readLoopTaskTask = Task.Factory.StartNew(
                () => ReadLoop(dataProvider, presentDelegate, _readSessionCts.Token),
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);

            return this;
        }

        public override void WaitCompletion()
        {
            Log.Enter();

            _readLoopTaskTask?.GetAwaiter().GetResult().GetAwaiter().GetResult();

            Log.Exit();
        }

        public override void Dispose()
        {
            Log.Enter();

            _readSessionCts?.Cancel();
            _readSessionCts?.Dispose();
            _readSessionCts = null;

            Log.Exit();
        }
    }
}
