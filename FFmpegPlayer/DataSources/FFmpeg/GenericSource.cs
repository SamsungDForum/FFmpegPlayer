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
using Demuxer;
using Demuxer.Common;
using Demuxer.FFmpeg;
using FFmpegPlayer.Common;

namespace FFmpegPlayer.DataSources.FFmpeg
{
    public sealed class GenericSource : DataSource
    {
        private string[] _sourceUrls;
        private FFmpegDemuxer _demuxer;
        private DataSourceOptions _options;
        private CancellationTokenSource _sessionCts;

        public override Task<ClipConfiguration> Open()
        {
            Log.Enter();

            _sessionCts = new CancellationTokenSource();
            _demuxer = new FFmpegDemuxer(new FFmpegGlue());

            // TODO: Add token cancellation support
            var openTask = _demuxer.InitForUrl(_sourceUrls[0], _options?.Options);

            Log.Info($"Opening {_sourceUrls[0]}");

            Log.Exit();
            return openTask;
        }

        public override Task<Packet> NextPacket(CancellationToken token)
        {
            return _demuxer.NextPacket(token);
        }

        public override Task<TimeSpan> Seek(TimeSpan position)
        {
            Log.Enter(position.ToString());

            var seekTask = _demuxer.Seek(position, _sessionCts.Token);

            Log.Exit();
            return seekTask;
        }

        public override Task<bool> Suspend()
        {
            Log.Enter();

            var suspendTask = _demuxer.Pause();

            Log.Exit();
            return suspendTask;
        }

        public override Task<bool> Resume()
        {
            Log.Enter();

            var resumeTask = _demuxer.Play();

            Log.Exit();
            return resumeTask;
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
            Log.Enter(nameof(DataSourceOptions));

            _options = options;

            Log.Exit();
            return this;
        }

        public override DataSource AddHandler(ErrorDelegate errorHandler)
        {
            // Not used by implementation.
            // Non buffered generic source is a mere pass-through element to underlying source of data (demuxer)
            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            _sessionCts?.Cancel();

            Log.Info($"Disposing demuxer: {_demuxer != null}");
            _demuxer?.Dispose();
            _demuxer = null;
            _sourceUrls = null;

            _sessionCts?.Dispose();
            _sessionCts = null;

            Log.Exit();
        }
    }
}
