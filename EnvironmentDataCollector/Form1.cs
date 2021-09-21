using MongoDB.Bson;
using MongoDB.Driver;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Charts;
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
        private List<DataDb> toDispaly;
        private Task update, export;

        public EnvDataForm()
        {
            InitializeComponent();
            toDispaly = null;
            FileDialog.Filter = "Excel File (*.xls)|*.xls|Excel File (*.xlsx)|*.xlsx|CSV File (*.csv)|*.csv";
        }

        private void WriteTextSafe(string text)
        {
            if (labelExport.InvokeRequired)
            {
                Action safeWrite = delegate { WriteTextSafe(text); };
                labelExport.Invoke(safeWrite);
            }
            else
                labelExport.Text = text;
        }

        private void HideLabel(Label label)
        {
            if (label.InvokeRequired)
            {
                Action safeHide = delegate { HideLabel(label); };
                label.Invoke(safeHide);
            }
            else
                label.Visible = false;
        }

        //MOSTRO DIALOG PER CARICARE IL FILE EXCEL
        private void FileBtn_Click(object sender, EventArgs e)
        {
            if (FileDialog.ShowDialog() != DialogResult.OK) return;

            if (update != null && !update.IsCompleted)
            {
                MessageBox.Show("Il precedente aggiornamento non è stato completato.\r\nAttendi", "Attenzione", MessageBoxButtons.OK);
                return;
            }
            labelUpload.Visible = true;

            //UPDATE DB DA FILE CARICATO
            update = Task.Run(() =>
            {
                FileStream excelFile = null;

                try
                {
                    excelFile = new FileStream(FileDialog.FileName, FileMode.Open, FileAccess.Read)
                    {
                        Position = 0
                    };
                }
                catch (IOException)
                {
                    MessageBox.Show("File aperto da un 'altro processo.\r\nChiudere il file e riprovare!", "Errore", MessageBoxButtons.OK);
                    return;
                }
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
                HideLabel(labelUpload);
            });
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

        private void Search()
        {
            if (toDispaly == null)
                toDispaly = Program.GetData(GetFilters);
        }

        //RICERCA DATI PER FILTRO
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            Search();

            DataTable dataTable = DataDb.CreateDataTable();
            foreach (DataDb disp in toDispaly)
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
            labelExport.Text = "Ricerca dati...";
            labelExport.Visible = true;

            if (export != null && !export.IsCompleted)
            {
                MessageBox.Show("La precedente exportazione non è stata completata.\r\nAttendi", "Attenzione", MessageBoxButtons.OK);
                return;
            }

            export = Task.Run(() =>
            {
                Search();

                WriteTextSafe("Creazione file...");
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "All files (*.*)|*.*|Excel File (*.xlsx)|*.xlsx",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    HideLabel(labelExport);
                    return;
                }

                Stream toSave;
                try
                {
                    toSave = saveFileDialog.OpenFile();
                }
                catch (IOException)
                {
                    MessageBox.Show("Impossibile sovrascrivere.\r\nFile aperto da un 'altro processo.\r\nChiudere il file e riprovare!", "Errore", MessageBoxButtons.OK);
                    HideLabel(labelExport);
                    return;
                }

                if (toSave == null)
                {
                    HideLabel(labelExport);
                    return;
                }

                WriteTextSafe("Inserimento dati...");
                XSSFWorkbook xssfwb = new XSSFWorkbook();
                ISheet dataSheet = xssfwb.CreateSheet("Rilevazioni");

                xssfwb.Add(dataSheet);

                int i = 0, j = 0;
                IRow row = dataSheet.CreateRow(i++);
                DataTable fromDataGrid = DataDb.CreateDataTable();

                foreach (DataColumn column in fromDataGrid.Columns)
                {
                    ICell cell = row.CreateCell(j++);
                    cell.SetCellValue(column.ColumnName);
                }

                foreach (DataRow item in toDispaly.ConvertAll(x => x.FillDataTable(fromDataGrid.NewRow()))/*fromDataGrid.Rows*/)
                {
                    j = 0;
                    row = dataSheet.CreateRow(i++);
                    foreach (DataColumn column in fromDataGrid.Columns)
                    {
                        ICell cell = row.CreateCell(j++);
                        string currentType = column.DataType.ToString();
                        switch (currentType)
                        {
                            case "System.DateTime":
                                cell.SetCellValue(((DateTime)item[column]).ToString("dd/MM/yyyy HH:mm:ss"));
                                break;
                            case "System.String":
                                cell.SetCellValue(item[column].ToString() ?? "");
                                break;
                            case "System.Int64":
                                cell.SetCellValue((long)item[column]);
                                break;
                            default:
                                break;
                        }
                    }
                }

                j = 0;
                foreach (DataColumn column in fromDataGrid.Columns)
                {
                    dataSheet.AutoSizeColumn(j++);
                }

                WriteTextSafe("Creazione grafici...");

                ISheet graphSheet = xssfwb.CreateSheet("Grafici");

                //GRAFICO TEMPERATURA
                XSSFDrawing drawing = (XSSFDrawing)graphSheet.CreateDrawingPatriarch();
                XSSFClientAnchor anchor = (XSSFClientAnchor)drawing.CreateAnchor(0, 0, 0, 0, 1, 2, 20, 20);
                XSSFChart chart = (XSSFChart)drawing.CreateChart(anchor);
                ILineChartData<DateTime, double> lineChart = chart.ChartDataFactory.CreateLineChartData<DateTime, double>();
                IChartLegend legend = chart.GetOrCreateLegend();
                legend.Position = LegendPosition.TopRight;

                IChartAxis ascissa = chart.ChartAxisFactory.CreateDateAxis(AxisPosition.Bottom);
                ascissa.MajorTickMark = AxisTickMark.None;
                IValueAxis ordinata = chart.ChartAxisFactory.CreateValueAxis(AxisPosition.Left);
                ordinata.Crosses = AxisCrosses.AutoZero;
                ordinata.SetCrossBetween(AxisCrossBetween.Between);

                IChartDataSource<DateTime> xValues = DataSources.FromArray(toDispaly.ConvertAll(x => x.DataRilevazione).ToArray());
                IChartDataSource<double> yValues = DataSources.FromArray(toDispaly.ConvertAll(x => x.MetaField.Ch2_Value).ToArray());
                ILineChartSeries<DateTime, double> serie = lineChart.AddSeries(xValues, yValues);
                serie.SetTitle("Temperatura");

                chart.Plot(lineChart, ascissa, ordinata);

                //GRAFICO UMIDITÀ
                drawing = (XSSFDrawing)graphSheet.CreateDrawingPatriarch();
                anchor = (XSSFClientAnchor)drawing.CreateAnchor(0, 0, 0, 0, 1, 22, 20, 42);
                chart = (XSSFChart)drawing.CreateChart(anchor);
                lineChart = chart.ChartDataFactory.CreateLineChartData<DateTime, double>();
                legend = chart.GetOrCreateLegend();
                legend.Position = LegendPosition.TopRight;

                ascissa = chart.ChartAxisFactory.CreateDateAxis(AxisPosition.Bottom);
                ascissa.MajorTickMark = AxisTickMark.None;
                ordinata = chart.ChartAxisFactory.CreateValueAxis(AxisPosition.Left);
                ordinata.Crosses = AxisCrosses.AutoZero;
                ordinata.SetCrossBetween(AxisCrossBetween.Between);

                xValues = DataSources.FromArray(toDispaly.ConvertAll(x => x.DataRilevazione).ToArray());
                yValues = DataSources.FromArray(toDispaly.ConvertAll(x => x.MetaField.Ch1_Value).ToArray());
                serie = lineChart.AddSeries(xValues, yValues);
                serie.SetTitle("Umidità");

                chart.Plot(lineChart, ascissa, ordinata);

                xssfwb.Write(toSave, false);
                xssfwb.Close();

                HideLabel(labelExport);
            });
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
