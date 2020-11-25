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
    // https://github.com/FFmpeg/FFmpeg/blob/master/libavformat/rtpproto.c
    // https://ffmpeg.org/ffmpeg-protocols.html#rtp
    public struct RtpOption
    {
        /// <summary>
        /// Time to live (in milliseconds, multicast only).
        /// </summary>
        public const string TtlMulticast = "ttl";
        /// <summary>
        /// Send/Receive buffer size (in bytes).
        /// </summary>
        public const string BufferSize = "buffer_size";
        /// <summary>
        /// Custom remote rtcp port.
        /// </summary>
        public const string RemoteRtcpPort = "rtcp_port";
        /// <summary>
        /// Local rtp port.
        /// </summary>
        public const string LocalRtpPort = "local_rtpport";
        /// <summary>
        /// Local rtcp port.
        /// </summary>
        public const string LocalRtcpPort = "local_rtcpport";
        /// <summary>
        /// Do a connect() on the UDP socket. Yes(1), No(0).
        /// </summary>
        public const string Connect = "connect";
        /// <summary>
        /// Send packets to the source address of the latest received packet Yes(1), No(0). 
        /// </summary>
        public const string WriteToSource = "write_to_source";
        /// <summary>
        /// Set max packet size (in bytes).
        /// </summary>
        public const string PacketSize = "pkt_size";
        /// <summary>
        /// DSCP class.
        /// </summary>
        public const string Dscp = "dscp";
        /// <summary>
        /// Set timeout (in microseconds) of socket I/O operations.
        /// </summary>
        public const string Timeout = "timeout";
        /// <summary>
        /// List allowed source IP addresses. 
        /// </summary>
        public const string AllowedSources = "sources";
        /// <summary>
        /// List disallowed (blocked) source IP addresses. 
        /// </summary>
        public const string BlockedSources = "block";
        /// <summary>
        /// FEC
        /// </summary>
        public const string Fec = "fec";
    }
}
