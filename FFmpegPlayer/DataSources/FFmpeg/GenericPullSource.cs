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

namespace FFmpegPlayer.DataSources.FFmpeg
{
    public sealed class GenericPullSource : DataSource
    {
        private string[] _sourceUrls;
        private FFmpegDemuxer _demuxer;
        private DataSourceOption _options;

        public override Task<ClipConfiguration> Open()
        {
            Log.Enter();

            _demuxer = new FFmpegDemuxer(new FFmpegGlue());

            var openTask = _options != null
                ? _demuxer.InitForUrl(_sourceUrls[0], _options.Options)
                : _demuxer.InitForUrl(_sourceUrls[0]);

            Log.Info($"Opening {_sourceUrls[0]}");
            
            Log.Exit();
            return openTask;
        }

        public override ValueTask<Packet> NextPacket()
        {
            return new ValueTask<Packet>(_demuxer.NextPacket());
        }

        public override Task<TimeSpan> Seek(TimeSpan position)
        {
            Log.Enter(position.ToString());

            var seekTask = _demuxer.Seek(position, CancellationToken.None);

            Log.Exit();
            return seekTask;
        }

        public override Task<bool> Suspend()
        {
            Log.Enter();

            var pauseTask = _demuxer.Pause();

            Log.Exit();
            return pauseTask;
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

        public override DataSource With(DataSourceOption options)
        {
            Log.Enter(typeof(DataSourceOption).ToString());
            
            _options = options;
            
            Log.Exit();
            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            _demuxer.Dispose();
            _demuxer = null;
            _sourceUrls = null;

            Log.Exit();
        }
    }
}
