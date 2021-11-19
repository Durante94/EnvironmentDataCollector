using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEnvironmentDataCollector.Models;

namespace WebEnvironmentDataCollector.Util
{
    public class MongoHandler
    {
        private static readonly string collName = "EnvironmentData",
            logCollName = "Log";

        private readonly IMongoClient client;

        public MongoHandler(/*string mongoHost, string mongoUser, string mongoPswd, string mongoDB*/)
        {
            // FORMATO STRINGA DI CONNESSIONE MONGO DB:  mongodb://{NOME UTENTE}:{PASSWORD}@{HOST}/{DB A CUI AUTENTICARSI}?{...OPZIONI DI CONNESSIONE}
            string mongoConn = "mongodb://localhost:27017";//$"mongodb://{mongoUser}:{mongoPswd}@{mongoHost}/{mongoDB}";
            client = new MongoClient(mongoConn);
        }

        private IMongoDatabase GetDb(string dbName)
        {
            IMongoDatabase db;
            if (!client.ListDatabaseNames().ToList().Contains(dbName))
            {
                db = client.GetDatabase(dbName);
                db.CreateCollection(collName, new CreateCollectionOptions
                {
                    TimeSeriesOptions = new TimeSeriesOptions(DataDb.GetTimeSeriesField(), DataDb.metaFieldName)
                });
                db.GetCollection<DataDb>(collName).Indexes.CreateOne(new CreateIndexModel<DataDb>(DataDb.GetIndex()));
            }
            else
                db = client.GetDatabase(dbName);

            return db;
        }

        internal JsonWriterSettings JWS
        {
            get
            {
                return new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
            }
        }

        internal string LogCollName
        {
            get
            {
                return logCollName;
            }
        }

        internal void SaveData(DataDb document, string userDb)
        {
            IMongoDatabase db = GetDb(userDb);

            if (db.GetCollection<DataDb>(collName).Find(x => x.DataRilevazione == document.DataRilevazione).CountDocuments() > 0) return;

            db.GetCollection<DataDb>(collName).InsertOne(document);
        }

        internal List<DataDb> GetData(BsonDocument filter, string userDb)
        {
            IMongoDatabase db = GetDb(userDb);
            db.GetCollection<BsonDocument>(logCollName).InsertOne(new BsonDocument { { "timestamp", DateTime.Now }, { "operazione", "Ricerca" }, { "documento", filter.ToJson(JWS) } });
            return db.GetCollection<DataDb>(collName).Find(filter).Sort(Builders<DataDb>.Sort.Ascending(x => x.DataRilevazione)).ToList();
        }
    }
}
