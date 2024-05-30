using System.Collections.Generic;
using System.Security.Policy;

namespace Projekat1.LogDir
{
    public class LogUtils
    {
        private static readonly object locker = new object();
        public static (bool, Log) getLog(Dictionary<string, Log> dictionary, string url)
        {
            bool ima;
            Log l = null;
            lock (locker)
            {
                ima = dictionary.TryGetValue(url, out l);
            }
            return (ima, l);
        }

        public static void writeResponse(Dictionary<string, Log> dictionary, string url, Log l)
        {
            lock (locker)
            {
                dictionary.Add(url, l);
            }
        }
    }
}