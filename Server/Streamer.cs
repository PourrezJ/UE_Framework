using CitizenFX.Core;
using Newtonsoft.Json;
using UE_Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UE_Server
{
    public class Streamer
    {
        //public static List<TextLabel> TextLabels = new List<TextLabel>();

        public static void Init()
        {

        }

        public static void OnPlayerConnected(Player player)
        {
        }
        /*
        public static TextLabel CreateTextLabel(string text, int font, Vector3 position, float range = 5)
        {
            TextLabel label = null;

            lock (TextLabels)
            {
                int handle = TextLabels.Count + 1;
                label = new TextLabel()
                {
                    Handle = handle,
                    Font = font,
                    Position = position,
                    Range = range,
                    Text = text
                };

                TextLabels.Add(label);
                GameMode.Instance.TriggerClientsEvent("CreateTextLabel", JsonConvert.SerializeObject(label));
            }

            return label;
        }*/
    }
}
