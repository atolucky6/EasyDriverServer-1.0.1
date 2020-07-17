using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static Timer timer;
        static Stopwatch sw = new Stopwatch();
        static int count = 0;
        static int interval = 10000;

        static void Main(string[] args)
        {
            double v = 1;
            Console.WriteLine(v.ToString());
            v = 0;
            Console.WriteLine(v.ToString());


            timer = new Timer(new TimerCallback(Callback), null, 100, interval);
            while(true)
            {
                string value = Console.ReadLine();
                if (int.TryParse(value, out interval))
                {
                    timer.Change(0, interval);
                }
                else
                {
                    timer.Dispose();
                }
            }
        }

        private static void Callback(object state)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            if (sw == null)
                sw = new Stopwatch();
            sw.Stop();
            count++;
            Thread.Sleep(3000);
            Console.WriteLine($"Interval: {sw.ElapsedMilliseconds}, Count: {count}");
            sw.Restart();
            if (timer != null)
                timer.Change(interval, 0);
        }
    }
}
