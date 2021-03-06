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
using Demuxer;
using Demuxer.Common;
using FFmpegPlayer.Common;

namespace FFmpegPlayer.DataSources
{
    public abstract class DataSource : IDisposable
    {
        public abstract Task<ClipConfiguration> Open();
        public abstract Task<Packet> NextPacket(CancellationToken token);
        public abstract Task<TimeSpan> Seek(TimeSpan position);

        // TODO: Remove bool result. Add error handling inside DataSource
        public abstract Task<bool> Suspend();
        // TODO: Remove bool result. Add error handling inside DataSource
        public abstract Task<bool> Resume();

        public abstract DataSource Add(params string[] urls);
        public abstract DataSource With(DataSourceOptions options);
        public abstract DataSource AddHandler(ErrorDelegate errorHandler);
        public abstract void Dispose();
    }
}
