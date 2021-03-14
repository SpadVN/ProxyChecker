using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Leaf.xNet;

namespace ProxyChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Proxy Checker by DevXYZ#9788";
            var proxies = File.ReadAllLines("proxies.txt");
            var threads = 50;
            var requestTimeout = 5 * 1000;
            var proxyTimeout = 2 * 1000;
            var proxyType = ProxyType.Socks4;

            var good = 0;

            Parallel.ForEach(proxies, new ParallelOptions() {
                MaxDegreeOfParallelism = threads }, proxy =>
                {
                    try
                    {
                        using (var request = new HttpRequest())
                        {
                            request.ConnectTimeout = requestTimeout;
                            request.Proxy = ProxyClient.Parse(proxyType, proxy);
                            request.Proxy.ConnectTimeout = proxyTimeout;

                            request.Get("http://azenv.net/");

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[GOOD PROXY]" + proxy);
                            Console.ResetColor();

                            Interlocked.Increment(ref good);
                            File.AppendAllText("good.txt", proxyTimeout + Environment.NewLine);
                        }
                    }
                    catch 
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[BAD PROXY]" + proxy);
                        Console.ResetColor();
                    }
                });

            Console.WriteLine("Done checking!\n\n Good proxies: " + proxies);
            Console.ReadKey();
        }
    }
}
