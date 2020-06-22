using CitizenFX.Core;
using CitizenFX.Core.Native;
using MongoDB.Driver;
using UE_Server.Controllers;
using UE_Server.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UE_Shared.Network;
using UE_Shared;
using UE_Inventory;
using MongoDB.Bson.IO;

namespace UE_Server.Entities
{
    public static class PlayerManager
    {
        public static Dictionary<Player, PlayerData> PlayerDataList = new Dictionary<Player, PlayerData>();

        private static double moneyStart = 1000;
        private static int maxClients;
        private static string[] whitelist;

        private readonly static Location[] FirstSpawnPos =
        {
            new Location(new Vector3(2520.084f, -1250.082f, 50.04138f), new Vector3(0,0,0)),
            new Location(new Vector3(1232.205f, -1251.088f, 73.67763f), new Vector3(0,0,0))
        };

        public static void Init()
        {
            GameMode.RegisterEventHandler("GetPlayerData", new Action<Player, string>(OnPlayerConnecting));
            GameMode.RegisterEventHandler("PlayerUpdatePosition", new Action<Player, string>(PlayerUpdatePosition));
            GameMode.RegisterEventHandler("playerDropped", new Action<Player, string>(OnPlayerDropped));
            GameMode.RegisterEventHandler("OpenInventory", new Action<Player>(OpenInventory));
            GameMode.RegisterEventHandler("UpdateInventory", new Action<Player, string, string>(UpdateInventory));
            GameMode.RegisterEventHandler("playerConnecting", new Action<Player, string, CallbackDelegate>(BeginPlayerConnecting));

            GameMode.RegisterEventHandler("PlayerCreation", new Action<Player, string>(PlayerCreation));
        }

        private static void PlayerCreation([FromSource]Player player, string data)
        {
            PedCharacter charData = Newtonsoft.Json.JsonConvert.DeserializeObject<PedCharacter>(data);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(charData));
        }

        internal static void BeginPlayerConnecting([FromSource] Player source, string playerName, CallbackDelegate DenyWithReason)
        {
            try
            {
                if (source.Identifiers["steam"] == null)
                {
                    DenyWithReason?.Invoke($"Vous devez être connecter a steam.");
                    API.CancelEvent();
                }

                maxClients = API.GetConvarInt("sv_maxclients", 32);

                var strWhitelist = API.GetConvar("sv_whitelist", "");

                whitelist = strWhitelist != "" ? strWhitelist.Split(',') : null;

                if (whitelist != null)
                {
                    if (!whitelist.Contains(source.Identifiers.Where(i => i.Contains("steam")).FirstOrDefault().ToString()))
                    {
                        DenyWithReason?.Invoke($"Vous n'êtes pas whitelist. \nPlus d'info sur Discord.gg/trucmuche");
                        API.CancelEvent();
                    }
                }

                var count = GameMode.PlayersList.Count();
                Debug.WriteLine($"Connecting: '{source.Name}' (steam: {source.Identifiers.Where(i => i.Contains("steam")).FirstOrDefault().ToString()} ip: {source.Identifiers.Where(i => i.Contains("ip")).FirstOrDefault().ToString()}) | Player count {count}/{maxClients}");

                
                if (count >= maxClients)
                {
                    DenyWithReason?.Invoke($"Le serveur est plein avec {count} joueurs sur {maxClients}.");
                    API.CancelEvent();
                }
                BaseScript.TriggerClientEvent("playerConnecting", source.Handle, playerName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"HardCap PlayerConnecting Error: {ex.Message}");
            }
        }

        private static void UpdateInventory([FromSource]Player client, string pocketStr, string bagStr)
        {
            Logger.Info("Update Inventory");
            PlayerData playerData = client.GetPlayerDatabase();

            if (playerData == null)
                return;

            Inventory pocketinventory = Newtonsoft.Json.JsonConvert.DeserializeObject<Inventory>(pocketStr);
            playerData.PocketInventory = pocketinventory;

            Inventory baginventory = null;

            if (string.IsNullOrEmpty(bagStr))
            {
                baginventory = Newtonsoft.Json.JsonConvert.DeserializeObject<Inventory>(bagStr);
                playerData.BagInventory = baginventory;
            }

            Task.Run(()=> playerData.Update());
        }

        private static void PlayerUpdatePosition([FromSource]Player client, string data)
        {       
            var ph = client.GetPlayerDatabase();

            if (ph == null)
                return;


            ph.Location = Newtonsoft.Json.JsonConvert.DeserializeObject<Location>(data);

            Task.Run(()=> ph.Update());
        }

