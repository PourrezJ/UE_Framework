using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MongoDB.Bson;
using MongoDB.Driver;
using UE_Server.Entities;
using UE_Server.Menus;
using UE_Server.Utils;
using UE_Shared;
using UE_Shared.Network;

namespace UE_Server
{
    public class GameMode : BaseScript
    {
        #region Fields
        private static GameMode _instance;
        public static GameMode Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public static WorldData WorldData;

        public static bool IsDebug = true;
        public static bool FirstPlayerConnected;
        public static bool ServerStarted;
        public static PlayerList PlayersList;

        #endregion

        #region C4TOR
        public GameMode()
        {
            Instance = this;

            var ci = new CultureInfo("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            PlayersList = Players;
            Logger.Info("Server loading ...");

            Config.LoadConfig("Config.ini");
            Database.MongoDB.Init();

            API.SetMapName("UE Framework");

            var collection = Database.MongoDB.CollectionExist<GameMode>("worlddata");
            if (collection)
            {
                var database = Database.MongoDB.GetMongoDatabase();

                if (database == null)
                    return;

                Task.Run(async () =>
                {
                    Logger.Info("Chargement de la base de donnée");
                    var collectionData = Database.MongoDB.GetCollectionSafe<WorldData>("worlddata");
                    var data = await collectionData.FindAsync<WorldData>(new BsonDocument());
                    if (data == null)
                        return;
                    WorldData = await data.FirstOrDefaultAsync();  
                });
            }
            else
            {
                Logger.Info("Création de la base de donnée");
                // Fresh Server
                WorldData = new WorldData();
                Task.Run(async () => await Database.MongoDB.Insert<WorldData>("worlddata", WorldData));
            }

            Events.Init();
            PlayerManager.Init();
            Commands.Init();
            RPGInventoryManager.Init();
            MenuManager.Init();
            DestinationTP.Init();
            PlayerKeyHandler.Init();

            Tick += OnTick;

            Logger.Info("Events Initialised");
            ServerStarted = true;
            Logger.Info("Server Initialised"); 
        }
        #endregion

        #region Events

        private Task OnTick()
        {
            WorldData?.WordTime.Update();
            FPSCounter.OnTick();


            return Task.FromResult(0);
        }
        #endregion

        #region Methods
        public static async Task Save()
        {
            await Database.MongoDB.Update(WorldData, "worlddata", "0");
        }

        internal static void RegisterEventHandler(string name, Delegate action)
        {
            Instance.EventHandlers[name] += action;
        }

        public static void RegisterTickHandler(Func<Task> tick)
        {
            Instance.Tick += tick;
        }

        public static void DeregisterTickHandler(Func<Task> tick)
        {
            Instance.Tick -= tick;
        }

        public void TriggerClientsEvent(string eventName, params object[] args)
        {
            TriggerClientEvent(eventName, args);
        }
        #endregion
    }
}
