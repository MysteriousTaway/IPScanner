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
            for (int i = 0; i < args.Length; i++) {
                switch (args[i].ToLower()) {
                    case "-config":
                    case "--config":
                        // TODO: if the user inserts wrong config file then generate a prefab
                        // TODO: input yaml config and read values from that
                        /* ! config format:
                         * 0:
                         *  ip: 192.168.255.0
                         *  mask: 24
                         *  port: 8728
                         *  msTimeout: 100
                         *  outputFile: "default"
                         *  printConnectionFailures: false
                         */
                        break;
                }
            }
            List<Subnet> subnets = new List<Subnet>();
            subnets.Add(new Subnet("192.168.255.0", 24, 8728, 100, "default", true));
            subnets[0].Run();
        }
    }
}