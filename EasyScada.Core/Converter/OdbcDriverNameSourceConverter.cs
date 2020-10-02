using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyScada.Core
{
    public class OdbcDriverNameSourceConverter : StringConverter
    {
        private static StandardValuesCollection collection;

        static OdbcDriverNameSourceConverter()
        {
            collection = new StandardValuesCollection(GetSystemDriverList());
        }

        public static List<string> GetSystemDriverList()
        {
            List<string> names = new List<string>();
            // get system dsn's
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBCINST.INI");
                    if (reg != null)
                    {

                        reg = reg.OpenSubKey("ODBC Drivers");
                        if (reg != null)
                        {
                            // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                            foreach (string sName in reg.GetValueNames())
                            {
                                names.Add(sName);
                            }
                        }
                        try
                        {
                            reg.Close();
                        }
                        catch { }
                    }
                }
            }
            return names;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                if (collection == null)
                    collection = new StandardValuesCollection(GetSystemDriverList());
                return collection;
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
    }
}
