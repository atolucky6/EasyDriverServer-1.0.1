using EnvDTE;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace EasyScada.Core
{
    public class TagPathConverter : StringConverter
    {
        static TagPathConverter()
        {

        }

        public TagPathConverter()
        {
            
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                string debugPath = GetCurrentDesignPath(context) + "\\Debug\\ConnectionSchema.json";
                string releasePath = GetCurrentDesignPath(context) + "\\Release\\ConnectionSchema.json";

                ConnectionSchema driverConnector = null;
                if (File.Exists(debugPath))
                {
                    string resJson = File.ReadAllText(debugPath);
                    driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson, new ConnectionSchemaJsonConverter());

                }
                else if (File.Exists(releasePath))
                {
                    string resJson = File.ReadAllText(releasePath);
                    driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson, new ConnectionSchemaJsonConverter());
                }

                if (driverConnector != null)
                    return new StandardValuesCollection(driverConnector.GetAllTagPath().ToList());

                return new StandardValuesCollection(new string[] { });
            }
            catch (Exception)
            {
                return new StandardValuesCollection(new string[] { });
            }
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }


        public static object test;

        private string GetCurrentDesignPath(IServiceProvider context)
        {
            try
            {
     
                DTE dte = (DTE)context.GetService(typeof(DTE));
                if (dte.ActiveDocument != null)
                {
                    // Get output path
                    //var window = dte.ActiveDocument.ActiveWindow;
                    //if (window != null)
                    //{
                    //    if (window.Project != null)
                    //    {
                    //        Configuration config = window.Project.ConfigurationManager.ActiveConfiguration;
                    //        string outputPath = config.Properties.Item("OutputPath").Value.ToString();
                    //        MessageBox.Show(outputPath);
                    //        MessageBox.Show(window.Project.FullName);
                    //        //if (outputPath.StartsWith("\\" + "\\"))
                    //    }
                    //}

                    //if (dte.ActiveDocument.ProjectItem != null)
                    //{
                    //    FileCodeModel model = (FileCodeModel)dte.ActiveDocument.ProjectItem.FileCodeModel;
                    //    foreach (CodeElement codeElement in model.CodeElements)
                    //    {
                    //        if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                    //        {
                    //            foreach (CodeElement child in codeElement.Children)
                    //            {
                    //                if (child is CodeClass codeClass)
                    //                {
                    //                    foreach (var item in child.Children)
                    //                    {
                    //                        if (item is CodeProperty prop)
                    //                        {
                    //                            if (prop.Name == "Tag1")
                    //                                codeClass.RemoveMember(item);

                    //                        }
                    //                    }
                    //                    var edit = (EditPoint)codeClass.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                    //                    edit.Insert(Environment.NewLine);
                    //                    edit.Indent(null, 2);
                    //                    edit.Insert("public EasyScada.Core.ITag Tag1 { get; set; }" + Environment.NewLine);

                    //                }
                    //            }
                                
                    //        }
                    //    }
                    //}

                    // Get Code
                    //TextDocument currentDocument = (TextDocument)dte.ActiveDocument.Object("TextDocument");
                    //if (currentDocument != null)
                    //{
                    //    EnvDTE.EditPoint editPoint = currentDocument.StartPoint.CreateEditPoint();
                    //    string res = editPoint.GetText(currentDocument.EndPoint);
                    //    MessageBox.Show(res);
                    //}
                    return Path.GetDirectoryName(dte.ActiveDocument.FullName) + "\\bin";
                }
                //MessageBox.Show(dte.ActiveDocument.Path);
                return "";
            }
            catch (Exception) {
                return string.Empty; }
        }
    }
}
