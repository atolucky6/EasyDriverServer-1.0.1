using EasyScada.Core;
using EnvDTE;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    internal static class EasyScadaHelper
    {
        public static string REGEX_ValidFileName = @"^[\w\-. ]+$";
        public const string IpAddressPattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

        /// <summary>
        /// Kiểm tra chuỗi có phải là định dạng địa chỉ Ip
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIpAddress(this string str)
        {
            if (str == "localhost")
                return true;
            return Regex.IsMatch(str, IpAddressPattern);
        }

        /// <summary>
        /// The method to get a <see cref="PropertyDescriptor"/> of the control by property name
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static PropertyDescriptor GetPropertyByName(this object control, [CallerMemberName]string propName = null)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(control)[propName];
            if (null == prop)
                throw new ArgumentException("Matching property not found!", propName);
            else
                return prop;
        }

        /// <summary>
        /// Set the value for property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        public static void SetValue(this object control, object value = null, [CallerMemberName]string propName = null)
        {
            control.GetPropertyByName(propName).SetValue(control, value);
        }

        public static void SetInvoke<T>(this T control, Action<T> setAction)
            where T : Control
        {
            if (control.InvokeRequired)
            {
                MethodInvoker methodInvoker = delegate
                {
                    setAction(control);
                };
                control.Invoke(methodInvoker);
            }
            else
            {
                setAction(control);
            }
        }

        public static async void SetInvokeAsync<T>(this T control, Action<T> setAction)
            where T : Control
        {
            await Task.Run(() =>
            {
                SetInvoke(control, setAction);
            });
        }

        public static Bitmap BitmapColorShade(Bitmap sourceBitmap, Color shadeColor)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                    sourceBitmap.Width, sourceBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {

                int gray = (int)(pixelBuffer[k + 2] * 0.33) + (int)(pixelBuffer[k + 1] * 0.33) + (int)(pixelBuffer[k] * 0.34);

                double rScale = shadeColor.R * 2 / 255;
                double gScale = shadeColor.G * 2 / 255;
                double bScale = shadeColor.B * 2 / 255;

                int r = (int)(gray * rScale);
                int g = (int)(gray * gScale);
                int b = (int)(gray * bScale);

                var rgb = RGB.Scale(new RGB() { R = r, G = g, B = b });

                pixelBuffer[k] = (byte)rgb.B;
                pixelBuffer[k + 1] = (byte)rgb.G;
                pixelBuffer[k + 2] = (byte)rgb.R;
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static void CopySelectedRowToClipboard(this DataGridView gridView)
        {
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            int firstIndex = -1;
            bool needReverse = false;
            foreach (DataGridViewRow row in gridView.SelectedRows)
            {
                if (firstIndex == -1)
                {
                    firstIndex = gridView.Rows.IndexOf(row);
                }
                else
                {
                    if (firstIndex > gridView.Rows.IndexOf(row))
                        needReverse = true;
                }
                selectedRows.Add(row);
            }
            if (needReverse)
                selectedRows.Reverse();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < gridView.ColumnCount; i++)
                sb.Append($"\t{gridView.Columns[i].HeaderText}");
            sb.Append("\r\n");
            for (int i = 0; i < selectedRows.Count; i++)
            {
                if (!selectedRows[i].IsNewRow)
                {
                    for (int j = 0; j < gridView.ColumnCount; j++)
                    {
                        sb.Append($"\t{selectedRows[i].Cells[j].Value?.ToString()}");
                    }
                    if (i != selectedRows.Count - 1)
                        sb.Append("\r\n");
                }
            }
            string copyValue = sb.ToString();
            Clipboard.Clear();
            Clipboard.SetText(copyValue);
        }

        public static bool CanPaste(this DataGridView gridView)
        {
            if (Clipboard.ContainsText())
            {
                string content = Clipboard.GetText();
                if (!string.IsNullOrEmpty(content))
                {
                    string[] lines = content.Split('\n');
                    if (lines.Length > 1)
                    {
                        string[] columns = lines[0].Split('\t');
                        if (columns.Length == gridView.ColumnCount + 1)
                        {
                            bool isMatchColumns = true;
                            for (int i = 0; i < gridView.ColumnCount; i++)
                            {
                                if (columns[i + 1].Replace("\r", "") != gridView.Columns[i].HeaderText)
                                {
                                    isMatchColumns = false;
                                    break;
                                }
                            }
                            if (isMatchColumns)
                            {
                                string line = lines[1]?.Replace("\n", "")?.Replace("\r", "")?.Replace("\t", "");
                                return !string.IsNullOrWhiteSpace(line);
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Trích xuất chuỗi số cuối cùng của chuỗi hoặc số nằm trong dấu ()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="hasValue"></param>
        /// <param name="hasBracketsSurround"></param>
        /// <returns></returns>
        public static uint ExtractLastNumberFromString(this string str, out bool hasValue, out bool hasBracketsSurround)
        {
            var extractStr = Regex.Match(str, @"((\d+)|([(]\d+[)]))$").ToString();
            hasValue = false;
            hasBracketsSurround = false;
            if (!string.IsNullOrEmpty(extractStr))
            {
                hasValue = true;
                if (hasBracketsSurround = extractStr.Contains('('))
                    extractStr = extractStr.Trim(new[] { '(', ')' });
                return uint.Parse(extractStr);
            }
            return 0;
        }

        /// <summary>
        /// Xóa chuỗi số hoặc chuỗi số nằm trong () khỏi chuỗi.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveLastNumberFromString(this string str)
        {
            return Regex.Replace(str, @"((\d+)|([(]\d+[)]))$", "");
        }

        /// <summary>
        /// Tạo 1 tên mới không trùng với bất kì item nào trong group
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="patternName"></param>
        /// <param name="insertBrackets"></param>
        /// <returns></returns>
        public static string GetUniqueNameInCollection(this ICollection<string> collection, string patternName, bool insertBrackets = false)
        {
            uint index = patternName.ExtractLastNumberFromString(out bool hasIndex, out bool hasBracketsSurround);
            index = insertBrackets ? 1 : index;
            if (index == 0)
                index++;
            if (patternName.IsUniqueStringInCollection(collection))
                return patternName;
            patternName = insertBrackets ? hasBracketsSurround ? patternName.RemoveLastNumberFromString() : patternName?.Trim() : patternName.RemoveLastNumberFromString();
            string name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            while (!name.IsUniqueStringInCollection(collection))
            {
                index++;
                name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            }
            return name;
        }

        /// <summary>
        /// Kiểm tra tên có phải là duy nhất trong group không
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="findAllLevel"></param>
        /// <returns></returns>
        public static bool IsUniqueStringInCollection(this string name, ICollection<string> collection)
        {
            return !collection.Contains(name);
        }

        public static string GetUniqueNameInCollection(this ICollection collection, Func<object, string> getNameFunc, string patternName, bool insertBrackets = false)
        {
            uint index = patternName.ExtractLastNumberFromString(out bool hasIndex, out bool hasBracketsSurround);
            index = insertBrackets ? 1 : index;
            if (index == 0)
                index++;
            if (patternName.IsUniqueStringInCollection(collection, getNameFunc))
                return patternName;
            patternName = insertBrackets ? hasBracketsSurround ? patternName.RemoveLastNumberFromString() : patternName?.Trim() : patternName.RemoveLastNumberFromString();
            string name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            while (!name.IsUniqueStringInCollection(collection, getNameFunc))
            {
                index++;
                name = string.Format("{0}{1}", patternName, hasBracketsSurround || insertBrackets ? $"({index})" : $"{index}");
            }
            return name;
        }

        public static bool IsUniqueStringInCollection(this string name, ICollection collection, Func<object, string> getNameFunc)
        {
            foreach (var item in collection)
            {
                if (getNameFunc(item) == name)
                    return false;
            }
            return true;
        }

        public static void Paste(this DataGridView gridView, string colNameHeader = "Name")
        {
            if (Clipboard.ContainsText())
            {
                string content = Clipboard.GetText();
                if (!string.IsNullOrEmpty(content))
                {
                    string[] lines = content.Split('\n');
                    if (lines.Length > 1)
                    {
                        string[] columns = lines[0].Split('\t');
                        if (columns.Length == gridView.ColumnCount + 1)
                        {
                            bool isMatchColumns = true;
                            int colNameIndex = -1;
                            for (int i = 0; i < gridView.ColumnCount; i++)
                            {
                                string colName = columns[i + 1].Replace("\r", "");
                                if (colName == colNameHeader)
                                    colNameIndex = i;
                                if (colName != gridView.Columns[i].HeaderText)
                                {
                                    isMatchColumns = false;
                                    break;
                                }
                            }
                            if (isMatchColumns)
                            {
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    string[] values = lines[i].Split('\t');
                                    if (values.Length == columns.Length)
                                    {
                                        string[] rowValues = new string[values.Length - 1];
                                        for (int j = 0; j < values.Length - 1; j++)
                                        {
                                            string value = values[j + 1].Replace("\r", "");
                                            if (j == colNameIndex)
                                            {
                                                if (string.IsNullOrEmpty(value))
                                                    return;
                                                List<string> nameValues = new List<string>();
                                                foreach (DataGridViewRow row in gridView.Rows)
                                                {
                                                    if (!row.IsNewRow)
                                                    {
                                                        string name = row.Cells[colNameIndex]?.Value?.ToString();
                                                        nameValues.Add(name);
                                                    }
                                                }
                                                value = GetUniqueNameInCollection(nameValues, value);
                                            }
                                            rowValues[j] = value;
                                        }
                                        gridView.Rows.Add(rowValues);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        public static string GetUniqueNameInDataGridView(this DataGridView gridView, string template, out int colNameIndex, string columnName = "Name")
        {
            colNameIndex = -1;
            for (int i = 0; i < gridView.ColumnCount; i++)
            {
                if (gridView.Columns[i].HeaderText == columnName)
                {
                    colNameIndex = i;
                    break;
                }
            }

            if (colNameIndex > -1)
            {
                List<string> nameCollection = new List<string>();
                foreach (DataGridViewRow row in gridView.Rows)
                    nameCollection.Add(row.Cells[colNameIndex]?.Value?.ToString());
                return GetUniqueNameInCollection(nameCollection, template);
            }
            return "";
        }

        public static IEnumerable<string> GetAllTagPath(bool isInDesignMode, out ConnectionSchema connectionSchema)
        {
            connectionSchema = null;
            List<string> result = new List<string>();
            try
            {
                if (isInDesignMode)
                {
                    TagPathConverter tagPathConverter = new TagPathConverter();
                    foreach (var item in tagPathConverter.GetStandardValues())
                    {
                        result.Add(item.ToString());
                    }
                }
                else
                {
                    string applicationDir = Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string debugPath = applicationDir + "\\ConnectionSchema.json";
                    if (File.Exists(debugPath))
                    {
                        string resJson = File.ReadAllText(debugPath);
                        connectionSchema = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                        if (connectionSchema != null)   
                            return connectionSchema.GetAllTagPath();
                    }
                }
            }
            catch { }
            return result;
        }

        public static string GetApplicationOutputPath(IServiceProvider context)
        {
            try
            {
                DTE dte = (DTE)context.GetService(typeof(DTE));

                // Get output path
                // The output folder can have these patterns:
                // 1) "\\server\folder"
                // 2) "drive:\folder"
                // 3) "..\..\folder"
                // 4) "folder"
                var window = dte.ActiveDocument.ActiveWindow;
                if (window != null)
                {
                    if (window.Project != null)
                    {
                        Configuration config = window.Project.ConfigurationManager.ActiveConfiguration;
                        string absoluteOutputPath = null;
                        string projectFolder = null;
                        string outputPath = config.Properties.Item("OutputPath").Value.ToString();
                        if (outputPath.StartsWith($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}"))
                        {
                            // This is the case 1: "\\server\folder"
                            absoluteOutputPath = outputPath;
                        }
                        else if (outputPath.Length >= 2 && outputPath[1] == Path.VolumeSeparatorChar)
                        {
                            // This is the case 2: "drive:\folder"
                            absoluteOutputPath = outputPath;
                        }
                        else if (outputPath.IndexOf(@"..\") != -1)
                        {
                            // This is the case 3: "..\..\folder"
                            projectFolder = Path.GetDirectoryName(window.Project.FullName);
                            while (outputPath.StartsWith(@"..\"))
                            {
                                outputPath = outputPath.Substring(3);
                                projectFolder = Path.Combine(projectFolder, outputPath);
                            }
                            absoluteOutputPath = Path.Combine(projectFolder, outputPath);
                        }
                        else
                        {
                            // This is the case 4: "folder"
                            projectFolder = Path.GetDirectoryName(window.Project.FullName);
                            absoluteOutputPath = Path.Combine(projectFolder, outputPath);
                        }
                        return absoluteOutputPath;
                    }
                }
            }
            catch { }
            return null;
        }

        public static ConnectionSchema GetDesignConnectionSchema(IServiceProvider context, string fileName = "ConnectionSchema.json")
        {
            try
            {
                if (context != null)
                {
                    string applicationPath = GetApplicationOutputPath(context);
                    fileName = applicationPath + fileName;
                    if (File.Exists(fileName))
                    {
                        string resJson = File.ReadAllText(fileName);
                        return JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                    }
                }
                else
                {
                    string applicationPath = "";
                }
            }
            catch { }
            return null;
        }

        public static TreeNode ToTreeNode(this ICoreItem item, bool includeChilds = true, bool includeTags = true)
        {
            if (item == null)
                return null;

            TreeNode node = new TreeNode(item.Name, (int)item.ItemType, (int)item.ItemType);
            node.Tag = item;
            if (item is ICheckable checkable)
                node.Checked = checkable.Checked;
            if (includeChilds && item.Childs != null)
            {
                foreach (ICoreItem child in item.Childs)
                {
                    if (child is ITag)
                    {
                        if (includeTags)
                        {
                            TreeNode childNode = child.ToTreeNode(includeChilds, includeTags);
                            if (childNode != null)
                                node.Nodes.Add(childNode);
                        }
                    }
                    else
                    {
                        TreeNode childNode = child.ToTreeNode(includeChilds, includeTags);
                        if (childNode != null)
                            node.Nodes.Add(childNode);
                    }
                }
            }
            return node;
        }

        public static string ValidateName(this string str, string name)
        {
            str = str?.Trim();
            if (string.IsNullOrEmpty(str))
                return $"The {name} name can't be empty.";
            if (!Regex.IsMatch(str, REGEX_ValidFileName))
                return $"The {name} name was not in correct format.";
            return string.Empty;
        }

        public static string ToHexString(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Color ToColor(this string colorString)
        {
            try
            {
                return ColorTranslator.FromHtml(colorString);
            }
            catch { return Color.Transparent; }
        }

        public static bool ShowSelectTag(this IServiceProvider serviceProvider, out string selectedTag)
        {
            selectedTag = "";
            SelectTagPathDesignerForm form = new SelectTagPathDesignerForm(serviceProvider);
            if (form.ShowDialog() == DialogResult.OK)
            {
                selectedTag = form.SelectedTagPath;
                return true;
            }
            return false;
        }
    }
}
