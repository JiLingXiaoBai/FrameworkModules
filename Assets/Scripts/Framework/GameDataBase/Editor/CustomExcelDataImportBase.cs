using System.Collections.Generic;
using OfficeOpenXml;

namespace JLXB.Framework.GameDataBase.Editor
{
    public abstract class CustomExcelDataImportBase
    {
        protected string OutPath;
        protected List<string> Keys;

        protected CustomExcelDataImportBase(string outPath)
        {
            OutPath = outPath;
        }

        public void SetKey(List<string> keys)
        {
            Keys = keys;
        }

        public abstract void ImportExcel(string excelName, ExcelWorksheet sheet);
    }
}