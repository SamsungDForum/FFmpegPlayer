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

namespace Demuxer
{
    public static class DemuxerConfiguration
    {
        public static ulong BufferSize { get; set; } = 64 * 1024; // 32kB seems to be "low level standard", but content downloading pipeline works better for 64kB

        public static int ProbeSize { get; set; } = 32 * 1024; // higher values may cause problems when probing certain kinds of content (assert "len >= s->orig_buffer_size" in aviobuf)

        public static TimeSpan MaxAnalyzeDuration { get; set; } = TimeSpan.FromSeconds(10);
    }
}
