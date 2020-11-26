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
using System.Threading.Channels;
using System.Threading.Tasks;
using Common;

namespace FFmpegPlayer.Toolbox
{
    internal class DataBuffer<T> : IDisposable
    {
        private Channel<T> _dataBufferChannel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleReader = false,
            SingleWriter = true
        });

        public bool Add(T data)
        {
            return _dataBufferChannel.Writer.TryWrite(data);
        }

        public ValueTask<T> Take(CancellationToken token)
        {
            return _dataBufferChannel.Reader.ReadAsync(token);
        }

        public void Dispose()
        {
            Log.Enter();

            _dataBufferChannel.Writer.Complete();

            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                // Stored type is IDisposable.
                int disposeCount = 0;
                while (_dataBufferChannel.Reader.TryRead(out var p))
                {
                    // Who said there can be no null?
                    if (!(p is IDisposable toDispose))
                        continue;

                    toDispose.Dispose();
                    disposeCount++;
                }

                Log.Info($"Disposed {disposeCount} {typeof(T)}");
            }

            _dataBufferChannel = null;

            Log.Exit();
        }
    }
}
