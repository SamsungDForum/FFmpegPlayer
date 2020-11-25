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
    public struct HttpOption
    {
        // Note. Some of the available options in https://github.com/FFmpeg/FFmpeg/blob/master/libavformat/http.c
        // are not listed.

        /// <summary>
        /// Sets seekability of connection. Yes(1), No(0), Autodetect(-1)
        /// </summary>
        public const string Seekable = "seekable";
        /// <summary>
        /// Use chunked transfer-encoding for posts. Yes(1), No(0)
        /// </summary>
        public const string ChunkedPost = "chunked_post";
        /// <summary>
        /// Set HTTP proxy to tunnel through. ie. http://rabbithole.proxy:1234
        /// </summary>
        public const string HttpProxy = "http_proxy";
        /// <summary>
        /// set custom HTTP headers, can override built in default headers.
        /// </summary>
        public const string Headers = "headers";
        /// <summary>
        /// Set a specific content type for the POST messages.
        /// </summary>
        public const string ContentType = "content_type";
        /// <summary>
        /// Override User-Agent header.
        /// </summary>
        public const string UserAgent = "user_agent";
        /// <summary>
        /// Override referer header.
        /// </summary>
        public const string Referer = "referer";
        /// <summary>
        /// Use persistent connections. Yes(1), No(0)
        /// </summary>
        public const string MultipleRequests = "multiple_requests";
        /// <summary>
        /// set custom HTTP post data.
        /// </summary>
        public const string PostData = "post_data";
        /// <summary>
        /// Export the MIME type.
        /// </summary>
        public const string MimeType = "mime_type";
        /// <summary>
        /// Export the http response version. Usually "1.0" or "1.1". 
        /// </summary>
        public const string HttpVersion = "http_version";
        /// <summary>
        /// Set cookies to be sent in applicable future requests.
        /// </summary>
        public const string Cookies = "cookies";
        /// <summary>
        /// Force sending an Expect: 100-continue header for POST.
        /// </summary>
        public const string SendExpect100 = "send_expect_100";
        /// <summary>
        /// Initial byte offset.
        /// </summary>
        public const string Offset = "offset";
        /// <summary>
        /// Try to limit the request to bytes preceding this offset
        /// </summary>
        public const string EndOffset = "end_offset";
        /// <summary>
        /// Override the HTTP method or set the expected HTTP method from a client.
        /// </summary>
        public const string Method = "method";
        /// <summary>
        /// Auto reconnect after disconnect before EOF.
        /// </summary>
        public const string Reconnect = "reconnect";
        /// <summary>
        /// Auto reconnect at EOF.
        /// </summary>
        public const string ReconnectAtEof = "reconnect_at_eof";
        /// <summary>
        /// Auto reconnect streamed / non seekable streams.
        /// </summary>
        public const string ReconnectStreamed = "reconnect_streamed";
        /// <summary>
        /// Max reconnect delay in seconds after which to give up.
        /// </summary>
        public const string ReconnectMaxDelay = "reconnect_delay_max";
    }
}
