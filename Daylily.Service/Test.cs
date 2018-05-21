using Daylily.Common.Models;
using System;
using System.Threading;

namespace Daylily.Service
{
    public class Test : IService
    {
        public void Run()
        {
            Thread t = new Thread(Async);
            t.Start();
        }
        private static void Async()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine(DateTime.Now);
            }
        }
    }
}
