using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace IPScanner {
    internal class Program {
        // private static int port = 8728;
        public static void Main(string[] args) {
            List<Subnet> subnets = new List<Subnet>();
            // try {
            //     // Get IP from user:
            //     Console.WriteLine("Valid IP format: 192.168.0.1");
            //     string ip = Console.ReadLine();
            //     
            //     // Get mask from user:
            //     int mask;
            //     do {
            //         Console.WriteLine("Valid mask format: 24");
            //         mask = Int32.Parse(Console.ReadLine());
            //     } while (mask < 0 || mask > 32);
            //     
            //     // Get port:
            //     Console.WriteLine("Valid port format: 8728");
            //     int port = Int32.Parse(Console.ReadLine());
            //     subnets.Add(new Subnet(ip, mask, port));
            //     
            // } catch (Exception exception) {
            //     Console.ForegroundColor = ConsoleColor.Red;
            //     Console.WriteLine("Internal error occured: {0}", exception);
            //     Console.ForegroundColor = ConsoleColor.White;
            //     Main(args);
            // }
            subnets.Add(new Subnet("192.168.255.0", 24, 8728, 100));
            subnets[0].Run();
        }
    }
}