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
using System.Runtime.InteropServices;
using Logger = Common.Log;
using Interop = FFmpegBindings.Interop;
using FFmpegMacros = FFmpegBindings.Interop.FFmpegMacros;

namespace Demuxer.FFmpeg
{
    public class FFmpegGlue : IFFmpegGlue
    {

        public void Initialize()
        {
            try
            {
                Interop.FFmpeg.av_register_all();
                Interop.FFmpeg.avformat_network_init();
                unsafe
                {
                    Interop.FFmpeg.av_log_set_level(FFmpegMacros.AV_LOG_DEBUG);
                    Interop.av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
                    {
                        if (level > Interop.FFmpeg.av_log_get_level()) return;

                        const int lineSize = 1024;
                        var lineBuffer = stackalloc byte[lineSize];
                        var printPrefix = 1;
                        Interop.FFmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                        var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);

                        Logger.Warn(line);
                    };
                    Interop.FFmpeg.av_log_set_callback(logCallback);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not load and register FFmpeg library");
                throw new DemuxerException("Could not load and register FFmpeg library", e);
            }
        }

        public IAVIOContext AllocIOContext(ulong bufferSize, ReadPacket readPacket)
        {
            return new AVIOContextWrapper(bufferSize, readPacket);
        }

        public IAVFormatContext AllocFormatContext()
        {
            return new AVFormatContextWrapper();
        }

        public void Dispose()
        {
            Logger.Enter();

            Interop.FFmpeg.avformat_network_deinit();

            Logger.Exit();
        }
    }
}
