using CitizenFX.Core;
using Newtonsoft.Json;
using UE_Shared;
using System;
using System.Collections.Generic;

namespace UE_Client.Controllers
{
    internal static class PedsManager
    {
        internal static void Init()
        {
            GameMode.RegisterEventHandler("GetAllPeds", new Action<string>(CreateAllPeds));
            GameMode.RegisterEventHandler("CreatePedFromSrv", new Action<string>(CreatePedFromSrv));
        }

        private static async void CreatePedFromSrv(string data)
        {
            UE_Shared.Network.PedNetwork peddata = JsonConvert.DeserializeObject<UE_Shared.Network.PedNetwork>(data);
            //Logger.Debug(PedHash.ToString());
            /*
            RedM.External.Ped ped = await World.CreatePed(new Model(peddata.Model), peddata.Location.Pos, peddata.Location.Rot.Z, false);

            if (ped == null)
            {
                Logger.Info("ped is null");
                return;
            }

            while (!ped.Exists())
                await BaseScript.Delay(10);

           // Function.Call(Hash.TASK_WANDER_STANDARD, ped.Handle, 0, 0); // make not static

            ped.SetPosition(peddata.Location.Pos, peddata.Location.Rot.Z);*/
        }

        private static async void CreateAllPeds(string obj)
        {
            /*
            var pedsData = JsonConvert.DeserializeObject<List<UE_Shared.Network.PedNetwork>>(obj);

            foreach(var pednet in pedsData)
            {
                await World.CreatePed(pednet.Model, pednet.Location.Pos, pednet.Location.Rot.Z, true);
            }*/
        }
    }
}
