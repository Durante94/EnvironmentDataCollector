﻿using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.HSSF.UserModel;
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
            if (toDispaly == null)
                toDispaly = Program.GetData(GetFilters).ConvertAll(doc => doc.ConvertToDispaly());

            //Salva Excel
        }

        //UPDATE DB DA FILE CARICATO
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
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
                HSSFWorkbook hssfwb = new HSSFWorkbook(excelFile);
                ExcelFileHandler(hssfwb.GetSheetAt(0));
            }
            else if (string.Compare("csv", extension, true) == 0)
                CSVFileHandler(new StreamReader(excelFile));
            else
                MessageBox.Show("Tipo file non riconosciuto!", "Attenzione", MessageBoxButtons.OK);
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
            string[] header = csvLine.Split(';');

            while (!sr.EndOfStream)
            {
                csvLine = sr.ReadLine();
                DataMap parsed = DataMap.CreateFromCSVRow(header, csvLine.Split(';'));
                Program.SaveData(parsed.ConvertForDB());
            }
        }
    }
}
