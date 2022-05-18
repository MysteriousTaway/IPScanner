namespace IPScanner {
    public class TextAppend {
        public string text;
        public string fileName;
        public int threadNum;

        public TextAppend(string text, string fileName, int threadNum) {
            this.text = text;
            this.fileName = fileName;
            this.threadNum = threadNum;
        }
    }
}