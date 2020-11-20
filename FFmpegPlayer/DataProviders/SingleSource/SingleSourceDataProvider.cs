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
using FFmpegPlayer.DataSources;

namespace FFmpegPlayer.DataProviders.SingleSource
{
    public sealed class SingleSourceDataProvider : DataProvider
    {
        public override ClipConfiguration CurrentConfiguration { get; protected set; }
        private DataSource _dataSource;

        public override async Task<ClipConfiguration> Open()
        {
            Log.Enter();

            var config = await _dataSource.Open();
            CurrentConfiguration = config;

            Log.Exit();
            return config;
        }

        public override ValueTask<Packet> NextPacket(CancellationToken token)
        {
            return _dataSource.NextPacket(token);
        }

        public override Task<TimeSpan> Seek(TimeSpan position)
        {
            Log.Enter(position.ToString());

            var seekTask = _dataSource.Seek(position);

            Log.Exit();
            return seekTask;
        }

        public override Task Suspend()
        {
            Log.Enter();

            var suspendTask = _dataSource.Suspend();

            Log.Exit();
            return suspendTask;

        }

        public override Task Resume()
        {
            Log.Enter();

            var resumeTask = _dataSource.Resume();

            Log.Exit();
            return resumeTask;
        }

        public override DataProvider Add(params DataSource[] dataSources)
        {
            Log.Enter();

            if (dataSources == null || dataSources.Length == 0)
                throw new ArgumentException("Invalid data sources. Null or empty.", nameof(dataSources));

            _dataSource = dataSources[0];

            Log.Exit();
            return this;
        }

        public override void Dispose()
        {
            Log.Enter();

            Log.Info($"Disposing data source: {_dataSource != null}");
            _dataSource?.Dispose();
            _dataSource = null;

            Log.Exit();
        }
    }
}
