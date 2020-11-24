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
using Common;
using Interop = FFmpegBindings.Interop;

namespace Demuxer.FFmpeg
{
    public unsafe class AvDictionary
    {
        public ref Interop.AVDictionary* Dictionary
        {
            get { return ref _avDictionary; }
        }

        private Interop.AVDictionary* _avDictionary;

        public AvDictionary(IReadOnlyCollection<KeyValuePair<string, object>> options = null)
        {
            if (options == null)
                return;

            foreach (var (key, value) in options)
            {
                switch (value)
                {
                    case string stringValue:
                        Set(key, stringValue);
                        break;

                    case long longValue:
                        Set(key, longValue);
                        break;

                    case null:
                        Set(key, null);
                        break;

                    default:
                        throw new ArgumentException($"{value} is of unsupported type {value.GetType()}");
                }
            }
        }

        public AvDictionary Set(string key, string value, int flags = 0)
        {
            Log.Enter($"{key}={value}");

            fixed (Interop.AVDictionary** dictionaryPtr = &_avDictionary)
                if (Interop.FFmpeg.av_dict_set(dictionaryPtr, key, value, flags) != 0)
                    throw new ArgumentException($"Failed to set {key} to {value}");

            Log.Exit(Interop.FFmpeg.av_dict_count(_avDictionary).ToString());
            return this;
        }

        public AvDictionary Set(string key, long value, int flags = 0)
        {
            Log.Enter($"{key}={value}");

            fixed (Interop.AVDictionary** dictionaryPtr = &_avDictionary)
                if (Interop.FFmpeg.av_dict_set_int(dictionaryPtr, key, value, flags) != 0)
                    throw new ArgumentException($"Failed to set {key} to {value}");

            Log.Exit(Interop.FFmpeg.av_dict_count(_avDictionary).ToString());
            return this;
        }
    }
}
