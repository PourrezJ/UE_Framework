using CitizenFX.Core;
using UE_Server.Entities;
using UE_Shared;
using UE_Shared.Network;

namespace UE_Server.Utils.Extensions
{
    public static class PlayerExtension
    {
        public static string GetSteamID(this Player player) 
            => player.Identifiers["steam"];

        public static PlayerData GetPlayerDatabase(this Player client)
        {
            if (client == null)
                return null;

            if (PlayerManager.PlayerDataList.TryGetValue(client, out PlayerData value))
                return value;

            return null;
        }
        public static bool HasMoney(this Player client, double money)
        {
            var playerData = client.GetPlayerDatabase();

            if (playerData == null)
                return false;

            return playerData.HasMoney(money);
        }

        public static void SendNotification(this Player client, string text)
        {
            if (text == "")
                return;

            client.TriggerEvent("notify", "Notification", text, 7000);
        }

        public static void SendNotificationError(this Player client, string text)
        {
            client.TriggerEvent("alertNotify", "Erreur", text, 7000);
        }

        public static void SendNotificationSuccess(this Player client, string text)
        {
            client.TriggerEvent("successNotify", "Succès", text, 7000);
        }

        public static void SetPosition(this Player client, Vector3 position, int fadedelay = 0)
        {
            client.TriggerEvent("SetPlayerPosition", position.X, position.Y, position.Z, fadedelay);
        }

        public static Location GetLocation(this Player client)
        {
            return client.GetPlayerDatabase()?.Location ?? new Location(new Vector3(), new Vector3());
        }

        public static StaffRank GetStaffRank(this Player client)
        {
            return client.GetPlayerDatabase()?.StaffRank ?? StaffRank.Citoyen;
        }
    }
}
