using CitizenFX.Core;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using UE_Shared;
using UE_Shared.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UE_Server.Entities
{
    public static class PedsManager
    {
        public static List<PedNetwork> PedsList = new List<PedNetwork>();

        public delegate Task NpcPrimaryCallBackAsync(Player client, Ped npc);
        public delegate Task NpcSecondaryCallBackAsync(Player client, Ped npc);

        public delegate void NpcPrimaryCallBack(Player client, Ped npc);
        public delegate void NpcSecondaryCallBack(Player client, Ped npc);

        [JsonIgnore, BsonIgnore]
        public static NpcPrimaryCallBackAsync NpcInteractCallBackAsync { get; set; }
        [JsonIgnore, BsonIgnore]
        public static NpcSecondaryCallBackAsync NpcSecInteractCallBackAsync { get; set; }

        [JsonIgnore, BsonIgnore]
        public static NpcPrimaryCallBack NpcInteractCallBack { get; set; }
        [JsonIgnore, BsonIgnore]
        public static NpcSecondaryCallBack NpcSecInteractCallBack { get; set; }


        public static PedNetwork CreatePed(PedHash pedHash, Location position)
        {
            var ped = new PedNetwork()
            {
                Model = (int)pedHash,
                Location = position
            };

            lock (PedsList)
            {
                PedsList.Add(ped);
            }

            GameMode.Instance.TriggerClientsEvent("CreatePedFromSrv", JsonConvert.SerializeObject(ped));
            return ped;
        }

        public static void OnPlayerConnected(Player player)
        {
            if (PedsList.Count > 0)
                player.TriggerEvent("GetAllPeds", Newtonsoft.Json.JsonConvert.SerializeObject(PedsList));
        }
    }
}
