using System.Diagnostics;
using System.Threading;

namespace IPScanner {
    public class Autoflush {
        public int threadNum;
        public int autoFlushInterval;

        public Autoflush(int threadNum, int autoFlushInterval) {
            this.threadNum = threadNum;
            this.autoFlushInterval = autoFlushInterval;
        }

        public void Run() {
            while (!(threadNum + 1).Equals(Program.threadsFinished)) {
                Thread.Sleep(30000);
                for (int i = 0; i < threadNum; i++) {
                    Program.PrintInfoToConsole("Attempting to force flush writer buffer for thread [" + i + "]");
                    Program.ForceFlushWriteForThreadBuffer(i);
                }
            }
            Program.PrintWarningToConsole("AutoFlush thread stopped !");
        }
    }
}