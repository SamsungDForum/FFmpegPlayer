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
using System.Collections.ObjectModel;


namespace FFmpegPlayer.DataSources
{
    public enum OptionValue : long
    {
        Yes = 1,
        No = 0,
        Autodetect = -1
    }

    public class DataSourceOptions
    {
        public ReadOnlyCollection<KeyValuePair<string, object>> Options => _options.AsReadOnly();

        private readonly List<KeyValuePair<string, object>> _options = new List<KeyValuePair<string, object>>();
        public DataSourceOptions Set(string key, string value)
        {
            _options.Add(KeyValuePair.Create<string, object>(key, value));
            return this;
        }

        public DataSourceOptions Set(string key, long value)
        {
            _options.Add(KeyValuePair.Create<string, object>(key, value));
            return this;
        }

        public DataSourceOptions Set(string key, OptionValue value)
        {
            _options.Add(KeyValuePair.Create<string, object>(key, (long)value));
            return this;
        }

        public DataSourceOptions Clear(string key)
        {
            _options.RemoveAll(pair => pair.Key.Equals(key));
            return this;
        }
    }
}