        public static async Task LoadPlayer(Player client, string social, bool firstSpawn = false)
        {
            PlayerData playerData = null; 
            try
            {
                Logger.Info(Newtonsoft.Json.JsonConvert.SerializeObject(client.Identifiers));

                if (string.IsNullOrEmpty(client.GetSteamID()))
                { 
                    Logger.Warn("SteamID Unknown for IP: " + client.Identifiers["ip"]);
                    return;
                }

                if (await PlayerDataExist(client))
                {
                    Logger.Info("Chargement des données joueur de: " + social);
                    playerData = await GetPlayerDatabase(client.GetSteamID());
                }
                else
                {
                    Logger.Info("Lancement du charcreator pour le joueur " + social);
                    client.TriggerEvent("OpenCharCreator");


                    /*
                     *  Besoin d'être ajouter après le charcreator
                    Logger.Info("Création du personnage pour: " + social);

                    playerData = new PlayerData()
                    {
                        Health = 100,
                        Hunger = 100,
                        Thirst = 100,
                        Location = FirstSpawnPos[UE_Shared.Utils.RandomNumber(FirstSpawnPos.Length)],
                        Money = moneyStart,
                        SteamID = client.GetSteamID(),
                        PocketInventory = new Inventory(15, 4),
                        OutfitInventory = new OutfitInventory()
                    };
                    playerData.Location = FirstSpawnPos[UE_Shared.Utils.RandomNumber(FirstSpawnPos.Length)];
                    await Database.MongoDB.Insert("players", playerData);*/
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }

            lock (PlayerDataList)
            {
                PlayerDataList.Add(client, playerData);
            }

            client.TriggerEvent("ClientConnected", Newtonsoft.Json.JsonConvert.SerializeObject(playerData), Newtonsoft.Json.JsonConvert.SerializeObject(GameMode.WorldData));
        }

        private static void OnPlayerDropped([FromSource]Player player, string reason)
        {
            if (GameMode.PlayersList.Count() == 0)
            {
                GameMode.FirstPlayerConnected = false;
                Logger.Info("Deconnection du dernier joueur FirstPlayerConnected est maintenant false");
            }

            Logger.Info($"Player {player.Name} dropped (Reason: {reason}).");
        }

        private static void OnPlayerConnecting([FromSource]Player player, string socialClub)
        {
            Logger.Info($"Connection du joueur {player.Name} {socialClub}.");
            while (!GameMode.ServerStarted)
                Thread.Sleep(50);

            // Streamer.OnPlayerConnected(player);
            if (!GameMode.FirstPlayerConnected)
            {
                GameMode.FirstPlayerConnected = true;
                player.TriggerEvent("SpawnTrain");
                // Todo envoi des infos serveurs: Vehicule ...
            }

            Task.Run(async () => {
 

                await LoadPlayer(player, socialClub);
            });
            BlipsManager.OnPlayerConnected(player);
            PedsManager.OnPlayerConnected(player);
        }

        private static void OpenInventory([FromSource]Player player)
        {
            Logger.Debug("Open Inventory");

            Inventory distant = null; // Todo besoin de connaitre quel inventaire distant ouvrir si besoin

            var ph = player.GetPlayerDatabase();

            if (ph == null)
                return;

            new RPGInventoryMenu(ph.PocketInventory, ph.OutfitInventory, ph.BagInventory).OpenMenu(player);
        }

        public static Task<PlayerData> GetPlayerDatabase(string steamid) =>
            Database.MongoDB.GetCollectionSafe<PlayerData>("players").Find(p => p.SteamID.ToLower() == steamid.ToLower()).FirstOrDefaultAsync();

        public static async Task<bool> PlayerDataExist(Player player)
        {
            try
            {
                return await Database.MongoDB.GetCollectionSafe<PlayerData>("players").Find(p => p.SteamID == player.GetSteamID()).AnyAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public static bool AddItem(Player client, Item item, int quantity = 1)
        {
            PlayerData playerData = client.GetPlayerDatabase();

            if (playerData == null)
                return false;

            if (playerData.PocketInventory.AddItem(item, quantity))
            {
                //if (RPGInventoryManager.HasInventoryOpen(this.Client))
                //{
                //    var rpg = RPGInventoryManager.GetRPGInventory(this.Client);
                //    if (rpg != null)
                //        RPGInventoryManager.Refresh(this.Client, rpg);
                //}

                //item.OnPlayerGetItem(client);
                return true;
            }
            else if (playerData.BagInventory != null && playerData.BagInventory.AddItem(item, quantity))
            {
                //if (RPGInventoryManager.HasInventoryOpen(client))
                //{
                //    var rpg = RPGInventoryManager.GetRPGInventory(this.Client);
                //    if (rpg != null)
                //        RPGInventoryManager.Refresh(this.Client, rpg);
                //}

                //item.OnPlayerGetItem(this.Client);
                return true;
            }
            else
                return false;
        }

        public static PlayerData GetPlayerBySteamID(string steamID)
        {
            try
            {
                var players = GameMode.PlayersList;
                for (int a = 0; a < players.Count(); a++)
                {
                    if (players[a] == null)
                        continue;

                    var psteamID = players[a].GetSteamID();
                    if (steamID.ToLower() == steamID.ToLower())
                        return players[a].GetPlayerDatabase();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("GetPlayerBySCN: " + steamID +  " " + ex);
            }

            return null;
        }
    }
}
