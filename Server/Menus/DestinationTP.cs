using CitizenFX.Core;
using UE_Server.Utils.Extensions;
using UE_Shared;
using UE_Shared.Env;
using UE_Shared.MenuManager;
using System;
using Player = CitizenFX.Core.Player;

namespace UE_Server.Menus
{
    public static class DestinationTP
    {
        public static void Init()
        {
            GameMode.RegisterEventHandler("OpenDestinationMenu", new Action<Player, string>(OpenDestinationTP));
        }

        public static void OpenDestinationTP([FromSource]Player player, string currentCity)
        {
            Menu menu = new Menu("", "Transport", currentCity);
            menu.BackCloseMenu = true;
            menu.ItemSelectCallback += OnCallBack;

            foreach (var data in TeleportsLocation.Teleports)
            {
                if (currentCity == data.Key)
                    continue;

                menu.Add(new MenuItem(data.Key, $"Prix: ${CalculPrice(currentCity, data.Key)}", "", true));
            }

            menu.OpenMenu(player);
        }

        private static void OnCallBack(Player client, Menu menu, IMenuItem menuItem, int itemIndex)
        {
            Logger.Info("CallBack");
            if (client.HasMoney(CalculPrice(menu.SubTitle, menuItem.Text)))
            {
                var location = TeleportsLocation.Teleports[menuItem.Text];
                client.SetPosition(location.Pos, 100);
                Logger.Info("set pos");
            }
            else
            {
                Logger.Info("no money");
                // no money
            }
            MenuManager.CloseMenu(client);
        }

        private static double CalculPrice(string current, string wanted)
        {
            Vector3 currentPos = TeleportsLocation.Teleports[current].Pos;
            Vector3 wantedPos = TeleportsLocation.Teleports[wanted].Pos;

            float distance = currentPos.DistanceTo2D(wantedPos);
            return Math.Round(distance * 0.001, 2);
        }
    }
}
