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
    public sealed class SingleUrlPullSource : DataSource
    {
        private string[] _sourceUrls;
        private FFmpegDemuxer _demuxer;

        public override Task<ClipConfiguration> Open()
        {
            Log.Enter();

            _demuxer = new FFmpegDemuxer(new FFmpegGlue());
            var openTask = _demuxer.InitForUrl(_sourceUrls[0]);
            Log.Info($"Opening url {_sourceUrls[0]}");

            Log.Exit();
            return openTask;
        }

        public override Task<Packet> NextPacket()
        {
            return _demuxer.NextPacket();
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
            // Suspend not required by implementation.
            return Task.FromResult(true);
        }

        public override Task<bool> Resume()
        {
            // Resume not required by implementation.
            return Task.FromResult(true);
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

            _demuxer.Dispose();
            _demuxer = null;
            _sourceUrls = null;

            Log.Exit();
        }
    }
}
