using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public static class CsvHelper
    {
        public static string ToCsvString<T>(this IEnumerable objects)
            where T : class
        {
            CsvBuilder builder = new CsvBuilder();
            Type objectType = typeof(T);
            PropertyInfo[] publicProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            publicProperties = publicProperties.Where(x => !x.PropertyType.IsInterface).ToArray();

            if (publicProperties != null && publicProperties.Length > 0)
            {
                foreach (var propInfo in publicProperties)
                {
                    builder.AddColumn(propInfo.Name);
                }

                foreach (var item in objects)
                {
                    string[] rowData = new string[publicProperties.Length];
                    for (int i = 0; i < rowData.Length; i++)
                    {
                        try
                        {
                            rowData[i] = publicProperties[i].GetValue(item)?.ToString();
                        }
                        catch { }
                    }
                    builder.AddRow(rowData);
                }
            }
            return builder.ToString();
        }

        public static string ToCsvString<T>(this IEnumerable<T> objects)
            where T : class
        {
            CsvBuilder builder = new CsvBuilder();
            Type objectType = typeof(T);
            PropertyInfo[] publicProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            publicProperties = publicProperties.Where(x => !x.PropertyType.IsInterface).ToArray();

            if (publicProperties != null && publicProperties.Length > 0)
            {
                foreach (var propInfo in publicProperties)
                {
                    builder.AddColumn(propInfo.Name);
                }

                foreach (var item in objects)
                {
                    string[] rowData = new string[publicProperties.Length];
                    for (int i = 0; i < rowData.Length; i++)
                    {
                        try
                        {
                            rowData[i] = publicProperties[i].GetValue(item)?.ToString();
                        }
                        catch { }
                    }
                    builder.AddRow(rowData);
                }
            }
            return builder.ToString();
        }

        public static DataTable ToDataTable(string[] lines, char delimiter = ',')
        {
            if (lines == null || lines.Length == 0)
                return null;

            DataTable dt = new DataTable();

            string[] columns = lines[0].Split(delimiter);
            foreach (var col in columns)
                dt.Columns.Add(col);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] rowData = lines[i].Split(delimiter);
                if (rowData.Length == lines.Length)
                {
                    dt.Rows.Add(rowData);
                }
            }
            return dt;
        }

        public static DataTable ToDataTable(string fileName, char delimiter = ',')
        {
            if (!File.Exists(fileName))
                return null;

            string[] lines = File.ReadAllLines(fileName);
            return ToDataTable(lines, delimiter);
        }

        public static List<T> ToList<T>(string[] lines, char delimiter = ',')
            where T : class
        {
            return ToDataTable(lines, delimiter).ToList<T>();
        }

        public static List<T> ToList<T>(string fileName, char delimiter = ',')
            where T : class
        {
            return ToDataTable(fileName, delimiter).ToList<T>();
        }
    }
}
