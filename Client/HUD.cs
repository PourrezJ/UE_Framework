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
        internal static Nui NuiHud;

        
        internal static void Init(int hunger, int thirst, double money)
        {
            Logger.Info("Initialisation du HUD");

            Hunger = hunger;
            Thirst = thirst;
            Money = money;
            GameMode.RegisterEventHandler("UpdateSurvival", new Action<int, int, double>(UpdateSurvival));

            NuiHud = new Nui()
            {
                UIName = "Hud",
                Action = "Activate",
                Data = new { hunger = Hunger, thirst = Thirst, show = false, money = Money, vocaltype = "Parler", mute = false, vocal = false }
            };
            NuiHud.SendNuiMessage();
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
            if (NuiHud == null)
                return;

            if ((DateTime.Now - _lastupdate).TotalMilliseconds < 30)
                return;

            _lastupdate = DateTime.Now;

            NuiHud.UIName = "Hud";
            NuiHud.Action = "Refresh";
            NuiHud.Data = new { hunger = Hunger, thirst = Thirst, show = !hide, money = Money, vocaltype = "Parler", mute = false, vocal = false };
            NuiHud.SendNuiMessage();
        }
    }
}
