using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace IPScanner {
    internal class Program {
        private static int port = 8728;
        
        private static int maskInt;

        private static long original;
        private static long current;
        private static long maskLong;
        
        public static void Main(string[] args) {
            try {
                // Get IP from user:
                Console.WriteLine("Valid IP format: 192.168.0.1");
                original = IPStringToLong(Console.ReadLine());
                
                // Get mask from user:
                Console.WriteLine("Valid mask format: 24");
                maskInt = Int32.Parse(Console.ReadLine());
                maskLong = GetMaskLong(original, maskInt);
                // Get first ip address long:
                Console.WriteLine("---------------------------");
                Console.WriteLine("maskLong: " + maskLong);
                GetFirstAddressLong(original, maskLong);
            } catch (Exception exception) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Internal error occured: {0}", exception);
                Console.ForegroundColor = ConsoleColor.White;
                Main(args);
            }
        }

        static long GetFirstAddressLong(long ip, long mask) {
            Console.WriteLine("ip: " + IPLongToString(ip));
            Console.WriteLine("mask: " + IPLongToString(mask));
            
            String ipBin = StringToBinary(IPLongToString(ip));
            String maskBin = StringToBinary(IPLongToString(mask));
            
            Console.WriteLine("ipBin: " + ipBin);
            Console.WriteLine("maskBin: " + maskBin);
            return 0;
        }
        
        private static string StringToBinary(string data) {
            StringBuilder sb = new StringBuilder();
 
            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        static long GetMaskLong(long ip , int mask) {
            return IPStringToLong(GetBroadcastAddressString(new IPAddress(ip), GetMaskFromInt(mask)));
        }

        static IPAddress GetMaskFromInt(int maskInt) {
            int remaining = 32-maskInt;
            String ipBin = "";
            
            for (int i = 0; i < maskInt; i++) { ipBin += "1"; }
            for (int j = 0; j < remaining; j++) { ipBin += "0"; }

            string a = Convert.ToInt32(ipBin.Substring(0, 8), 2).ToString();
            string b = Convert.ToInt32(ipBin.Substring(8, 8), 2).ToString();
            string c = Convert.ToInt32(ipBin.Substring(16, 8), 2).ToString();
            string d = Convert.ToInt32(ipBin.Substring(24, 8), 2).ToString();
            
            String ip = a + "." + b + "." + c + "." + d;
            return IPAddress.Parse(ip);
        }

        static String ConvertMaskIntString(int maskInt) {
            int remaining = 32-maskInt;
            String ipBin = "";
            
            for (int i = 0; i < maskInt; i++) { ipBin += "1"; }
            for (int j = 0; j < remaining; j++) { ipBin += "0"; }
            return ipBin.Substring(0, 8) + "." + ipBin.Substring(7, 8) + "." + ipBin.Substring(15, 8) + "." + ipBin.Substring(23, 8);
        }
        
        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask) {
            uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
        }
        
        private static String GetBroadcastAddressString(IPAddress address, IPAddress mask) {
            uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint broadCastIpAddress = ipAddress | ~ipMaskV4;

            return Convert.ToString(broadCastIpAddress);
        }

        static long IPStringToLong(string addr) {
            return (long) (uint) IPAddress.NetworkToHostOrder((int) IPAddress.Parse(addr).Address);
        }

        static string IPLongToString(long address) { 
            Console.WriteLine( IPAddress.Parse(address.ToString()).ToString() + "/" + address.ToString());
            return IPAddress.Parse(address.ToString()).ToString();
        }

        static bool CanPingIP(String ipAddr) {
            Ping pingSender = new Ping ();
            PingOptions options = new PingOptions ();
            options.DontFragment = true;
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes (data);
            int timeout = 1;
            PingReply reply = pingSender.Send(ipAddr, timeout, buffer, options);
            if (reply.Status == IPStatus.Success) {
                Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
                return true;
            }
            return false;
        }
    }
}