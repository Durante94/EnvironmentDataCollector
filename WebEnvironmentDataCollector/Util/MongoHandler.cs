using MongoDB.Bson;
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
        private static readonly string collName = "EnvironmentData";

        private IMongoClient dbClient;

        private IMongoDatabase db;

        public MongoHandler()
        {
        }

        internal void Init(string mongoHost, string mongoUser, string mongoPswd, string mongoDB)
        {
            // FORMATO STRINGA DI CONNESSIONE MONGO DB:  mongodb://{NOME UTENTE}:{PASSWORD}@{HOST}/{DB A CUI AUTENTICARSI}?{...OPZIONI DI CONNESSIONE}
            string mongoConn = "mongodb://localhost:27017";//$"mongodb://{mongoUser}:{mongoPswd}@{mongoHost}/{mongoDB}";
            dbClient = new MongoClient(mongoConn);
            db = dbClient.GetDatabase("test");
        }

        private IMongoDatabase GetDb(string dbName)
        {
            IMongoDatabase db;
            if (!dbClient.ListDatabaseNames().ToList().Contains(dbName))
            {
                db = dbClient.GetDatabase(dbName);
                db.CreateCollection(collName, new CreateCollectionOptions
                {
                    TimeSeriesOptions = new TimeSeriesOptions(DataDb.GetTimeSeriesField(), DataDb.metaFieldName)
                });
                db.GetCollection<DataDb>(collName).Indexes.CreateOne(new CreateIndexModel<DataDb>(DataDb.GetIndex()));
            }
            else
                db = dbClient.GetDatabase(dbName);

            return db;
        }

        internal void SaveData(DataDb document)
        {
            if (db.GetCollection<DataDb>(collName).Find(x => x.DataRilevazione == document.DataRilevazione).CountDocuments() > 0) return;

            db.GetCollection<DataDb>(collName).InsertOne(document);
        }

        internal List<DataDb> GetData(BsonDocument filter)
        {
            return db.GetCollection<DataDb>(collName).Find(filter).Sort(Builders<DataDb>.Sort.Ascending(x => x.DataRilevazione)).ToList();
        }
    }
}
