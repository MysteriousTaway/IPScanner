using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace IPScanner {
    public class Subnet {
        // Vars:
        public string ip;
        public int mask;

        // long of ^
        public long ipLong;
        public long maskLong;

        public string networkIP;
        public string broadcastIP;
        
        // Port to which it's connecting:
        public int port;
        
        // the whole subnet thingy:
        public long networkIPLong;
        public long broadcastIPLong;

        public int msTimeout;
        public string outputFile;
        public bool printConnectionFailures;
        public bool printToConsole;
        public int threadNum;

        public Subnet(string ip, int mask, int port, int msTimeout, string outputFile, bool printConnectionFailures, bool printToConsole, int threadNum) {
            this.ip = ip;
            this.mask = mask;
            this.port = port;
            this.msTimeout = msTimeout;
            this.outputFile = outputFile;
            this.printConnectionFailures = printConnectionFailures;
            this.printToConsole = printToConsole;
            this.threadNum = threadNum;
            
            ipLong = ConvertIpToLong(ip);
            networkIP = GetNetworkIPLong(ip, mask);
            broadcastIP = GetBroadcastIPLong(ip, mask);
            networkIPLong = ConvertIpToLong(networkIP);
            broadcastIPLong = ConvertIpToLong(broadcastIP);
        }

        public void Run() {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (printToConsole) {
                Program.PrintInfoToConsole("[" + threadNum + "] Scan of " + networkIP + " started!");
            }
            for (long i = networkIPLong; i <= broadcastIPLong; i++) {
                try {
                    String currentIP = ConvertLongToIp(i);
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IAsyncResult result = socket.BeginConnect(currentIP, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(msTimeout, true);
                    if (success) {
                        if (printToConsole) {
                            Program.PrintToConsole("[" + threadNum + "] " + currentIP);
                        }

                        socket.Disconnect(false);

                        if (outputFile.ToLower().Equals("default")) {
                            Program.AppendLineToFile(networkIP.Replace(Char.Parse("."), Char.Parse("_")) + "_" + mask + "_" + port + ".txt", currentIP + "\n", threadNum);
                        } else {
                            Program.AppendLineToFile(outputFile, currentIP + "\n", threadNum);
                        }
                    } else {
                        if (printConnectionFailures) {
                            if (printToConsole) {
                                Program.PrintWarningToConsole(currentIP);
                            }
                        }
                    }
                }
                catch (Exception exception) {
                    Program.PrintWarningToConsole("Exception on thread [" + threadNum + "]\n" + exception.ToString());
                }
            }
            if (printToConsole) {
                Program.PrintScanFinishToConsole("[" + (Program.threadsFinished + 1) + "/" + Program.threadNum + "][" + threadNum + "] Scan of [" + networkIP + "] finished! \nProcess took: " + stopwatch.ElapsedMilliseconds + "ms");
            }
            stopwatch.Stop();
            Program.ForceFlushWriteForThreadBuffer(threadNum);
            Program.threadsFinished++;
        }

        #region IP conversion

        string GetNetworkIPLong(String ip, int mask) {
            String maskBinary = ConvertMaskBinaryToString(mask);
            long ipLong = ConvertIpToLong(ip);
            long maskLong = ConvertIpToLong(maskBinary);
            return ConvertLongToIp(ipLong & maskLong);
        }
        
        string GetBroadcastIPLong(String ip, int mask) {
            String maskBinary = ConvertMaskBinaryToString(mask);
            long ipLong = ConvertIpToLong(ip);
            long maskLong = ConvertIpToLong(maskBinary);
            
            return ConvertLongToIp(ipLong | ~maskLong);
        }
        
        long ConvertIpToLong(String ip) {
            String[] split = ip.Split(Char.Parse("."));
            int[] intSplit = new int[4];
            for (int i = 0; i < split.Length; i++) {
                intSplit[i] = Int32.Parse(split[i]);
            }

            intSplit[0] *= (int)Math.Pow(2, 24);
            intSplit[1] *= (int)Math.Pow(2, 16);
            intSplit[2] *= (int)Math.Pow(2, 8);
            intSplit[3] *= 1;
            return intSplit[0] + intSplit[1] + intSplit[2] + intSplit[3];
        }

        string ConvertLongToIp(long ip) {
            long[] split = new long[4];
            
            split[0] = (ip >> 24) & 0xFF;
            split[1] = (ip >> 16) & 0xFF;
            split[2] = (ip >> 8) & 0xFF;
            split[3] = (ip >> 0) & 0xFF;
            
            return split[0] + "." + split[1] + "." + split[2] + "." + split[3];
        }

        #endregion

        #region Mask conversion

        string ConvertMaskBinaryToString(int mask) {
            long mask1 = 0xFFFFFFFFF << 32 - mask;
            long[] maskArr = new long[4];
            maskArr[0] = (mask1 >> 24) & 0xFF;
            maskArr[1] = (mask1 >> 16) & 0xFF;
            maskArr[2] = (mask1 >> 8) & 0xFF;
            maskArr[3] = (mask1 >> 0) & 0xFF;
            return maskArr[0] + "." + maskArr[1] + "." + maskArr[2] + "." + maskArr[3];
        }
        #endregion
    }
}