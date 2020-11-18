using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FFmpegPlayer.DataSources
{
    public class DataSourceOption
    {
        public ReadOnlyCollection<KeyValuePair<string, object>> Options => _options.AsReadOnly();

        private readonly List<KeyValuePair<string, object>> _options = new List<KeyValuePair<string, object>>();
        public DataSourceOption Set(string key, string value)
        {
            _options.Add(KeyValuePair.Create<string, object>(key,value));
            return this;
        }

        public DataSourceOption Set(string key, long value)
        {
            _options.Add(KeyValuePair.Create<string, object>(key, value));
            return this;
        }

        public DataSourceOption Clear(string key)
        {
            _options.RemoveAll(pair => pair.Key.Equals(key));
            return this;
        }
    }
}
