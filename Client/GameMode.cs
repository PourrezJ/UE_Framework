﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using UE_Client.Env;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using UE_Client.Controllers;
using UE_Client.Colshape;
using UE_Shared;

namespace UE_Client
{
    public class GameMode : BaseScript
    {
        #region Fields
        internal static GameMode Instance { get; set; }
        public bool SnowActivate { get; private set; }

        internal static bool IsDebug = true;
 
        internal static List<Ped> PedsHandles = new List<Ped>();

        private static bool firstTick = true;
        #endregion

        #region C4TOR
        public GameMode()
        {
            Instance = this;

            UE_Shared.Logger.Info("Client Initialise...");

            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
            Tick += OnTick;

            Logger.Info("Client Initialised...");
        }
        #endregion

        #region Events

        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            foreach(Ped ped in PedsHandles)
            {
                ped.Delete();
            }

            foreach (var blipData in BlipsManager.BlipsList)
            {
                blipData.Value.Delete();
            }

            Debug.WriteLine("onClientResourceStop called!");
        }

        private async void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            API.DoScreenFadeOut(0);

            while (!API.NetworkIsPlayerActive(Game.Player.Handle))
                await Delay(50);

            Logger.Info("Demande des informations joueurs");
            TriggerServerEvent("GetPlayerData", Game.Player.Name); 
            API.DestroyAllCams(true);
        }


        private Task OnTick()
        {
            if (firstTick)
            {
                firstTick = false;
                
                //IplLoader.Init();
                RAPI.Init();
                BlipsManager.Init();
                MenuManager.Init();
                ColshapeManager.Init();
                Commands.Init();
                SessionManager.Init();
                PedsManager.Init();

                for (int i = 0; i < 16; i++)
                    Function.Call(Hash.ENABLE_DISPATCH_SERVICE, i, true);
            }
            
           // Streamer.OnTick();
           // ColshapeManager.OnTick();
            //WeatherManager.OnTick();
            //HUD.OnTick();
            //Toast.Tick();

            //Utils.Misc.ShowControlPressed();    
            return Task.FromResult(0);
        }
        #endregion

        #region Methods  
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

        public void RegisterNUICallback(string msg, Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            Debug.WriteLine($"Registering NUI EventHandler for {msg}");
            API.RegisterNuiCallbackType(msg);

            EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>((body, resultCallback) =>
            {
                callback.Invoke(body, resultCallback);
            });
            EventHandlers[$"{msg}"] += new Action<ExpandoObject, CallbackDelegate>((body, resultCallback) =>
            {
                callback.Invoke(body, resultCallback);
            });
        }
        #endregion
    }
}