using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public class CsvBuilder
    {
        public List<string> Columns { get; private set; }
        public List<string[]> Rows { get; private set; }
        public string Delimeter { get; set; }

        public CsvBuilder()
        {
            Columns = new List<string>();
            Rows = new List<string[]>();
            Delimeter = ",";
        }

        public CsvBuilder AddColumn(string colName)
        {
            if (!Columns.Contains(colName))
                Columns.Add(colName);
            return this;
        }

        public CsvBuilder AddRow(params string[] value)
        {
            Rows.Add(value);
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            // Append columns
            for (int i = 0; i < Columns.Count; i++)
            {
                sb.Append(Columns[i]);
                if (i < Columns.Count - 1)
                    sb.Append(Delimeter);
            }
            // Append rows
            for (int i = 0; i < Rows.Count; i++)
            {
                sb.Append(Environment.NewLine);
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (j < Rows[i].Length)
                        sb.Append(Rows[i][j]);
                    if (j < Columns.Count - 1)
                        sb.Append(Delimeter);
                }
            }
            return sb.ToString();
        }
    }
}
