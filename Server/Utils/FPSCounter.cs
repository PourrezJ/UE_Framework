using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Server.Utils
{
    public static class FPSCounter
    {
        private static int lastTick;
        private static int lastFrameRate;
        private static int frameRate;

        private static string addedTittle = "FiveM |";
        private static string title;

        public static void OnTick()
        {
            if (!Console.Title.Contains(addedTittle))
                title = Console.Title;
            else
                title = "FiveM |";

            var time = GameMode.WorldData?.WordTime.ToString() ?? "";
            Console.Title = addedTittle + $" FPS: {CalculateFrameRate()} Joueurs: {GameMode.PlayersList.Count()} Heures: {time}";
        }

        public static int CalculateFrameRate()
        {
            if (Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = Environment.TickCount;
            }

            frameRate++;
            return lastFrameRate;
        }
    }
}