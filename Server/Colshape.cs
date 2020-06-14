using CitizenFX.Core;
using UE_Shared;
using System;
using System.Collections.Generic;

namespace UE_Server
{
    public class Colshape
    { 
        public int ID;
        public Vector3 Position;
        public float Range;
        public ColshapeType Type;

        public static void CreateColshape(Vector3 position, float range, ColshapeType type)
        {
            int id = ColshapeManager.Colshapes.Count + 1;
            Colshape colshape = new Colshape()
            {
                ID = id,
                Position = position,
                Range = range,
                Type = type
            };

            lock (ColshapeManager.Colshapes)
            {
                ColshapeManager.Colshapes.Add(colshape);
            }

            GameMode.Instance.TriggerClientsEvent("CreateColshape", id, position, range, type);
        }
    }

    public static class ColshapeManager
    {
        public static List<Colshape> Colshapes = new List<Colshape>();



        public static void Init()
        {
            GameMode.RegisterEventHandler("OnPlayerEnterColshape", new Action<Player, int>(OnPlayerEnterColshape));
            GameMode.RegisterEventHandler("OnPlayerExitColshape", new Action<Player, int>(OnPlayerExitColshape));
        }

        private static void OnPlayerExitColshape(Player Player, int id)
        {
            Console.WriteLine("OnExitColshape");
        }

        private static void OnPlayerEnterColshape(Player Player, int id)
        {
            Console.WriteLine("OnPlayerEnterColshape");
        }
    }
}
