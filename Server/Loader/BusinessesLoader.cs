using MongoDB.Driver;
using UE_Server.Businesses;
using UE_Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Server.Loader
{
    public static class BusinessesLoader
    {
        public static List<Business> BusinessesList = new List<Business>();
        public static async Task LoadAllBusinesses()
        {
            Logger.Info("--- Start loading all businesses in database ---");

            var _businessesList = await Database.MongoDB.GetCollectionSafe<Business>("businesses").AsQueryable().ToListAsync();

            foreach (var _businesses in _businessesList)
                _businesses.Init();

            Util.SetInterval(async () =>
            {
                foreach (var _businesses in _businessesList)
                {
                    _businesses.UpdateInBackground();
                    await Task.Delay(50);
                }
            }, (int)TimeSpan.FromMinutes(5).TotalMilliseconds);

            Logger.Info($"--- Finish loading all businesses in database: {_businessesList.Count} ---");
        }

    }
}
