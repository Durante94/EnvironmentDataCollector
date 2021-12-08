using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebEnvironmentDataCollector.Models
{
    [BsonIgnoreExtraElements]
    public class Log
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime timestamp { get; private set; }
        public string operazione { get; }
        public string documento { get; }

        public Log(string operazione, string documento)
        {
            timestamp = DateTime.Now;
            this.operazione = operazione;
            this.documento = documento;
        }
    }
}
