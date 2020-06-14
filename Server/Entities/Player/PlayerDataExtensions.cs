using CitizenFX.Core;
using UE_Inventory;
using UE_Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Server.Entities
{
    public static class PlayerDataExtensions
    {
        #region Misc
        public static Player GetPlayer(this PlayerData playerData)
        {
            return PlayerManager.PlayerDataList.First(p => p.Value == playerData).Key;
        }
        #endregion

        #region Save
        public static void Update(this PlayerData playerData)
        {
            Task.Run(async () =>
            {
                var result = await Database.MongoDB.Update(playerData, "players", playerData.SteamID);

                if (result.MatchedCount == 0)
                    Logger.Warn($"Update error for player {playerData.SteamID}");
            });
        }
        #endregion

        #region Inventory
        public static bool AddItem(this PlayerData playerData, Item item, int quantity = 1)
        {
            Player client = playerData.GetPlayer();

            if (playerData.PocketInventory.AddItem(item, quantity))
            {
                if (RPGInventoryManager.HasInventoryOpen(client))
                {
                    var rpg = RPGInventoryManager.GetRPGInventory(client);
                    if (rpg != null)
                        RPGInventoryManager.Refresh(client, rpg);
                }

                return true;
            }
            else if (playerData.BagInventory != null && playerData.BagInventory.AddItem(item, quantity))
            {
                if (RPGInventoryManager.HasInventoryOpen(client))
                {
                    var rpg = RPGInventoryManager.GetRPGInventory(client);
                    if (rpg != null)
                        RPGInventoryManager.Refresh(client, rpg);
                }

                return true;
            }
            else
                return false;
        }

        public static bool HasItemID(this PlayerData playerData, ItemID id)
        {
            if (playerData.PocketInventory.HasItemID(id))
                return true;
            else if (playerData.BagInventory != null && playerData.BagInventory.HasItemID(id))
                return true;
            else
                return false;
        }

        public static bool DeleteItem(this PlayerData playerData, int slot, string inventoryType, int quantity)
        {
            switch (inventoryType)
            {
                case InventoryTypes.Pocket:
                    return playerData.PocketInventory.Delete(slot, quantity);

                case InventoryTypes.Bag:
                    return playerData.BagInventory.Delete(slot, quantity);

                case InventoryTypes.Outfit:
                    return playerData.OutfitInventory.Delete(slot, quantity);
            }

            return false;
        }
        #endregion

        #region Money
        public static bool HasMoney(this PlayerData playerData, double money)
        {
            if (playerData.Money >= money)
            {
                playerData.Money -= money;
                return true;
            }
            return false;
        }
        #endregion
    }
}
