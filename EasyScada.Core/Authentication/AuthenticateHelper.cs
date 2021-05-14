using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyScada.Core
{
    public static class AuthenticateHelper
    {
        static bool isFirstInit = false;

        public static string CurrentUser { get; internal set; }
        public static string CurrentRole { get; internal set; }
        public static List<Role> RolesSource { get; internal set; }

        public static bool Login(string userName, string password, out string error)
        {
            error = string.Empty;
            try
            {
                LogProfile profile = GetDbProfile(null);
                if (profile != null)
                {

                    using (MySqlConnection conn = new MySqlConnection(profile.GetConnectionString(true)))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            cmd.CommandText = $"select count(*) from {profile.TableName}";

                            int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                            if (rowCount == 0)
                                return true;

                            if (string.IsNullOrWhiteSpace(userName))
                            {
                                error = "Username is required.";
                                return false;
                            }

                            if (string.IsNullOrWhiteSpace(password))
                            {
                                error = "Password is required.";
                                return false;
                            }

                            cmd.CommandText = $"select * from {profile.TableName} where Username = '{userName}' and Password = '{password}'";
                            using (MySqlDataAdapter adp = new MySqlDataAdapter(cmd))
                            {
                                DataTable table = new DataTable();
                                adp.Fill(table);
                                if (table.Rows.Count > 0)
                                {
                                    string newRole = table.Rows[0]["Role"].ToString();
                                    string newUser = table.Rows[0]["Username"].ToString();
                                    if (newRole != CurrentRole || newUser != CurrentUser)
                                    {
                                        if (!string.IsNullOrWhiteSpace(CurrentUser))
                                            Logouted?.Invoke(null, new LogoutEventArgs(CurrentUser, CurrentRole, DateTime.Now));
                                        Logged?.Invoke(null, new LoginEventArgs(newUser, newRole, DateTime.Now));
                                    }
                                    CurrentRole = newRole;
                                    CurrentUser = newUser;
                                    return true;
                                }
                                else
                                {
                                    error = "Wrong username or password!";
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    error = "Database or table doesn't exists.";
                }
            }
            catch (Exception ex)
            {
                error = $"Error: {ex.ToString()}";
            }
            return false;
        }

        public static bool Logout()
        {
            if (!string.IsNullOrEmpty(CurrentUser))
                Logouted?.Invoke(null, new LogoutEventArgs(CurrentUser, CurrentRole, DateTime.Now));
            CurrentUser = string.Empty;
            CurrentRole = null;
            return true;
        }

        public static LogProfile GetDbProfile(IServiceProvider context)
        {
            string applicationPath = DesignerHelper.GetApplicationOutputPath(context);
            string authenticateSettingPath = applicationPath + "\\AuthenticationProfile.json";
            try
            {
                if (File.Exists(authenticateSettingPath))
                {
                    LogProfile result = JsonConvert.DeserializeObject<LogProfile>(Base64Decode(File.ReadAllText(authenticateSettingPath)));
                    return result;
                }
                return null;
            }
            catch { return null; }
        }

        public static void SaveDbProfile(IServiceProvider context, LogProfile profile)
        {
            string applicationPath = DesignerHelper.GetApplicationOutputPath(context);
            string authenticateSettingPath = applicationPath + "\\AuthenticationProfile.json";
            File.WriteAllText(authenticateSettingPath, Base64Encode(JsonConvert.SerializeObject(profile, Formatting.Indented)));
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static List<Role> GetRoles(IServiceProvider context)
        {
            if (!isFirstInit)
            {
                if (RolesSource == null)
                    RolesSource = DesignerHelper.GetRoleSettings(context);
                else
                    RolesSource = new List<Role>();

                isFirstInit = true;
            }

            return RolesSource;
        }

        public static Role GetCurrentRole()
        {
            return GetRoles(null)?.FirstOrDefault(x => x.Name == CurrentRole);
        }

        public static Role GetRole(string roleName)
        {
            return GetRoles(null)?.FirstOrDefault(x => x.Name == roleName);
        }

        public static bool Authenticate(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return true;
           
            Role role = GetRole(roleName);
            Role currentRole = GetCurrentRole();
            if (role != null)
            {
                if (currentRole != null)
                    return role.Level <= currentRole.Level;
            }
            return false;
        }

        public static event EventHandler<LoginEventArgs> Logged;
        public static event EventHandler<LogoutEventArgs> Logouted;
    }
}
