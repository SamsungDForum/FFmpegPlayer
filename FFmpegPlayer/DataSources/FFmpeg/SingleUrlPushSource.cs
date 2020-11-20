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
using Demuxer;
using Demuxer.Common;
using Demuxer.FFmpeg;
using FFmpegPlayer.Toolbox;

namespace FFmpegPlayer.DataSources.FFmpeg
{
    public sealed class SingleUrlPushSource : DataSource
    {
        private string[] _sourceUrls;
        private FFmpegDemuxer _demuxer;
        private DataBuffer<Packet> _buffer;
        private CancellationTokenSource _sessionCts;
        private Task<Task> _bufferWriteLoopTaskTask;

        private async Task BufferWriteLoop()
        {
            Log.Enter();

            var token = _sessionCts.Token;
            bool packetAdded = true;
            
            Log.Info("Started");
            
            while (!token.IsCancellationRequested && packetAdded)
                packetAdded = _buffer.Add(await _demuxer.NextPacket());
            
            Log.Info($"Terminated. Cancelled: {token.IsCancellationRequested}. Packet add failed: {packetAdded}");
            
            Log.Exit();
        }

        private void NewSession()
        {
            Log.Enter();

            _sessionCts?.Dispose();
            _sessionCts = new CancellationTokenSource();
            _buffer = new DataBuffer<Packet>();
            _bufferWriteLoopTaskTask = Task.Factory.StartNew(
                BufferWriteLoop,
                TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning);

            Log.Info("Session starting");
            
            Log.Exit();
        }

        public override async Task<ClipConfiguration> Open()
        {
            Log.Enter();

            _demuxer = new FFmpegDemuxer(new FFmpegGlue());
            var openTask =  _demuxer.InitForUrl(_sourceUrls[0]);
            Log.Info($"Opening url {_sourceUrls[0]}");

            NewSession();
            var result = await openTask;

            Log.Exit();
            return result;
        }

        public override async Task<Packet> NextPacket()
        {
            try
            {
                return await _buffer.Take(_sessionCts.Token);
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancelled");
            }

            return null;
        }

        public override async Task<TimeSpan> Seek(TimeSpan position)
        {
            Log.Enter(position.ToString());

            _sessionCts.Cancel();
            Log.Info("Terminating BufferWriteLoop");

            await await _bufferWriteLoopTaskTask;
            var seekPosition = await _demuxer.Seek(position, CancellationToken.None);
            Log.Info($"Demuxer seeked to {seekPosition}");

            Log.Exit();
            return seekPosition;
        }

        public override async Task<bool> Suspend()
        {
            Log.Enter();

            _sessionCts.Cancel();
            _sessionCts.Dispose();
            _sessionCts = null;

            await await _bufferWriteLoopTaskTask;
            _bufferWriteLoopTaskTask = null;
            var demuxerPause = await _demuxer.Pause();
            Log.Info($"Demuxer suspending");

            Log.Exit();
            return demuxerPause;
        }

        public override Task<bool> Resume()
        {
            Log.Enter();
            
            var demuxerPlayTask = _demuxer.Play();
            Log.Info("Demuxer resuming");
            
            NewSession();

            Log.Enter();
            return demuxerPlayTask;
        }

        public override DataSource Add(params string[] urls)
        {
            Log.Enter();

            _sourceUrls = urls;
            Log.Info($"Added urls {string.Join(", ", urls)}");

            Log.Exit();
            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            Log.Info($"Terminating BufferWriteLoop: {_bufferWriteLoopTaskTask != null}");
            _sessionCts?.Cancel();
            _sessionCts?.Dispose();
            _sessionCts = null;
            _bufferWriteLoopTaskTask?.GetAwaiter().GetResult().GetAwaiter().GetResult();

            Log.Info($"Disposing demuxer: {_demuxer != null}");
            _demuxer?.Dispose();
            _demuxer = null;
            _sourceUrls = null;

            Log.Exit();
        }
    }
}
