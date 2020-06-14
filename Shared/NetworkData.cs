using Newtonsoft.Json;
using UE_Inventory;
using UE_Shared.Env;
using System;

namespace UE_Shared.Network
{
    public class Entity 
    {
        public Location Location;
        public float Health;
        public int Model;

    }

    public class VehicleNetwork : Entity
    {

    }

    public class PedNetwork : Entity
    {

    }

    public class HorseNetwork : PedNetwork
    {

    }

    public class PlayerData : Entity
    {
        public string SteamID;
        public StaffRank StaffRank;
        public int Hunger;
        public int Thirst;

        private double _money;
        public double Money
        {
            get => _money;
            set => _money = Math.Round(value, 2);
        }

        [JsonIgnore]
        public Inventory PocketInventory;

        [JsonIgnore]
        public Inventory BagInventory;

        [JsonIgnore]
        public OutfitInventory OutfitInventory;
    }

    public class WorldData
    {
        public WordTime WordTime;
        public WeatherType CurrentWeather;

        public WorldData()
        {
            WordTime = new WordTime();
            CurrentWeather = WeatherType.Sunny;
        }
    }
}
