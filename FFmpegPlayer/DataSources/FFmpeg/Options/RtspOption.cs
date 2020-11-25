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
    // Usage references:
    // https://github.com/FFmpeg/FFmpeg/blob/master/libavformat/rtsp.c
    // https://ffmpeg.org/ffmpeg-protocols.html#rtsp
    public struct RtspOption
    {
        /// <summary>
        /// Do not start playing the stream immediately. Yes(1), No(0)
        /// </summary>
        public const string InitialPause = "initial_pause";
        /// <summary>
        /// Set RTSP transport protocols listed in
        /// <list type="RtspOption.Transport"></list>/>
        /// Multiple lower transport protocols may be specified,
        /// in that case they are tried one at a time (if the setup of one fails, the next one is tried). 
        /// </summary>
        public const string RtspTransport = "rtsp_transport";
        /// <summary>
        /// RTSP flags listed in
        /// <list type="RtspOption.FlagsRtsp"></list>/>
        /// </summary>
        public const string RtspFlags = "rtsp_flags";
        /// <summary>
        /// RTSP flags listed in
        /// <list type="RtspOption.FlagsSdp"></list>/>
        /// </summary>
        public const string SdpFlags = "sdp_flags";
        /// <summary>
        /// Allowed media types listed in
        /// <list type="RtspOption.MediaType"></list>/>
        /// </summary>
        public const string AllowedMediaTypes = "allowed_media_types";
        /// <summary>
        /// Minimum local UDP port.
        /// </summary>
        public const string MinimumPort = "min_port";
        /// <summary>
        /// Maximum local UDP port.
        /// </summary>
        public const string MaximumPort = "max_port";
        /// <summary>
        /// Timeout (in microseconds) of socket TCP I/O operations.
        /// </summary>
        public const string Timeout = "timeout";
        /// <summary>
        /// Set number of packets to buffer for handling of reordered packets.
        /// </summary>
        public const string ReorderQueueSize = "reorder_queue_size";
        /// <summary>
        /// Underlying protocol send/receive buffer size.
        /// </summary>
        public const string BufferSize = "buffer_size";
        /// <summary>
        /// Underlying protocol send packet size.
        /// </summary>
        public const string PacketSize = "pkt_size";
        /// <summary>
        /// Override User-Agent header. If not specified, it defaults to the libavformat identifier string.
        /// </summary>
        public const string UserAgent = "user_agent";

        public struct Transport
        {
            /// <summary>
            /// Tcp.
            /// </summary>
            public const string Udp = "udp";
            /// <summary>
            /// Udp.
            /// </summary>
            public const string Tcp = "tcp";
            /// <summary>
            /// Udp multicast.
            /// </summary>
            public const string UdpMulticast = "udp_multicast";
            /// <summary>
            /// Http.
            /// </summary>
            public const string Http = "http";
            /// <summary>
            /// Https.
            /// </summary>
            public const string Https = "https";
        }

        public struct FlagsRtsp
        {
            /// <summary>
            /// Receive packets from the negotiated peer IP.
            /// </summary>
            public const string FilterSource = "filter_src";
            /// <summary>
            /// Wait for incoming connections.
            /// </summary>
            public const string Listen = "listen";
            /// <summary>
            /// Try RTP via TCP first, if available
            /// </summary>
            public const string PreferTcp = "prefer_tcp";
        }

        public struct FlagsSdp
        {
            /// <summary>
            /// Use custom I/O.
            /// </summary>
            public const string FilterSource = "custom_io";
            /// <summary>
            /// send RTCP packets to the source address of received packets. Yes(1), No(0)
            /// </summary>
            public const string RtcpToSource = "rtcp_to_source";
            /// <summary>
            /// Listening timeout.
            /// </summary>
            public const string ListemTimeout = "listen_timeout";
        }

        public struct MediaType
        {
            /// <summary>
            /// Video.
            /// </summary>
            public const string Video = "video";
            /// <summary>
            /// Audio.
            /// </summary>
            public const string Audio = "audio";
            /// <summary>
            /// Data.
            /// </summary>
            public const string Data = "data";
            /// <summary>
            /// Subtitles.
            /// </summary>
            public const string Subtitle = "subtitle";
        }
    }
}
