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
using System.Threading;

namespace Demuxer.FFmpeg
{
    public class OperationMonitor
    {
        private CancellationToken _token = CancellationToken.None;
        private bool _isRunning = false;
        private int _startTicks = int.MaxValue;
        private const int TickTimeOut = 1000 * 5; // timeout in ms. 5s.

        public void Start()
        {
            _token = CancellationToken.None;
            _startTicks = Environment.TickCount;
            _isRunning = true;
        }

        public void Start(CancellationToken token, bool noTimeout = false)
        {
            _token = token;
            _startTicks = noTimeout ? int.MaxValue : Environment.TickCount;
            _isRunning = true;
        }

        public void End()
        {
            _isRunning = false;
            var currentToken = _token;
            bool throwTimeout = Environment.TickCount - _startTicks > TickTimeOut;
            _token = CancellationToken.None;
            _startTicks = int.MaxValue;

            currentToken.ThrowIfCancellationRequested();
            if (throwTimeout)
                throw new TimeoutException("Demuxer operation timeout");
        }

        public int CancelOrTimeout()
        {
            // Does not matter what value is returned. > 0 result in EXIT code being retured.
            return _isRunning && (_token.IsCancellationRequested || Environment.TickCount - _startTicks > TickTimeOut) ? 1 : 0;
        }

        public static bool IsAborted(int result)
        {
            // -541478725  'EOF ' code = ~' FOE'
            // -1414092869 'EXIT' code = ~'TIXE'
            return result == -1414092869 || result == -541478725;
        }
    }
}
