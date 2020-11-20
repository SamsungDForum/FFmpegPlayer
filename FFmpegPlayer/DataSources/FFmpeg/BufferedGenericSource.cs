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
    public sealed class BufferedGenericSource : DataSource
    {
        private string[] _sourceUrls;
        private FFmpegDemuxer _demuxer;
        private DataBuffer<Packet> _buffer;
        private CancellationTokenSource _sessionCts;
        private Task<Task> _bufferWriteLoopTaskTask;
        private DataSourceOptions _options;
        private event Action<string> ErrorHandler;
        private async Task BufferWriteLoop(CancellationToken token)
        {
            Log.Enter();
            
            Packet packet = null;
            bool packetAdded = false;
            try
            {
                Log.Info("Started");
                do
                {
                    packet = await _demuxer.NextPacket(token);
                    packetAdded = _buffer.Add(packet);
                } while (packet != null && !token.IsCancellationRequested && packetAdded);
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancelled");
            }
            catch (Exception e)
            {
                var errorMsg = e.ToString();
                Log.Fatal(errorMsg);
                ErrorHandler?.Invoke(errorMsg);
            }

            Log.Info($"Terminated. Last packet: {packet==null} Packet added: {packetAdded} Cancelled: {token.IsCancellationRequested}");
            
            Log.Exit();
        }

        private CancellationToken NewSession()
        {
            Log.Enter();

            _sessionCts?.Dispose();
            _sessionCts = new CancellationTokenSource();

            Log.Exit();
            return _sessionCts.Token;
        }

        private void StartBufferWriteLoop(CancellationToken token)
        {
            Log.Enter();

            if (_buffer == null)
                _buffer = new DataBuffer<Packet>();

            _bufferWriteLoopTaskTask = Task.Factory.StartNew(() => BufferWriteLoop(token),
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);

            Log.Exit();

        }
        public override async Task<ClipConfiguration> Open()
        {
            Log.Enter();

            _demuxer = new FFmpegDemuxer(new FFmpegGlue());
            // TODO: Add token cancellation support
            var config = await _demuxer.InitForUrl(_sourceUrls[0], _options?.Options);
            Log.Info($"{_sourceUrls[0]} opened");
            StartBufferWriteLoop(NewSession());

            Log.Exit();
            return config;
        }

        public override ValueTask<Packet> NextPacket(CancellationToken token)
        {
            return _buffer.Take(token);
        }

        public override async Task<TimeSpan> Seek(TimeSpan position)
        {
            Log.Enter(position.ToString());

            _sessionCts.Cancel();
            await await _bufferWriteLoopTaskTask;
            _bufferWriteLoopTaskTask = null;
            Log.Info("BufferWriteLoop Terminated");

            _buffer.Dispose();
            _buffer = null;

            var newSession = NewSession();
            var seekPosition = await _demuxer.Seek(position, newSession);
            Log.Info($"Demuxer seeked to {seekPosition}");
            StartBufferWriteLoop(newSession);

            Log.Exit();
            return seekPosition;
        }

        public override async Task<bool> Suspend()
        {
            Log.Enter();

            _sessionCts.Cancel();

            await await _bufferWriteLoopTaskTask;
            _bufferWriteLoopTaskTask = null;

            var demuxerPaused = await _demuxer.Pause();
            Log.Info("Demuxer suspended");

            Log.Exit();
            return demuxerPaused;
        }

        public override Task<bool> Resume()
        {
            Log.Enter();

            var session = NewSession();
            var demuxerPlayTask = _demuxer.Play();
            StartBufferWriteLoop(session);
            Log.Info("Resuming demuxer");

            Log.Exit();
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

        public override DataSource With(DataSourceOptions options)
        {
            Log.Enter(options.GetType().ToString());

            
	   _options = options;
            
            Log.Exit();
            return this;
        }

        public override DataSource WithHandler(Action<string> errorHandler)
        {
            Log.Enter();

            ErrorHandler += errorHandler;

            Log.Exit();
            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            Log.Info($"Terminating BufferWriteLoop: {_bufferWriteLoopTaskTask != null}");
            _sessionCts?.Cancel();
            _bufferWriteLoopTaskTask?.GetAwaiter().GetResult().GetAwaiter().GetResult();

            Log.Info($"Disposing demuxer: {_demuxer != null}");
            _demuxer?.Dispose();
            _demuxer = null;
            _sourceUrls = null;

            Log.Info($"Disposing buffered packets: {_buffer != null}");
            _buffer?.Dispose();
            _buffer = null;

            _sessionCts?.Dispose();
            _sessionCts = null;

            Log.Exit();
        }
    }
}
