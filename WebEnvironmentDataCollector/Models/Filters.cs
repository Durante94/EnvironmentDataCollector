using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebEnvironmentDataCollector.Models
{
    public class Filters
    {
        private bool date_from_flag, date_to_flag, temp_from_flag, temp_to_flag, humid_from_flag, humid_to_flag;

        private DateTime date_From, date_To;

        private double temp_From, temp_To, humid_From, humid_To;

        public DateTime Date_From
        {
            set
            {
                date_from_flag = true;
                date_From = value;
            }
        }

        [To]
        public DateTime Date_To
        {
            set
            {
                date_to_flag = true;
                date_To = value;
            }
        }

        public double Temp_From
        {
            set
            {
                temp_from_flag = true;
                temp_From = value;
            }
        }

        [To]
        public double Temp_To
        {
            set
            {
                temp_to_flag = true;
                temp_To = value;
            }
        }

        public double Humid_From
        {
            set
            {
                humid_from_flag = true;
                humid_From = value;
            }
        }

        [To]
        public double Humid_To
        {
            set
            {
                humid_to_flag = true;
                humid_To = value;
            }
        }

        [JsonConstructor]
        public Filters()
        {
            date_From = new DateTime();
            date_To = new DateTime();

            date_from_flag = false;
            date_to_flag = false;
            temp_from_flag = false;
            temp_to_flag = false;
            humid_from_flag = false;
            humid_to_flag = false;
        }

        public BsonDocument GetFiletrs()
        {
            BsonDocument filter = new BsonDocument(true);

            if (date_from_flag)
                filter.Add(DataDb.GetTimeSeriesField(), new BsonDocument("$gte", date_From));

            if (date_to_flag)
                filter.Add(DataDb.GetTimeSeriesField(), new BsonDocument("$lte", date_To.AddDays(1)));

            if (humid_from_flag)//filtro $gte umidità
                filter.Add(DataDb.metaFieldName + ".Ch1_Value", new BsonDocument("$gte", humid_From));

            if (humid_to_flag)//filtro $lte umidità
                filter.Add(DataDb.metaFieldName + ".Ch1_Value", new BsonDocument("$lte", humid_To));

            if (temp_from_flag)//filtro $gte temperatura
                filter.Add(DataDb.metaFieldName + ".Ch2_Value", new BsonDocument("$gte", temp_From));

            if (temp_to_flag)//filtro $lte temperatura
                filter.Add(DataDb.metaFieldName + ".Ch2_Value", new BsonDocument("$lte", temp_To));

            return filter;
        }
    }

    internal class ToAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Filters instance = (Filters)validationContext.ObjectInstance;
            string property = validationContext.MemberName.Split('_')[0];

            System.Reflection.FieldInfo field = validationContext.ObjectType.GetField($"{property.ToLowerInvariant()}_From");
            object from = field.GetValue(instance);

            if (from == null) return new ValidationResult("Errore ordine serializzazione");

            if (field.FieldType == typeof(DateTime))
            {
                if ((DateTime)from < (DateTime)value)
                    return new ValidationResult("La Data di fine ricerca non può essere prima di quella di inizio");
                else
                    return ValidationResult.Success;
            }
            else
            {
                if ((double)from < (double)value)
                    return new ValidationResult($"La {(string.Compare(property, "Humid") == 0 ? "Umidit&aacute;" : "Temperatura")} di fine ricerca non può essere prima di quella di inizio");
                else
                    return ValidationResult.Success;
            }
        }
    }
}
