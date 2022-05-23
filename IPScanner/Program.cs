using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
namespace IPScanner {
    internal class Program {
        public static int threadNum;
        public static int threadsFinished;
        
        // Required:
        public static string ip;
        public static int mask;
        public static int port;
             
        // Optional:
        public static int msTimeout;
        public static string outputFile;
        public static bool printConnectionFailures;
        public static bool printToConsole;

        public static string ipRange;
        public static string network;

        public static int flushInterval;
        public static bool ignoreExceptions;
        
        public static void Main(string[] args) {
            
            ip = "";
            mask = -1;
            port = -1;
            msTimeout = 1000;
            outputFile = "default";
            printConnectionFailures = false;
            printToConsole = false;
            ignoreExceptions = false;
            ipRange = "";
            network = "";
            flushInterval = 30000;

            if (args.Length.Equals(0)) {
                Console.WriteLine(
                    "Example: -ip 192.168.255.0 -mask 24 -port 8728 -msTimeout 100 -outputFile default -printConnectionFailures false -printToConsole true -thread 0"+
                    "Available args: (arguments themselves are not case sensitive)" +
                    "-thread                   (int)       [Separates where args for threads end and must be on the end even if you want to use only one thread make sure to write index of the thread after that]" +
                    "-ip                       (string)    " +
                    "-mask                     (int)       " +
                    "-port                     (int)       " +
                    "-network                  (string)    [Example: -network 127.0.0.1/24]" +
                    "-ipRange                  (string)    [Example: -ipRange 127.0.0.1-127.0.10.1]" +
                    "-msTimeout                (int)       [For how long the program is trying to contact a specific IP address]" +
                    "-outputFile               (string)    [Location of output file]" +
                    "-printConnectionFailures  (bool)      [Prints red messages when unable to connect to IP]" +
                    "-printToConsole           (bool)      [Prints green messages when able to connect to IP]" +
                    "-flushInterval            (int)       [How often in milliseconds will AutoFlush save output to files.]" +
                    "-ignoreExceptions         (bool)      [If set to false then will not print out scan exceptions. Set to false by default.]"
                );
                // Console.ReadLine();
                return;
            }

            List<Thread> threadPool = new List<Thread>();
            List<Argument> arguments = new List<Argument>();

            // basically a current thread will be used: (0 first index)
            threadNum = 0;
            for (int i = 0; i < args.Length; i = i + 2 ) {
                args[i] = args[i].Replace("-", "");
                switch (args[i].ToLower()) {
                    case "ip":
                        ip = args[i+1];
                        break;
                    case "mask":
                        mask = Int32.Parse(args[i+1]);
                        break;
                    case "port":
                        port = Int32.Parse(args[i+1]);
                        break;
                    case "mstimeout":
                        msTimeout = Int32.Parse(args[i+1]);
                        break;
                    case "outputfile":
                        outputFile = args[i+1];
                        break;
                    case "printconnectionfailures":
                        printConnectionFailures = Boolean.Parse(args[i+1]);
                        break;
                    case "printtoconsole":
                        printToConsole = Boolean.Parse(args[i+1]);
                        break;
                    case"iprange":
                        ipRange = args[i + 1];
                        break;
                    case"network":
                        network = args[i + 1];
                        break;
                    case "thread":
                        arguments.Add(new Argument(ip, mask, port, msTimeout, outputFile, printConnectionFailures, printToConsole, threadNum, network, ipRange));
                        threadNum++;
                        break;
                    case "flushinterval":
                        flushInterval = Int32.Parse(args[i + 1]);
                        break;
                    case "ignoreexceptions":
                        ignoreExceptions = Boolean.Parse(args[i + 1]);
                        break;
                    default:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Invalid argument \"{0}\" at position {1}", args[i], i);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }

            foreach (var varArgument in arguments) {
                String inputDataType = "";

                if (!varArgument.ip.Equals("") && !varArgument.mask.Equals(-1) && !varArgument.port.Equals(-1)) {
                    inputDataType = "Standard";
                } else if (!varArgument.ipRange.Equals("") && !varArgument.port.Equals(-1)) {
                    inputDataType = "IP Range";
                } else if (!varArgument.network.Equals("")) {
                    inputDataType = "Network";
                } else {
                    inputDataType = "Undefined";
                }

                if (!inputDataType.Equals("Undefined")) {
                    if (printToConsole) {
                        Console.WriteLine("THREAD: [{0}] ARGS: [ip: {1} mask: {2} port: {3} msTimeout: {4} outputFile: {5} printConnectionFailures: {6} printToConsole: {7} network: {8} ipRange: {9}]", varArgument.threadNum, varArgument.ip, varArgument.mask, varArgument.port, varArgument.msTimeout, varArgument.outputFile, varArgument.printConnectionFailures, varArgument.printToConsole, varArgument.network, varArgument.ipRange);
                    }
                    Subnet subnet = new Subnet(varArgument.ip, varArgument.mask, varArgument.port, varArgument.msTimeout,
                        varArgument.outputFile, varArgument.printConnectionFailures, varArgument.printToConsole,
                        varArgument.threadNum, varArgument.network, varArgument.ipRange);
                    textAppend.Add(new TextAppend("", varArgument.outputFile, varArgument.threadNum));
                    Thread thread = new Thread(() =>
                        subnet.Run()
                    );
                    threadPool.Add(thread);
                } else {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Network parameters for thread [{0}] might not be correctly defined! This network will not be scanned! Parameters: -ip = {1} -mask = {2} -port = {3} -msTimeout = {4} -outputFile = {5} -printConnectionFailures = {6} -printToConsole = {7} -network = {8} -ipRange = {9}", varArgument.threadNum, varArgument.ip, varArgument.mask, varArgument.port, varArgument.msTimeout, varArgument.outputFile, varArgument.printConnectionFailures, varArgument.printToConsole, varArgument.network, varArgument.ipRange);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            int k = 0;
            // Run all threads:
            foreach (var t in threadPool) {
                t.Start();
                if (printToConsole) {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Thread {0} started!", k);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                k++;
            }

            threadsFinished = 0;
            Autoflush autoFlush = new Autoflush(threadNum, flushInterval);
            Thread autoFlushThread = new Thread(() => autoFlush.Run());
            autoFlushThread.Start();
            // Console.ReadLine();
        }
        
        private static List<TextAppend> textAppend = new List<TextAppend>();
        public static void AppendLineToFile( String fileName, String line, int threadNum) {
            bool foundInstance = false;
            for (int i = 0; i < textAppend.Count; i++) {
                if (textAppend[i].threadNum.Equals(threadNum)) {
                    textAppend[i].text += line;
                    foundInstance = true;
                }
            }
            
            if (!foundInstance) {
                textAppend.Add(new TextAppend(line, fileName, threadNum));
            }
        }

        public static void ForceFlushWriteForThreadBuffer(int threadNum) {
            for (int i = 0; i < textAppend.Count; i++) {
                if (textAppend[i].threadNum.Equals(threadNum)) {
                    File.AppendAllText(textAppend[threadNum].fileName, textAppend[threadNum].text);
                    textAppend[threadNum].text = "";
                    PrintInfoToConsole("Forced flush of writing buffer for thread [" + threadNum + "]");
                }
            }
        }

        public static void PrintToConsole(string text) {
            if (printToConsole) {
                Console.WriteLine("[" + DateTime.Now + "]" + text);
            }
        }
        public static void PrintInfoToConsole(string text) {
            if (printToConsole) {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("[" + DateTime.Now + "]" + text);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public static void PrintScanFinishToConsole(string text) {
            if (printToConsole) {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("[" + DateTime.Now + "]" + text);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public static void PrintWarningToConsole(string text) {
            if (printToConsole) {
                Console.Error.WriteLine("! " + "[" + DateTime.Now + "]" + text);
            }
        }
        public static void PrintExceptionToConsole(string text) {
            if (printToConsole && !ignoreExceptions) {
                Console.Error.WriteLine("! " + "[" + DateTime.Now + "]" + text);
            }
        }
    }
}