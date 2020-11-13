/*!
 * https://github.com/SamsungDForum/JuvoPlayer
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
using System.Diagnostics;
using Common;
using Demuxer.Common;

namespace FFmpegPlayer.DataReaders.Generic
{
    internal class GenericScheduler : IDisposable
    {
        private static readonly TimeSpan BufferedDuration = TimeSpan.FromMilliseconds(1010);
        private static readonly TimeSpan DelayThreshold = TimeSpan.FromMilliseconds(1110);

        private Stopwatch _stopwatch;
        private TimeSpan _schedulingOffset;

        internal TimeSpan Schedule(Packet packet)
        {
            if (_stopwatch == null)
            {
                _schedulingOffset = packet.Pts;
                _stopwatch = Stopwatch.StartNew();
                Log.Info($"Scheduling offset {_schedulingOffset}");
            }
            else
            {
                TimeSpan clockDrift = packet.Pts - _schedulingOffset - _stopwatch.Elapsed;

                if (clockDrift >= DelayThreshold)
                    return clockDrift - BufferedDuration;
            }

            return default;
        }

        public void Dispose()
        {
            Log.Enter();

            _stopwatch?.Stop();
            _stopwatch = null;

            Log.Exit();
        }
    }
}
