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

namespace FFmpegPlayer.DataSources.FFmpeg.Options
{
    public class RtspOption
    {
        public const string InitialPause = "initial_pause";
        public const string RtspTransport = "rtsp_transport";
        public const string RtspFlags = "rtsp_flags";
        public const string AllowedMediaTypes = "allowed_media_types";
        public const string MinimumPort = "min_port";
        public const string MaximumPort = "max_port";
        public const string IncomingConnectionTimeout = "timeout";
        public const string ReorderQueueSize = "reorder_queue_size";
        public const string SocketTimeout = "stimeout";
        public const string UserAgent = "user-agent";
        public const string BufferSize = "buffer_size";

        public static class Pause
        {
            public const long On = 1;
            public const long Off = 0;
        }

        public static class Transport
        {
            public const string Udp = "udp";
            public const string Tcp = "tcp";
            public const string UdpMulticast = "udp_multicast";
            public const string Http = "http";
        }

        public static class Flags
        {
            public const string FilterSource = "filter_src";
            public const string Listen = "listen";
            public const string PreferTcp = "prefer_tcp";
        }

        public static class MediaType
        {
            public const string Video = "video";
            public const string Audio = "audio";
            public const string Data = "data";
            public const string Subtitle = "subtitle";
        }
    }
}
