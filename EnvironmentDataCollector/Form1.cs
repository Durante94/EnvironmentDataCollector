using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvironmentDataCollector
{
    public partial class EnvDataForm : System.Windows.Forms.Form
    {
        private List<DataDisplay> toDispaly;
        public EnvDataForm()
        {
            InitializeComponent();
            toDispaly = null;
        }

        //MOSTRO DIALOG PER CARICARE IL FILE EXCEL
        private void FileBtn_Click(object sender, EventArgs e)
        {
            FileDialog.ShowDialog();
        }

        //FUNZIONE PER REPERIRE I FILTRI
        private BsonDocument GetFilters
        {
            get
            {
                BsonDocument filter = new BsonDocument(true);

                if (FromPiker.Checked)
                    filter.Add(DataDb.GetTimeSeriesField(), new BsonDocument("$gte", FromPiker.Value));

                if (ToPicker.Checked)
                    filter.Add(DataDb.GetTimeSeriesField(), new BsonDocument("$lte", ToPicker.Value));

                if (false)//filtro $gte umidità
                    filter.Add(DataDb.metaFieldName + "Ch1_Value", new BsonDocument("$gte", double.MinValue));

                if (false)//filtro $lte umidità
                    filter.Add(DataDb.metaFieldName + "Ch1_Value", new BsonDocument("$lte", double.MaxValue));

                if (false)//filtro $gte temperatura
                    filter.Add(DataDb.metaFieldName + "Ch2_Value", new BsonDocument("$gte", double.MinValue));

                if (false)//filtro $lte temperatura
                    filter.Add(DataDb.metaFieldName + "Ch2_Value", new BsonDocument("$lte", double.MaxValue));

                return filter;
            }
        }

        //RICERCA DATI PER FILTRO
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (toDispaly == null)
                toDispaly = Program.GetData(GetFilters).ConvertAll(doc => doc.ConvertToDispaly());
            DataGrid.DataSource = toDispaly;
            DataGrid.Refresh();
        }

        //ESPORTA DATI RICERCA
        private void ExportBtn_Click(object sender, EventArgs e)
        {
            if (toDispaly == null)
                toDispaly = Program.GetData(GetFilters).ConvertAll(doc => doc.ConvertToDispaly());

            //Salva Excel
        }

        //UPDATE DB DA FILE CARICATO
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //TASK PER NON BLOCCARE IL PROGRAMMA
            HSSFWorkbook hssfwb = new HSSFWorkbook();
            ISheet sheet = hssfwb.GetSheetAt(0);

            IRow row = sheet.GetRow(sheet.FirstRowNum);
            string[] header = new string[row.PhysicalNumberOfCells];

            int pos = 0;
            for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
            {
                string cellCont = row.GetCell(i)?.ToString() ?? "";
                if (!string.IsNullOrEmpty(cellCont) && !string.IsNullOrWhiteSpace(cellCont))
                    header[pos++] = cellCont;
            }

            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                DataMap parsed = DataMap.CreateFromExcelRow(header, sheet.GetRow(i));
                Program.SaveData(parsed.ConvertForDB());
            }
        }
    }
}
