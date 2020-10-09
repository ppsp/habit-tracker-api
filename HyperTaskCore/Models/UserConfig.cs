using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskCore.Models
{
    public class UserConfig
    {
        public ConfigKeyValuePair[] Configs { get; set; } = new ConfigKeyValuePair[0];
    }

    public class ConfigKeyValuePair
    {
        public string key { get; set; }
        public object value { get; set; }
    }
}
