using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace EnvironmentDataCollector
{
    internal class BaseData
    {
        [Integer]
        public long Position { get; protected set; }

        [BsonTSField]
        public DateTime DataRilevazione { get; protected set; }

        protected BaseData()
        {
            DataRilevazione = new DateTime();
        }
    }

    //CLASSE PER PARSARE IL FILE EXCEL
    internal class DataMap : BaseData
    {
        private string ch2_Unit;

        [BsonIgnore]
        private string Date
        {
            set
            {
                if (DateTime.TryParseExact(value, "yyyy/MM/dd", CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.None, out DateTime temp))
                    DataRilevazione = temp;
            }
        }

        [BsonIgnore]
        private string Time
        {
            set
            {
                string[] splitted = value.Split(':');
                if (splitted.Length > 0)
                {
                    if (int.TryParse(splitted[0], out int num))
                        DataRilevazione = DataRilevazione.AddHours(num);
                    if (splitted.Length > 1 && int.TryParse(splitted[1], out num))
                        DataRilevazione = DataRilevazione.AddMinutes(num);
                    if (splitted.Length > 2 && int.TryParse(splitted[2], out num))
                        DataRilevazione = DataRilevazione.AddSeconds(num);
                }
            }
        }

        [Float, BsonIndex]
        public double Ch1_Value { get; private set; }

        public string Ch1_Unit { get; private set; }

        [Float, BsonIndex]
        public double Ch2_Value { get; private set; }

        public string Ch2_unit
        {
            get
            {
                return ch2_Unit;
            }
            private set
            {
                ch2_Unit = value.Trim().Replace("DEGREE ", "°");
            }
        }

        private DataMap() : base()
        {
        }

        public static DataMap CreateFromExcelRow(string[] header, IRow excelRow)
        {
            DataMap rilevazione = new DataMap();

            for (int i = 0; i < header.Length; i++)
            {
                PropertyInfo current = rilevazione.GetType().GetProperty(header[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                string value = excelRow.GetCell(i)?.ToString() ?? "";

                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    i--;
                    continue;
                }
                else
                    value = value.Trim();
                if (current == null) continue;

                if (current.GetCustomAttribute<FloatAttribute>() != null)
                {
                    if (double.TryParse(value, out double num))
                        current.SetValue(rilevazione, num);
                    else
                        current.SetValue(rilevazione, -200d);
                }
                else if (current.GetCustomAttribute<IntegerAttribute>() != null)
                {
                    if (int.TryParse(value, out int num))
                        current.SetValue(rilevazione, num);
                    else
                        current.SetValue(rilevazione, 0);
                }
                else
                    current.SetValue(rilevazione, value);
            }

            return rilevazione;
        }

        internal DataDb ConvertForDB()
        {
            return new DataDb(this);
        }
    }

    //CLASSE PER SALVARE I DATI SU MONGO
    [BsonIgnoreExtraElements]
    internal class DataDb : BaseData
    {
        public static readonly string metaFieldName = "MetaField";
        //IL VALORE DEL CAMPO STATICO E IL NOME DELLA PROPRIETÀ SOTTOSTANTE DEVO AVERE LO STESSO VALORE
        public Values MetaField { get; private set; }

        public string Ch1_Unit { get; private set; }

        public string Ch2_Unit { get; private set; }

        [BsonConstructor]
        DataDb() : base()
        {
            MetaField = new Values();
        }

        internal DataDb(DataMap other) : this()
        {
            Position = other.Position;
            DataRilevazione = other.DataRilevazione;
            MetaField.Ch1_Value = other.Ch1_Value;
            MetaField.Ch2_Value = other.Ch2_Value;
            Ch1_Unit = other.Ch1_Unit;
            Ch2_Unit = other.Ch2_unit;
        }

        internal static string GetTimeSeriesField()
        {
            foreach (PropertyInfo property in typeof(DataDb).GetProperties())
            {
                if (property.GetCustomAttribute<BsonTSFieldAttribute>() != null)
                    return property.Name;
            }
            return string.Empty;
        }

        internal static IndexKeysDefinition<DataDb> GetIndex()
        {
            IndexKeysDefinition<DataDb> index = Builders<DataDb>.IndexKeys.Combine();
            foreach (PropertyInfo property in typeof(Values).GetProperties())
            {
                if (property.GetCustomAttribute<BsonIndexAttribute>() != null)
                    index = index.Ascending(metaFieldName + "." + property.Name);
            }
            return index;
        }

        internal DataDisplay ConvertToDispaly()
        {
            return new DataDisplay(this);
        }
    }

    internal class Values
    {
        [Float, BsonIndex]
        public double Ch1_Value { get; set; }

        [Float, BsonIndex]
        public double Ch2_Value { get; set; }
    }

    internal class DataDisplay : BaseData
    {
        public string Temperatura { get; private set; }

        public string Umidita { get; private set; }

        internal DataDisplay(DataDb other) : base()
        {
            Position = other.Position;
            DataRilevazione = other.DataRilevazione;
            Umidita = other.MetaField.Ch1_Value.ToString(CultureInfo.GetCultureInfo("it-IT")) + other.Ch1_Unit;
            Temperatura = other.MetaField.Ch2_Value.ToString(CultureInfo.GetCultureInfo("it-IT")) + other.Ch2_Unit;
        }

        internal static DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo property in typeof(DataDisplay).GetProperties())
            {
                dt.Columns.Add(property.Name, property.PropertyType);
            }

            return dt;
        }

        internal DataRow FillDataTable(DataRow populate)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                populate[property.Name] = property.GetValue(this);
            }
            return populate;
        }
    }

    internal class BsonTSFieldAttribute : Attribute
    {
    }

    internal class BsonIndexAttribute : Attribute
    {
    }

    internal class FloatAttribute : Attribute
    {
    }

    internal class IntegerAttribute : Attribute
    {
    }
}
