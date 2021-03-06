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
using System.Threading.Tasks;
using ElmSharp;
using FFmpegPlayer.Common;
using FFmpegPlayer.DataProviders;
using FFmpegPlayer.DataReaders;

namespace FFmpegPlayer.DataPresenters
{
    public enum SeekDirection
    {
        Forward,
        Backward
    }

    public abstract class DataPresenter : IDisposable
    {
        public abstract Task Open(Window presenterWindow);
        public abstract Task Seek(SeekDirection direction);
        public abstract Task Suspend();
        public abstract Task Resume();
        public abstract DataPresenter With(DataProvider dataProvider);
        public abstract DataPresenter With(FactoryDelegate<DataReader> createDataReader);
        public abstract DataPresenter AddHandlers(EosDelegate eosHandler, ErrorDelegate errorHandler);
        public abstract void Dispose();
    }
}
