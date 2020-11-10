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
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FFmpegPlayer
{

    public class EventChannel<TEvent> : IDisposable
    {
        private Channel<(TEvent, object)> _eventChannel = Channel.CreateBounded<(TEvent, object)>(
            new BoundedChannelOptions(1)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

        private readonly CancellationToken _sessionToken;
        public EventChannel(CancellationToken token)
        {
            _sessionToken = token;
        }

        public async ValueTask Send(TEvent message)
        {
            try
            {
                await _eventChannel.Writer.WriteAsync((message, null), _sessionToken);
            }
            catch (OperationCanceledException)
            {
                // Silently ignore
            }
        }

        public async ValueTask Send(TEvent message, object messageArg)
        {
            try
            {
                await _eventChannel.Writer.WriteAsync((message, messageArg), _sessionToken);
            }
            catch (OperationCanceledException)
            {
                // Silently ignore
            }
        }

        public ValueTask<(TEvent, object)> Read()
        {
            return _eventChannel.Reader.ReadAsync(_sessionToken);
        }

        public void Dispose()
        {
            _eventChannel.Writer.TryComplete();
            _eventChannel = null;
        }
    }
}
