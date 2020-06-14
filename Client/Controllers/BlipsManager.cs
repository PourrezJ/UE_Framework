using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UE_Client.Controllers
{
    internal static class BlipsManager
    {
        internal static Dictionary<int, Blip> BlipsList = new Dictionary<int, Blip>();

        internal static void Init()
        {
            GameMode.RegisterEventHandler("GetAllBlips", new Action<string>(GetAllBlips));
            GameMode.RegisterEventHandler("CreateBlip", new Action<int, dynamic>(CreateBlip));
        }

        private static void CreateBlip(int id, dynamic data)
        {
            dynamic blipdata = JsonConvert.DeserializeObject(data);
            dynamic pos = blipdata["Position"];
            //BlipsList.Add(id, new Blip((string)blipdata["Name"], (int)blipdata["Sprite"], new Vector3((float)pos["X"], (float)pos["Y"], (float)pos["Z"]), (float)blipdata["Rotation"]));
            BlipsList.Add(id, World.CreateBlip(new Vector3((float)pos["X"], (float)pos["Y"], (float)pos["Z"]), blipdata["Sprite"]));
        }

        private static void GetAllBlips(string data)
        {
            var blips = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(data);

            foreach(var blipDict in blips)
            {
                dynamic blipdata = blipDict.Value;
                dynamic pos = blipDict.Value["Position"];
                //BlipsList.Add(blipDict.Key, new Blip((string)blipdata["Name"], (int)blipdata["Sprite"], new Vector3((float)pos["X"], (float)pos["Y"], (float)pos["Z"]), (float)blipdata["Rotation"]));
                BlipsList.Add(blipDict.Key, World.CreateBlip(new Vector3((float)pos["X"], (float)pos["Y"], (float)pos["Z"]), blipdata["Sprite"]));
            }
        }
    }
}
