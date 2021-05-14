using EasyScada.Core;
using EnvDTE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public class RoleConverter : StringConverter
    {
        public RoleConverter()
        {

        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                string debugPath = GetCurrentDesignPath(context) + "\\Debug\\Roles.json";
                string releasePath = GetCurrentDesignPath(context) + "\\Release\\Roles.json";

                List<Role> roles = null;
                if (File.Exists(debugPath))
                {
                    string resJson = File.ReadAllText(debugPath);
                    roles = JsonConvert.DeserializeObject<List<Role>>(resJson);

                }
                else if (File.Exists(releasePath))
                {
                    string resJson = File.ReadAllText(releasePath);
                    roles = JsonConvert.DeserializeObject<List<Role>>(resJson);
                }

                if (roles != null && roles.Count > 0)
                    return new StandardValuesCollection(roles.Select(x => x.Name).ToArray());

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

        private string GetCurrentDesignPath(IServiceProvider context)
        {
            try
            {

                DTE dte = (DTE)context.GetService(typeof(DTE));
                if (dte.ActiveDocument != null)
                {
                    return Path.GetDirectoryName(dte.ActiveDocument.FullName) + "\\bin";
                }
                return "";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
