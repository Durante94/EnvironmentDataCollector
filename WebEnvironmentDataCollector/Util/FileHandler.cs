using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WebEnvironmentDataCollector.Models;

namespace WebEnvironmentDataCollector.Util
{
    public class FileHandler
    {
        public FileHandler() { }

        internal bool SetFile(Microsoft.AspNetCore.Http.IFormFile file, MongoHandler mongo, string userName)
        //RITORNO TRUE IN CASO DI PROBLEMI
        {
            int pos = file.FileName.LastIndexOf('.');

            if (pos < 0) return true;

            string extension = file.FileName[(pos + 1)..];

            if (string.Compare("xlsx", extension, true) == 0)
            {
                XSSFWorkbook xssfwb = new XSSFWorkbook(file.OpenReadStream());
                ExcelFileHandler(xssfwb.GetSheetAt(0), mongo, userName);
            }
            else if (string.Compare("xls", extension, true) == 0)
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook();
                try
                {
                    hssfwb = new HSSFWorkbook(file.OpenReadStream());
                    ExcelFileHandler(hssfwb.GetSheetAt(0), mongo, userName);
                }
                catch (NPOI.POIFS.FileSystem.NotOLE2FileException)
                {
                    CSVFileHandler(new StreamReader(file.OpenReadStream()), mongo, userName);
                }
            }
            else if (string.Compare("csv", extension, true) == 0)
                CSVFileHandler(new StreamReader(file.OpenReadStream()), mongo, userName);
            else
                return true;

            return false;
        }

        private void ExcelFileHandler(ISheet sheet, MongoHandler mongo, string userName)
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
                mongo.SaveData(parsed.ConvertForDB(), userName);
            }
        }

        private void CSVFileHandler(StreamReader sr, MongoHandler mongo, string userName)
        {
            string csvLine = sr.ReadLine();
            string[] header = csvLine.Split('\t');

            while (!sr.EndOfStream)
            {
                csvLine = sr.ReadLine();
                DataMap parsed = DataMap.CreateFromCSVRow(header, csvLine.Split('\t'));
                mongo.SaveData(parsed.ConvertForDB(), userName);
            }
        }
    }
}
