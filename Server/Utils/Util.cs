using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Server.Utils
{
    public static class Util
    {
        public static int RandomNumber(int max) => new Random().Next(max);
        public static int RandomNumber(int min, int max) => new Random().Next(min, max);

        public static void Delay(int ms, Action action)
        {
            Task.Delay(ms).ContinueWith((t) => action());
        }

        public static System.Timers.Timer SetInterval(Action action, int ms)
        {
            var t = new System.Timers.Timer(ms);
            t.Elapsed += (s, e) => action();
            t.Start();
            return t;
        }

        public static void StopTimer(System.Timers.Timer timer) => timer.Stop();
    }
}
