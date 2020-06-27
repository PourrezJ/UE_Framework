using CitizenFX.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using UE_Inventory;
using UE_Server.Controllers;
using UE_Server.Entities;
using UE_Server.Loader;
using UE_Server.Models;
using UE_Server.Utils.Extensions;
using UE_Shared;
using UE_Shared.Network;
using System;
using System.Collections.Generic;

namespace UE_Server.Businesses
{
    [BsonKnownTypes(typeof(GeneralStore))]
    public partial class Business
    {
        #region Fields
        [JsonIgnore]
        public BsonObjectId _id;

        public string BusinnessName = "Undefined Name";
        public Location Location;

        public string Owner { get; set; } = null;

        [BsonIgnore, JsonIgnore]
        public Blip Blip;
        public uint BlipSprite { get; private set; }


        [BsonIgnore, JsonIgnore]
        protected PedNetwork Ped;

        public uint PedHash { get; private set; }

        public int MaxEmployee { get; set; } = 5;
        public bool CanEmploy = false;
        public bool Buyable = false; // Commerce achetable?
        public bool OnSale = false;
        public Dictionary<string, string> Employees = new Dictionary<string, string>();
        public BankAccount BankAccount;
        public int BusinessPrice = 150000;
        public bool Resell = false;
        public DateTime Inactivity = DateTime.Now;
        public Inventory Inventory = new Inventory(500, 40);
        #endregion

        #region Constructor
        public Business(string businnessName, Location location, uint blipSprite, int inventoryMax, uint pedhash = 0, string owner = null, bool buyable = true, bool onsale = true)
        {
            BusinnessName = businnessName;
            Location = location;
            BlipSprite = blipSprite;
            PedHash = pedhash;
            Owner = owner;
            Buyable = buyable;
            OnSale = onsale;
            
            Inventory = new Inventory(inventoryMax, 40);
            BankAccount = new BankAccount(AccountType.Business, BankAccount.GenerateNewAccountNumber(), 0);
            BankAccount.Owner = this;
        }
        #endregion

        #region Loader
        public virtual void Init()
        {
            if (PedHash != 0)
            {
                PedNetwork ped = PedsManager.CreatePed(PedHash, Location);
               // ped.NpcInteractCallBack = OnNpcFirstInteract; // E
              //  ped.NpcSecInteractCallBackAsync = OnNpcSecondaryInteract; // W
                Ped = ped;
            }
            
            Blip = BlipsManager.CreateBlip(BusinnessName, (int)BlipSprite, Location.Pos, Location.Pos.Z /*(Owner == null || OnSale) ? 35 : 2, */);

            if (Employees == null)
                Employees = new Dictionary<string, string>();

            BankAccount.Owner = this;

            BusinessesLoader.BusinessesList.Add(this);
        }
        #endregion

        #region Methods
        public bool HasOwner()
        {
            if (Owner == null || string.IsNullOrEmpty(Owner) || Owner == "")
                return false;
            else
                return true;
        }
        
        public bool IsEmployee(Player client)
            => Employees.ContainsKey(client.GetSteamID());

        public bool IsOwner(Player client)
            => client.GetSteamID().ToLower() == Owner.ToLower();

       public static bool CanIHaveABusiness(string owner) => (BusinessesLoader.BusinessesList.Find(x => x.Owner.ToLower() == owner.ToLower()) == null || (PlayerManager.GetPlayerBySteamID(owner)).StaffRank >= StaffRank.Moderateur) ? true : false;
        
        #endregion
        
        #region Events
        /*
        public void OnNpcFirstInteract(IPlayer client, Ped npc = null)
        {
            OpenMenu(client, npc);
            Task.Run(async () => await OpenMenuAsync(client, npc));
        }

        public async Task OnNpcSecondaryInteract(IPlayer client, Ped npc = null)
        {
            Menu menu = new Menu("ID_SellMenu", BusinnessName, "Administration du magasin", backCloseMenu: true);
            await OpenSellMenu(client, menu);
        }

        public virtual Task OpenMenuAsync(IPlayer client, Ped npc) => Task.CompletedTask;

        public virtual void OpenMenu(IPlayer client, Ped npc) { }*/
        #endregion
    }
}
