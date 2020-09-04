using EasyScada.Core.Evaluate;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TokenExpression token = new TokenExpression($"3 > tag[{'"'}222abc{'"'}]");
            if (token.AnyErrors)
                Console.WriteLine("Error");
            if (token.Variables.Count == 0)
            {
                Evaluator eval = new Evaluator(token);

                eval.Evaluate(out string value, out string error);
                Console.WriteLine($"Value: {value} - Error: {error}");
            }
            else
            {
                Console.WriteLine("Error");
            }
            Console.ReadLine();
        }
    }
}
