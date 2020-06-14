using System;

namespace UE_Shared.Env
{
    public class WordTime
    {
        public int Hours { set; get; }
        public int Minutes { set; get; }
        public int Seconds { set; get; }

        private DateTime lastRefresh;


        public WordTime(int Hours = 8, int Minutes = 0, int Seconds = 0)
        {
            this.Hours = Hours;
            this.Minutes = Minutes;
            this.Seconds = Seconds;
        }

        public void Update()
        {
            if ((DateTime.Now.Subtract(lastRefresh)).Seconds < 1)
                return;

            lastRefresh = DateTime.Now;

            Seconds += 8;

            if (Seconds >= 60)
            {
                Seconds = 0;
                Minutes++;
            }

            if (Minutes == 60)
            {
                Minutes = 0;
                Hours++;
            };

            if (Hours == 24)
                Hours = 0;
        }

        public new string ToString() => $"{Hours}:{Minutes}:{Seconds}";
    }
}