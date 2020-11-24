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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demuxer.Common;

namespace Demuxer
{
    public struct ClipConfiguration
    {
        public IList<StreamConfig> StreamConfigs { get; set; }
        public IList<DrmInitData> DrmInitDatas { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public interface IDemuxer : IDisposable
    {
        bool IsInitialized();
        Task<ClipConfiguration> InitForUrl(string url, IReadOnlyCollection<KeyValuePair<string, object>> options = null);
        Task<ClipConfiguration> InitForEs();
        Task<Packet> NextPacket(CancellationToken token);
        Task<Packet> NextPacket();
        void PushChunk(byte[] chunk);
        Task Completion { get; }
        void Complete();
        void Reset();
        Task<TimeSpan> Seek(TimeSpan time, CancellationToken token);
    }
}