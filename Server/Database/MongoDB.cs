using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using UE_Shared;
using MongoDB.Bson.Serialization;
using CitizenFX.Core;
using UE_Shared.Network;
using UE_Server.Models;
using UE_Server.Businesses;

namespace UE_Server.Database
{
    public static class MongoDB
    {
        #region Private static variables
        private static IMongoClient _client;
        private static IMongoDatabase _database;
        #endregion

        #region Private methods
     
        #endregion

        #region Public static methods
        public static bool Init()
        {
           Logger.Log("MongoDB Starting ...", LogLevel.Info);

            try
            {
                string host = Config.Get<string>("host");
                string databaseName = Config.Get<string>("database");
                string user = Config.Get<string>("user");
                string password = Config.Get<string>("password");
                int port = Config.Get<int>("port");

                if (!string.IsNullOrEmpty(host))
                    _client = new MongoClient($"mongodb://{user}:{password}@{host}:{port}");
                else
                    _client = new MongoClient();

                _database = _client.GetDatabase("redm");

                var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
                ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);


                BsonSerializer.RegisterSerializer(typeof(Vector3), new VectorSerializer());

                BsonClassMap.RegisterClassMap<Location>(cm =>
                {
                    cm.AutoMap();
                    cm.MapProperty(c => c.Pos).SetSerializer(new VectorSerializer());
                    cm.MapProperty(c => c.Rot).SetSerializer(new VectorSerializer());
                });

                BsonClassMap.RegisterClassMap<PlayerData>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(p => p.SteamID);
                });

                Logger.Log("MongoDB Started!", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async static Task Insert<T>(string collectionName, T objet, [System.Runtime.CompilerServices.CallerMemberName] string caller = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            try
            {
                await GetCollectionSafe<T>(collectionName).InsertOneAsync(objet);
            }
            catch (MongoWriteException be)
            {
                Logger.Error(be);
            }
        }

        public async static Task<ReplaceOneResult> Update<T>(T objet, string collectionName, object ID, int requests = 1, [System.Runtime.CompilerServices.CallerMemberName] string caller = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            try
            {
                return await GetCollectionSafe<T>(collectionName).ReplaceOneAsync(Builders<T>.Filter.Eq("_id", ID), objet);
            }
            catch (BsonException be)
            {
                Logger.Error(be);
            }

            return null;
        }

        public async static Task<DeleteResult> Delete<T>(string collectionName, object ID, [System.Runtime.CompilerServices.CallerMemberName] string caller = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            try
            {
                if (Config.Get<bool>("DBProfiling"))
                    Logger.Info($"~m~{caller}() in {file.Substring(file.LastIndexOf('\\') + 1)} line {line} - Object ID: {ID}");

                return await _database.GetCollection<T>(collectionName).DeleteOneAsync(Builders<T>.Filter.Eq("_id", ID));
            }
            catch (BsonException be)
            {
                Logger.Error(be);
            }

            return null;
        }

        public static IMongoCollection<T> GetCollectionSafe<T>(string collectionName)
        {
            if (_database == null)
                return null;

            if (_database.GetCollection<T>(collectionName) == null)
                _database.CreateCollection(collectionName);

            return _database.GetCollection<T>(collectionName);
        }

        public static bool CollectionExist<T>(string collectionName)
        {

            if (_database == null)
                return false;

            if (_database.GetCollection<T>(collectionName) == null)
                return false;

            if (_database.GetCollection<T>(collectionName).CountDocuments(new BsonDocument()) == 0)
                return false;

            return true;
        }

        public static IMongoDatabase GetMongoDatabase() => _database;

        public async static Task<UpdateResult> UpdateBankAccount(BankAccount bankAccount, int requests, [System.Runtime.CompilerServices.CallerMemberName] string caller = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            try
            {
                if (Config.Get<bool>("DBProfiling"))
                    Logger.Trace($"~m~{caller}() in {file.Substring(file.LastIndexOf('\\') + 1)} line {line} - Requests: {requests} - Bank account: {bankAccount.AccountNumber} - Type: {bankAccount.AccountType.ToString()}");

                if (bankAccount.AccountType == AccountType.Business)
                {
                    var collection = GetCollectionSafe<Business>("businesses");
                    var filter = Builders<Business>.Filter.Eq("_id", ((Business)bankAccount.Owner)._id);
                    var update = Builders<Business>.Update.Set("BankAccount", bankAccount);
                    return await collection.UpdateOneAsync(filter, update);
                }
                /*
                else if (bankAccount.AccountType == AccountType.Faction)
                {
                    var collection = GetCollectionSafe<Faction>("factions");
                    var filter = Builders<Faction>.Filter.Eq("_id", ((Faction)bankAccount.Owner).FactionName);
                    var update = Builders<Faction>.Update.Set("BankAccount", bankAccount);
                    return await collection.UpdateOneAsync(filter, update);
                }
                else if (bankAccount.AccountType == AccountType.Society)
                {
                    var collection = GetCollectionSafe<Society.Society>("society");
                    var filter = Builders<Society.Society>.Filter.Eq("_id", ((Society.Society)bankAccount.Owner)._id);
                    var update = Builders<Society.Society>.Update.Set("BankAccount", bankAccount);
                    return await collection.UpdateOneAsync(filter, update);
                }
                else if (bankAccount.AccountType == AccountType.Personal)
                {
                    var collection = GetCollectionSafe<PlayerHandler>("players");
                    var filter = Builders<PlayerHandler>.Filter.Eq("_id", ((PlayerHandler)bankAccount.Owner).PID);
                    var update = Builders<PlayerHandler>.Update.Set("BankAccount", bankAccount);
                    return await collection.UpdateOneAsync(filter, update);
                }*/
            }
            catch (BsonException be)
            {
                Logger.Error(be);
            }

            return null;
        }
        #endregion
    }
}