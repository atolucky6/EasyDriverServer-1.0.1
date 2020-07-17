using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EasyScada.Winforms.Connector;
using Newtonsoft.Json;

namespace EasyScada.Winforms.Designer
{
    public class EasyScadaTagPathConverter : StringConverter
    {
        public EasyScadaTagPathConverter()
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
                    driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);

                }
                else if (File.Exists(releasePath))
                {
                    string resJson = File.ReadAllText(releasePath);
                    driverConnector = JsonConvert.DeserializeObject<ConnectionSchema>(resJson);
                }
                else
                {
                    MessageBox.Show("Could not found the connection schema file", "Easy Driver Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (driverConnector != null)
                    return new StandardValuesCollection(driverConnector.GetAllTagPath().ToList());
                return new StandardValuesCollection(new string[] { });
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
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

        private string GetCurrentDesignPath(ITypeDescriptorContext context)
        {
            try
            {
                EnvDTE.DTE dte = context.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                return Path.GetDirectoryName(dte.ActiveDocument.FullName) + "\\bin";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return string.Empty;
            }
        }
    }
}
