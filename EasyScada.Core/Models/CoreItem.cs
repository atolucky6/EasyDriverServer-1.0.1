using Newtonsoft.Json;

namespace EasyScada.Core
{
    public class CoreItem : CoreItemBase
    {
        [JsonConstructor]
        public CoreItem() : base()
        {
        }
    }
}
