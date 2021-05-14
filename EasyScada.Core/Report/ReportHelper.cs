using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public static class ReportHelper
    {
        public static bool ExportToCsv(DataTable data, string path, string delimeter = ",")
        {
            CsvBuilder builder = new CsvBuilder();
            builder.Delimeter = delimeter;
            foreach (DataColumn col in data.Columns)
            {
                builder.AddColumn(col.ColumnName);
            }
            foreach (DataRow row in data.Rows)
            {
                string[] rowValues = new string[data.Columns.Count];
                for (int i = 0; i < rowValues.Length; i++)
                    rowValues[i] = row[i]?.ToString();
                builder.AddRow(rowValues);
            }
            File.WriteAllText(path, builder.ToString());
            return true;
        }

        public static bool ExportToXlsx(DataTable data, string path, string mainTitle = null, string secondTitle = null)
        {
            // Create workbook
            using (XLWorkbook wb = new XLWorkbook())
            {
                // Create sheet in workbook
                IXLWorksheet ws = wb.Worksheets.Add("Sheet1");

                int currentRow = 1;               
                if (!string.IsNullOrEmpty(mainTitle))
                {
                    ws.Cell(currentRow, 1).Value = mainTitle;
                    ws.Cell(currentRow, 1).Style.Font.Bold = true;
                    ws.Range(currentRow, 1, currentRow, data.Columns.Count).Merge();
                    ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;
                }

                if (!string.IsNullOrEmpty(secondTitle))
                {
                    ws.Cell(currentRow, 1).Value = secondTitle;
                    ws.Cell(currentRow, 1).Style.Font.Italic = true;
                    ws.Range(currentRow, 1, currentRow, data.Columns.Count).Merge();
                    ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;
                }

                ws.Cell(currentRow, 1).InsertTable(data.AsEnumerable());
                wb.SaveAs(path);
            }
            return false;
        }
    }
}
