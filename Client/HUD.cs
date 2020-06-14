using UE_Client.Utils;
using UE_Shared;
using System;

namespace UE_Client
{
    internal static class HUD
    {
        internal static int Hunger;
        internal static int Thirst;
        internal static double Money;

        
        internal static void Init(int hunger, int thirst, double money)
        {
            Logger.Info("Initialisation du HUD");
            Hunger = hunger;
            Thirst = thirst;
            Money = money;
            GameMode.RegisterEventHandler("UpdateSurvival", new Action<int, int, double>(UpdateSurvival));
            
            var data = new Nui()
            {
                UIName = "Hud",
                Action = "Activate",
                Data = true
            };
            data.SendNuiMessage();
        }

        private static void UpdateSurvival(int hunger, int thirst, double money)
        {
            Hunger = hunger;
            Thirst = thirst;
            Money = money;
        }

        private static DateTime _lastupdate = DateTime.Now;
        private static bool hide;
        internal static void OnTick()
        {

            if (hide)
            {
                new Nui()
                {
                    UIName = "Hud",
                    Action = "Activate",
                    Data = true
                }.SendNuiMessage();
                hide = false;
            }

            if ((DateTime.Now - _lastupdate).TotalMilliseconds < 30)
                return;

            _lastupdate = DateTime.Now;

            if (hide)
                return;

            var data = new Nui()
            {
                UIName = "Hud",
                Action = "Refresh",
                Data = new { hunger = Hunger, thirst = Thirst, money = Money, vocaltype = "Parler", mute = false, vocal = false }
            };
            data.SendNuiMessage();
        }
    }
}
