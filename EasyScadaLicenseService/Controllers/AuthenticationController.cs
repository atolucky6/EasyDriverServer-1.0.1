using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyScadaLicenseService.Controllers
{
    public class AuthenticationController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public string Get(string serialKey, string product, string computerId)
        {
            LicenseModel result = new LicenseModel();
            try
            {
                string conStr = "Server = localhost; Port = 3306; Uid = easyscadalicense; Pwd = C0ngD@t!)@@)@); Database = easy_scada_license";
                using (MySqlConnection conn = new MySqlConnection(conStr))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = $"call proc_license_authenticate('{serialKey}', '{product}', '{computerId}')";
                        using (MySqlDataAdapter adp = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adp.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                result.SerialKey = dt.Rows[0]["SerialKey"].ToString();
                                result.Product = dt.Rows[0]["Product"].ToString();
                                result.ComputerId = dt.Rows[0]["ComputerId"].ToString();
                                result.LicenseType = dt.Rows[0]["LicenseType"].ToString();
                                result.ActiveTime = dt.Rows[0]["ActiveTime"].ToString();
                                result.Activated = dt.Rows[0]["Activated"].ToString();
                            }
                        }
                    }
                }
            }
            catch { }
            return JsonConvert.SerializeObject(result);
        }
    }

    public class LicenseModel
    {
        public string SerialKey { get; set; }
        public string Product { get; set; }
        public string ComputerId { get; set; }
        public string LicenseType { get; set; }
        public string ActiveTime { get; set; }
        public string Activated { get; set; }
    }
}
