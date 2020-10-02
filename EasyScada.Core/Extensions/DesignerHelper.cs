using EnvDTE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace EasyScada.Core
{
    public class DesignerHelper
    {
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
            catch { return null; }
            return null;
        }

        public static ConnectionSchema GetDesignConnectionSchema(IServiceProvider context, string fileName = "ConnectionSchema.json")
        {
            try
            {
                string applicationPath = GetApplicationOutputPath(context);
                fileName = applicationPath + fileName;
                if (File.Exists(fileName))
                {
                    string resJson = File.ReadAllText(fileName);
                    return JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                }
            }
            catch { }
            return null;
        }
    }
}
