using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.License
{
    public static class LicenseManager
    {
        public static string GetComputerId()
        {
            return new DeviceIdBuilder()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .UseFormatter(new HashDeviceIdFormatter(() => SHA256.Create(), new Base64UrlByteArrayEncoder()))
                .ToString();
        }

        public static async Task<int> AuthenticateAsync(string url, string product, string serialKey, string computerId = null)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string result = await client.GetStringAsync(url);
                    if (int.TryParse(result, out int limitTagCount))
                    {
                        if (limitTagCount > 0)
                        {
                            RegistryKey key = null;
                            key = Registry.CurrentUser.CreateSubKey(@"Software\EasyScada", RegistryKeyPermissionCheck.ReadWriteSubTree);
                            key = Registry.CurrentUser.OpenSubKey(@"Software\EasyScada", RegistryKeyPermissionCheck.ReadWriteSubTree);
                            if (key != null)
                            {
                                key.SetValue(product, serialKey);
                                key.Close();
                            }
                            return limitTagCount;
                        }
                    }
                    return 0;
                }
            }
            catch
            {
                return IsAuthenticated(product, serialKey);
            }
        }

        public static int IsAuthenticated(string product, string serialKey)
        {
            try
            {
                if (GetStoredSerialKey(product) == serialKey)
                    return int.MaxValue;
                return 0;
            }
            catch { return 0; }
        }

        public static string GetStoredSerialKey(string product)
        {
            try
            {
                RegistryKey key = null;
                key = Registry.CurrentUser.CreateSubKey(@"Software\EasyScada", RegistryKeyPermissionCheck.ReadWriteSubTree);
                key = Registry.CurrentUser.OpenSubKey(@"Software\EasyScada", RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key != null)
                {
                    if (key.GetValueNames().Contains(product))
                    {
                        string serialKey = key.GetValue(product)?.ToString();
                        return serialKey;
                    }
                    key.Close();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void SetSerialKey(string product, string serialKey)
        {
            try
            {
                RegistryKey key = null;
                if (Registry.LocalMachine.GetSubKeyNames().Contains(@"SOFTWARE\EASYSCADA"))
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EASYSCADA");
                else
                    key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\EASYSCADA");

                if (key != null)
                {
                    key.SetValue(product, serialKey);
                }
            }
            catch {  }
        }
    }

}
