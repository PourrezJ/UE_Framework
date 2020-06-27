using CitizenFX.Core;
using CitizenFX.Core.Native;
using UE_Inventory;
using UE_Inventory.ItemsClass;
using UE_Server.Controllers;
using UE_Server.Entities;
using UE_Server.Utils.Extensions;
using UE_Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UE_Server
{
    public static class Commands
    {
        public static void Init()
        {
            API.RegisterCommand("blip", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Player player = GameMode.PlayersList[source];

                if (player == null)
                    return;

                BlipsManager.CreateBlip("test", Convert.ToInt32(args[0]), player.GetPlayerDatabase().Location.Pos, 0);
            }), false);

            API.RegisterCommand("additem", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Player player = GameMode.PlayersList[source];

                if (player == null)
                    return;

                if (player.GetStaffRank() < StaffRank.Moderateur)
                    return;
                    

                string command = "";

                for (int i = 0; i < args.Count(); i++)
                    command += $" {args[i]}";

                command = command.ToLower();

                try
                {
                    string[] infos = command.Split(new[] { " x" }, StringSplitOptions.RemoveEmptyEntries);
                    int number = (Convert.ToInt32(infos[1]) != 0) ? Convert.ToInt32(infos[1]) : 1;
                    string itemName = infos[0].Remove(0, 1);
                    Item item = Items.ItemsList.Find(x => x.Name.ToLower() == itemName);

                    if (item != null)
                    {
                        
                        if (PlayerManager.AddItem(player, item, number))
                            player.SendNotificationSuccess($"Vous avez ajouté {number} {item.Name}");
                        else
                            player.SendNotificationError($"Vous n'avez pas la place dans votre inventaire pour {item.Name}");
                    }
                    else
                        player.SendNotificationError("Item inconnu.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }), false);


            API.RegisterCommand("charcreator", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Player player = GameMode.PlayersList[source];

                if (player == null)
                    return;
                Logger.Info("open char creator");
                player.TriggerEvent("OpenCharCreator");
            }), false);

            API.RegisterCommand("ped", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Player player = GameMode.PlayersList[source];

                if (player == null)
                    return;

                PedsManager.CreatePed((uint)API.GetHashKey("mp_male"), player.GetLocation());
            }), false);
        }
    }
}
