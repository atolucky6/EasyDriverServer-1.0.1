using EasyDriverPlugin;
using System.Threading.Tasks;

namespace EasyDriver.RemoteConnectionPlugin
{
    public abstract class RemoteStationBase : RemoteStation
    {
        protected RemoteStationBase(IGroupItem parent) : base(parent)
        {
        }
    }
}
