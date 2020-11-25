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
    // https://github.com/FFmpeg/FFmpeg/blob/master/libavformat/udp.c
    // https://ffmpeg.org/ffmpeg-protocols.html#udp
    public struct UdpOption
    {
        /// <summary>
        /// Set the UDP maximum socket buffer size in bytes. This is used to set either the receive or send buffer size,
        /// depending on what the socket is used for. Default is 32 KB for output, 384 KB for input.
        /// </summary>
        public const string BufferSize = "buffer_size";
        /// <summary>
        /// If set to nonzero, the output will have the specified constant bitrate if the input has enough packets to sustain it. 
        /// </summary>
        public const string Bitrate = "bitrate";
        /// <summary>
        /// Max length of bursts in bits (when using bitrate).
        /// </summary>
        public const string BurstBits = "burst_bits";
        /// <summary>
        /// Local port.
        /// </summary>
        public const string LocalPort = "local_port";
        /// <summary>
        /// Local address.
        /// </summary>
        public const string LocalAddress = "localaddr";
        /// <summary>
        /// UDPLite head size which should be validated by checksum.
        /// </summary>
        public const string UdpLiteCoverage = "udplite_coverage";
        /// <summary>
        /// Maximum UDP packet size.
        /// </summary>
        public const string PacketSize = "pkt_size";
        /// <summary>
        /// Eexplicitly allow socket reuse. Yes(1) Allowed, No(0) Not allowed.
        /// </summary>
        public const string ReuseSocket = "reuse_socket";
        /// <summary>
        /// Explicitly allow or disallow broadcasts. Yes(1) Allowed, No(0) Not allowed.
        /// </summary>
        public const string AllowBroadcast = "broadcast";
        /// <summary>
        /// Time to live value (for multicast only).
        /// </summary>
        public const string TtlMulticast = "ttl";
        /// <summary>
        /// connect() should be called on socket. Yes(1), No(0)
        /// </summary>
        public const string Connect = "connect";
        /// <summary>
        /// Set the UDP receiving circular buffer size, expressed as a number of packets with size of 188 bytes.
        /// </summary>
        public const string FifoSize = "fifo_size";
        /// <summary>
        /// Survive in case of UDP receiving circular buffer overrun.  Yes(1), No(0)
        /// </summary>
        public const string OverrunNonFatal = "overrun_nonfatal";
        /// <summary>
        /// Read timeout, expressed in microseconds.
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
    }
}
