using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using UE_Shared;
using UE_Shared.Network;
using System;
using System.Threading.Tasks;

namespace UE_Client
{
    internal static class SessionManager
    {
        private static PlayerData PlayerData;
        private static WorldData WorldData;

        private static bool ClientReady;

        internal static void Init()
        {
            GameMode.RegisterEventHandler("ClientConnected", new Action<string, string>(ClientConnected));
        }

        private static async void ClientConnected(string playerDataSTR, string worldDataSTR)
        {
            Logger.Debug("Starting client Connect...");
            
            await Game.Player.ChangeModel(new Model(CitizenFX.Core.PedHash.FreemodeMale01));


            /*
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(playerDataSTR);
            
            WorldData = JsonConvert.DeserializeObject<WorldData>(worldDataSTR);
            Game.Player.Character.Position = PlayerData.Location.Pos;
            Game.Player.Character.Rotation = PlayerData.Location.Rot;
            Function.Call(Hash.PAUSE_CLOCK, true);
            Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, WorldData.WordTime.Hours, WorldData.WordTime.Minutes, WorldData.WordTime.Seconds);

            Function.Call(Hash.NETWORK_SET_FRIENDLY_FIRE_OPTION, true);

            API.DoScreenFadeIn(0);
            API.ShutdownLoadingScreen();
            HUD.Init(PlayerData.Hunger, PlayerData.Thirst, PlayerData.Money);
            API.SetRelationshipBetweenGroups(5, (uint)Game.GenerateHash("Player"), (uint)Game.GenerateHash("Player"));

            GameMode.RegisterTickHandler(OnTick);

            Function.Call(Hash.SET_WANTED_LEVEL_MULTIPLIER, 0f);
            Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player.Character.Handle, false);
            */
            ClientReady = true;
            Logger.Info("End client connected...");
        }

        private static int tick;
        private static DateTime lastUpdate = DateTime.Now;
        public static Task OnTick()
        {
            tick++;
            if (tick > 20)
            {
                tick = 0;
                Function.Call(Hash.RESTORE_PLAYER_STAMINA, Game.Player.Handle, 100.0f);

                var time = WorldData.WordTime;
                time.Update();
                Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, time.Hours, time.Minutes, time.Seconds);


                if ((DateTime.Now - lastUpdate).TotalSeconds > 15)
                {
                    lastUpdate = DateTime.Now;

                    if (Game.Player.Character.IsSittingInVehicle())
                    {

                    }

                    GameMode.TriggerServerEvent("PlayerUpdatePosition", JsonConvert.SerializeObject(new Location(Game.Player.Character.Position, Game.Player.Character.Rotation)));
                }
            }

            return Task.FromResult(0);
        }
    }
}
