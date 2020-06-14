using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Shared
{
    public static class DictionaryExtensions
    {
        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.ContainsKey(key))
                if (dict[key] is T)
                    return (T)dict[key];
            return defaultVal;
        }
    }
}
