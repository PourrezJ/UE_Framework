using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UE_Server.Controllers
{
    public static class BlipsManager
    {
        public static Dictionary<int, Blip> BlipsList = new Dictionary<int, Blip>();

        public static void OnPlayerConnected(Player player)
        {
            player.TriggerEvent("GetAllBlips", JsonConvert.SerializeObject(BlipsList));
        }

        public static Blip CreateBlip(string name, int sprite, Vector3 pos, float rot)
        {
            Blip blip = new Blip(name, sprite, pos, rot);
            lock (BlipsList)
            {            
                int id = BlipsList.Count + 1;
                BlipsList.Add(id, blip);

                lock (GameMode.PlayersList)
                {
                    foreach(Player player in GameMode.PlayersList)
                    {
                        player.TriggerEvent("CreateBlip", id, JsonConvert.SerializeObject(blip));
                    }
                }
            }
            return blip;
        }

        internal static void Destroy(Blip blip)
        {
            //throw new NotImplementedException();
        }
    }

    public class Blip
    {
        public string Name;
        public int Sprite;
        public Vector3 Position;
        public float Rotation;

        public Blip(string name, int sprite, Vector3 pos, float rot)
        {
            this.Name = name;
            this.Sprite = sprite;
            this.Position = pos;
            this.Rotation = rot;
        }
    }
}
