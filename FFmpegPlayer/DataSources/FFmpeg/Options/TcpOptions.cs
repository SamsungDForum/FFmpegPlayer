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
    // https://github.com/FFmpeg/FFmpeg/blob/master/libavformat/http.c
    // https://ffmpeg.org/ffmpeg-protocols.html#http
    public struct TcpOptions
    {
        /// <summary>
        /// Listen for incoming connections. Yes(1), No(0).
        /// </summary>
        public const string Listen = "listen";
        /// <summary>
        /// Timeout (in microseconds) of socket I/O operations.
        /// </summary>
        public const string Timeout = "timeout";
        /// <summary>
        /// Connection awaiting timeout (in milliseconds).
        /// </summary>
        public const string ListenTimeout = "listen_timeout";
        /// <summary>
        /// Socket send buffer size (in bytes).
        /// </summary>
        public const string SendBufferSize = "send_buffer_size";
        /// <summary>
        /// Socket receive buffer size (in bytes).
        /// </summary>
        public const string ReceiveBufferSize = "recv_buffer_size";
        /// <summary>
        /// Use TCP_NODELAY to disable nagle's algorithm. Yes(1), No(0).
        /// </summary>
        public const string TcpNoDelay = "tcp_nodelay";
        /// <summary>
        /// Maximum segment size for outgoing TCP packets.
        /// </summary>
        /// <remarks>Not available under Winsock.</remarks>
        public const string MaximumSegmentSize = "tcp_mss";
    }
}
