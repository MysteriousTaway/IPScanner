namespace IPScanner {
    public class Argument {
        public string ip;
        public int mask;
        public int port;
        public int msTimeout;
        public string outputFile;
        public bool printConnectionFailures;
        public bool printToConsole;
        public int threadNum;
        public string network;
        public string ipRange;

        public Argument(string ip, int mask, int port, int msTimeout, string outputFile, bool printConnectionFailures, bool printToConsole, int threadNum, string network, string ipRange) {
            this.ip = ip;
            this.mask = mask;
            this.port = port;
            this.msTimeout = msTimeout;
            this.outputFile = outputFile;
            this.printConnectionFailures = printConnectionFailures;
            this.printToConsole = printToConsole;
            this.threadNum = threadNum;
            this.network = network;
            this.ipRange = ipRange;
        }
    }
}