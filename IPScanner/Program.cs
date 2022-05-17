using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace IPScanner {
    internal class Program {
        private static int port = 8728;
        

        private static long original;
        private static long current;
        private static long final;
        
        public static void Main(string[] args) {
            // Get IP from user:
            Console.WriteLine("Valid network format: 192.168.0.1 \n IP:");
            original = ToInt(Console.ReadLine());
            // TODO: Check if ip is valid.
        }
        
        static long ToInt(string addr) {
            return (long) (uint) IPAddress.NetworkToHostOrder((int) IPAddress.Parse(addr).Address);
        }

        static string ToAddr(long address) { 
            return IPAddress.Parse(address.ToString()).ToString();
        }

        static bool canPingIP(String ipAddr) {
            Ping pingSender = new Ping ();
            PingOptions options = new PingOptions ();
            options.DontFragment = true;
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes (data);
            int timeout = 120;
            PingReply reply = pingSender.Send (ipAddr, timeout, buffer, options);
            if (reply.Status == IPStatus.Success) {
                Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
                return true;
            }
            return false;
        }
    }
}