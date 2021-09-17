using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvironmentDataCollector
{
    static class Program
    {
        private static readonly MongoClient client;
        private static readonly IMongoDatabase db;
        public static readonly string collName = "EnvironmentData";

        static Program()
        {
            client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("test");

            if (!db.ListCollectionNames().ToList().Contains(collName))
            {
                db.CreateCollection(collName, new CreateCollectionOptions
                {
                    TimeSeriesOptions = new TimeSeriesOptions(DataDb.GetTimeSeriesField(), DataDb.metaFieldName)
                });
                db.GetCollection<DataDb>(collName).Indexes.CreateOne(new CreateIndexModel<DataDb>(DataDb.GetIndex()));
            }
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EnvDataForm());
        }
    }
}
