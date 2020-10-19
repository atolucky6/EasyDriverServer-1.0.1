using EasyDriverPlugin;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IInternalStorageService
    {
        bool AddOrUpdateInternalTag(ITagCore tagCore);
        bool RemoveInternalTag(string guid);
        InternalTagModel GetInternalTagValue(string guid);
    }

    public class InternalStorageService : IInternalStorageService
    {
        public bool AddOrUpdateInternalTag(ITagCore tagCore)
        {
            try
            {
                if (tagCore != null && tagCore.IsInternalTag)
                {
                    if (tagCore.ParameterContainer.Parameters.ContainsKey("GUID"))
                    {
                        using (var db = new LiteDatabase(GetConnectionString()))
                        {
                            string guid = tagCore.ParameterContainer.Parameters["GUID"];
                            var collection = db.GetCollection<InternalTagModel>("internal_tags");
                            InternalTagModel model = new InternalTagModel()
                            {
                                GUID = guid,
                                LastUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Value = tagCore.Value
                            };
                            collection.EnsureIndex(x => x.GUID);
                            if (collection.Exists(x => x.GUID == guid))
                            {
                                var res = collection.Update(model);
                                return res;
                            }
                            else
                            {
                                var res = collection.Insert(model);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex) { return false; }
        }

        public InternalTagModel GetInternalTagValue(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                    return null;

                using (var db = new LiteDatabase(GetConnectionString()))
                {
                    var collection = db.GetCollection<InternalTagModel>("internal_tags");
                    collection.EnsureIndex(x => x.GUID);
                    var result = collection.FindOne(x => x.GUID == guid);
                    return result;
                }
            }
            catch { return null; }
        }

        public bool RemoveInternalTag(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                    return false;
                if (!Guid.TryParse(guid, out Guid g))
                    return false;
                using (var db = new LiteDatabase(GetConnectionString()))
                {
                    var collection = db.GetCollection<InternalTagModel>("internal_tags");
                    collection.EnsureIndex(x => x.GUID);
                    return collection.Delete(new BsonValue(guid));
                }
            }
            catch { return false; }
        }

        private ConnectionString GetConnectionString()
        {

            string dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\internal.db";
            ConnectionString constr = new ConnectionString();
            constr.Filename = dbPath;
            constr.Password = "88888888";
            return constr;
        }
    }
}
