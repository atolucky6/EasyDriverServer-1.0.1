using EasyDriver.ServicePlugin;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EasyDriver.Service.InternalStorage
{
    [Service(0, true)]
    public class InternalStorageService : EasyServicePluginBase, IInternalStorageService
    {
        string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public InternalStorageService() : base()
        {
        }

        public bool AddOrUpdateStoreValue(string guidID, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(guidID))
                {
                    using (var db = new LiteDatabase(GetConnectionString()))
                    {
                        var collection = db.GetCollection<StoreValue>("internal_tags");
                        StoreValue model = new StoreValue()
                        {
                            GUID = guidID,
                            LastUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Value = value
                        };
                        collection.EnsureIndex(x => x.GUID);
                        if (collection.Exists(x => x.GUID == guidID))
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
                return false;
            }
            catch (Exception) { return false; }
        }

        public StoreValue GetStoreValue(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                    return null;

                using (var db = new LiteDatabase(GetConnectionString()))
                {
                    var collection = db.GetCollection<StoreValue>("internal_tags");
                    collection.EnsureIndex(x => x.GUID);
                    var result = collection.FindOne(x => x.GUID == guid);
                    return result;
                }
            }
            catch { return null; }
        }

        public bool RemoveStoreValue(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                    return false;
                using (var db = new LiteDatabase(GetConnectionString()))
                {
                    var collection = db.GetCollection<StoreValue>("internal_tags");
                    collection.EnsureIndex(x => x.GUID);
                    return collection.Delete(new BsonValue(guid));
                }
            }
            catch { return false; }
        }

        public override void BeginInit()
        {
            base.BeginInit();
        }

        public override void EndInit()
        {
            base.EndInit();
        }

        private ConnectionString GetConnectionString()
        {
            string dbPath = appDir + "\\internal.db";
            ConnectionString constr = new ConnectionString
            {
                Filename = dbPath,
                Password = "88888888"
            };
            return constr;
        }
    }
}
