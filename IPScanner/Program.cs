using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
namespace IPScanner {
    internal class Program {
        public static int threadNum;
        public static int threadsFinished;
        public static void Main(string[] args) {
            string ip = "";
            int mask = -1;
            int port = -1;
            
            int msTimeout = 1000;
            string outputFile = "default";
            bool printConnectionFailures = false;
            bool printToConsole = true;

            if (args.Length.Equals(0)) {
                Console.WriteLine(
                    "Example: -ip 192.168.255.0 -mask 24 -port 8728 -msTimeout 100 -outputFile default -printConnectionFailures false -printToConsole true -thread 0"+
                    "Available args: (arguments themselves are not case sensitive)" +
                    "-thread                   ()          [Separates where args for threads end and must be on the end even if you want to use only one thread make sure to write index of the thread after that]" +
                    "-ip                       (string)    " +
                    "-mask                     (int)       " +
                    "-port                     (int)       " +
                    "-msTimeout                (int)       [For how long the program is trying to contact a specific IP address]" +
                    "-outputFile               (string)    [Location of output file]" +
                    "-printConnectionFailures  (bool)      [Prints red messages when unable to connect to IP]" +
                    "-printToConsole           (bool)      [Prints green messages when able to connect to IP]"
                );
                Console.ReadLine();
                return;
            }

            List<Thread> threadPool = new List<Thread>();
            List<Argument> arguments = new List<Argument>();

            // basically a current thread will be used: (0 first index)
            threadNum = 0;
            for (int i = 0; i < args.Length; i = i + 2 ) {
                switch (args[i].ToLower()) {
                    case "-ip":
                    case "--ip":
                        ip = args[i+1];
                        break;
                    case "-mask":
                    case "--mask":
                        mask = Int32.Parse(args[i+1]);
                        break;
                    case "-port":
                    case "--port":
                        port = Int32.Parse(args[i+1]);
                        break;
                    case "-mstimeout":
                    case "--mstimeout":
                        msTimeout = Int32.Parse(args[i+1]);
                        break;
                    case "-outputfile":
                    case "--outputfile":
                        outputFile = args[i+1];
                        break;
                    case "-printconnectionfailures":
                    case "--printconnectionfailures":
                        printConnectionFailures = Boolean.Parse(args[i+1]);
                        break;
                    case "-printtoconsole":
                    case "--printtoconsole":
                        printToConsole = Boolean.Parse(args[i+1]);
                        break;
                    case "-thread":
                    case "--thread":
                        arguments.Add(new Argument(ip, mask, port, msTimeout, outputFile, printConnectionFailures, printToConsole, threadNum));
                        Console.WriteLine("{0}] ip: {1} mask: {2} port: {3} msTimeout: {4} outputFile: {5} printConnectionFailures: {6} printToConsole: {7}", arguments[threadNum].threadNum , arguments[threadNum].ip, arguments[threadNum].mask, arguments[threadNum].port, arguments[threadNum].msTimeout, arguments[threadNum].outputFile, arguments[threadNum].printConnectionFailures, arguments[threadNum].printToConsole);
                        threadNum++;
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
                // Check validity of args for threads:
                if (!varArgument.ip.Equals("") || !varArgument.mask.Equals(-1) || !varArgument.port.Equals(-1) || !varArgument.outputFile.Equals("")) {
                    Subnet subnet = new Subnet(varArgument.ip, varArgument.mask, varArgument.port, varArgument.msTimeout,
                        varArgument.outputFile, varArgument.printConnectionFailures, varArgument.printToConsole,
                        varArgument.threadNum);
                    textAppend.Add(new TextAppend("", varArgument.outputFile, varArgument.threadNum));
                    // Console.WriteLine("{0}] ip: {1} mask: {2} port: {3} msTimeout: {4} outputFile: {5} printConnectionFailures: {6} printToConsole: {7}", varArgument.threadNum , varArgument.ip, varArgument.mask, varArgument.port, varArgument.msTimeout, varArgument.outputFile, varArgument.printConnectionFailures, varArgument.printToConsole);
                    Thread thread = new Thread(() =>
                        subnet.Run()
                    );
                    threadPool.Add(thread);
                    
                } else {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    if (varArgument.ip.Equals("")) {
                        Console.WriteLine("[thread: {0}] IP has not been set!", varArgument.threadNum);
                    }
                    if (varArgument.mask.Equals(-1)) {
                        Console.WriteLine("[thread: {0}] Mask has not been set!", varArgument.threadNum);
                    }
                    if (varArgument.port.Equals(-1)) {
                        Console.WriteLine("[thread: {0}] Port has not been set!", varArgument.threadNum);
                    }
                    if (varArgument.outputFile.Equals("")) {
                        Console.WriteLine("[thread: {0}] Output file location has not been set!", varArgument.threadNum);
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            int k = 0;
            // Run all threads:
            foreach (var t in threadPool) {
                t.Start();
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Thread {0} started!", k);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                k++;
            }

            threadsFinished = 0;
            Autoflush autoFlush = new Autoflush(threadNum, 30000);
            Thread autoFlushThread = new Thread(() => autoFlush.Run());
            autoFlushThread.Start();
            Console.ReadLine();
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
            Console.WriteLine("[" + DateTime.Now + "]" + text);
        }
        public static void PrintInfoToConsole(string text) {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("[" + DateTime.Now + "]" + text);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintScanFinishToConsole(string text) {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("[" + DateTime.Now + "]" + text);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintWarningToConsole(string text) {
            Console.Error.WriteLine("! " + "[" + DateTime.Now + "]" + text);
        }
    }
}