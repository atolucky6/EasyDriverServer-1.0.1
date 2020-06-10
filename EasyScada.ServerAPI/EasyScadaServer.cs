using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.ServerAPI
{
    public class EasyScadaServer
    {
        public EasyScadaServer()
        {
        }

        public void Start()
        {
            string url = $"http://*:9090";

            WebApp.Start(url);
        }
    }
}
