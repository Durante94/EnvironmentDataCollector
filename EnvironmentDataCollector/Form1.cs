using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
            FileDialog.Filter = "Excel File (*.xls)|*.xls|Excel File (*.xlsx)|*.xlsx|CSV File (*.csv)|*.csv";
        }

        //MOSTRO DIALOG PER CARICARE IL FILE EXCEL
        private void FileBtn_Click(object sender, EventArgs e)
        {
            if (FileDialog.ShowDialog() != DialogResult.OK) return;

            //UPDATE DB DA FILE CARICATO
            FileStream excelFile = new FileStream(FileDialog.FileName, FileMode.Open, FileAccess.Read)
            {
                Position = 0
            };
            //TASK PER NON BLOCCARE IL PROGRAMMA

            int pos = FileDialog.FileName.LastIndexOf('.');
            if (pos < 0)
            {
                MessageBox.Show("File non leggibile!", "Attenzione", MessageBoxButtons.OK);
                return;
            }

            string extension = FileDialog.FileName[(pos + 1)..];

            if (string.Compare("xlsx", extension, true) == 0)
            {
                XSSFWorkbook xssfwb = new XSSFWorkbook(excelFile);
                ExcelFileHandler(xssfwb.GetSheetAt(0));
            }
            else if (string.Compare("xls", extension, true) == 0)
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook();
                try
                {
                    hssfwb = new HSSFWorkbook(excelFile);
                    ExcelFileHandler(hssfwb.GetSheetAt(0));
                }
                catch (NotOLE2FileException)
                {
                    CSVFileHandler(new StreamReader(FileDialog.FileName));
                }
            }
            else if (string.Compare("csv", extension, true) == 0)
                CSVFileHandler(new StreamReader(excelFile));
            else
                MessageBox.Show("Tipo file non riconosciuto!", "Attenzione", MessageBoxButtons.OK);

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

                if (InputMinHum.Validate())//filtro $gte umidità
                    filter.Add(DataDb.metaFieldName + ".Ch1_Value", new BsonDocument("$gte", InputMinHum.Value));

                if (InputMaxHum.Validate() && InputMaxHum.Value > InputMinHum.Value)//filtro $lte umidità
                    filter.Add(DataDb.metaFieldName + ".Ch1_Value", new BsonDocument("$lte", InputMaxHum.Value));

                if (InputMinTemp.Validate())//filtro $gte temperatura
                    filter.Add(DataDb.metaFieldName + ".Ch2_Value", new BsonDocument("$gte", InputMinTemp.Value));

                if (InputMaxTemp.Validate() && InputMaxTemp.Value > InputMinTemp.Value)//filtro $lte temperatura
                    filter.Add(DataDb.metaFieldName + ".Ch2_Value", new BsonDocument("$lte", InputMaxTemp.Value));

                return filter;
            }
        }

        private void CleanFilters(object sender, EventArgs e)
        {
            toDispaly = null;
        }

        //RICERCA DATI PER FILTRO
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (toDispaly == null)
                toDispaly = Program.GetData(GetFilters).ConvertAll(doc => doc.ConvertToDispaly());

            DataTable dataTable = DataDisplay.CreateDataTable();
            foreach (DataDisplay disp in toDispaly)
            {
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(disp.FillDataTable(row));
            }

            DataGrid.DataSource = dataTable;
            DataGrid.Refresh();
        }

        //ESPORTA DATI RICERCA
        private void ExportBtn_Click(object sender, EventArgs e)
        {
            SearchBtn_Click(sender, e);

            //chiedere posizione salvataggio
            XSSFWorkbook xssfwb = new XSSFWorkbook();
            ISheet sheet = xssfwb.CreateSheet("Rilevazioni");

            xssfwb.Add(sheet);

            int i = 0, j = 0;
            IRow row = sheet.CreateRow(i++);
            DataTable fromDataGrid = (DataTable)DataGrid.DataSource;

            foreach (DataColumn column in fromDataGrid.Columns)
            {
                ICell cell = row.CreateCell(j++);
                cell.SetCellValue(column.ColumnName);
            }

            foreach (DataRow item in fromDataGrid.Rows)
            {
                j = 0;
                row = sheet.CreateRow(i++);
                foreach (DataColumn column in fromDataGrid.Columns)
                {
                    ICell cell = row.CreateCell(j++);
                    cell.SetCellValue(item[column].ToString() ?? "");
                }
            }
        }

        private void ExcelFileHandler(ISheet sheet)
        {
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

        private void CSVFileHandler(StreamReader sr)
        {
            string csvLine = sr.ReadLine();
            string[] header = csvLine.Split('\t');

            while (!sr.EndOfStream)
            {
                csvLine = sr.ReadLine();
                DataMap parsed = DataMap.CreateFromCSVRow(header, csvLine.Split('\t'));
                Program.SaveData(parsed.ConvertForDB());
            }
        }
    }
}
