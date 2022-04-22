namespace AndroidUITestFramework
{
    public class NewLineDetector
    {
        System.IO.TextWriter textWriter;
        int index;
        int previousIndex;
        public System.Func<string> NewLine;
        private static object LOCK = new object();

        public NewLineDetector(System.IO.TextWriter textWriter, string from)
        {
            this.textWriter = textWriter;
            NewLine = () => from ?? this.textWriter.NewLine;
            reset();
        }

        public NewLineDetector(System.IO.TextWriter textWriter) : this(textWriter, null)
        {
        }

        public bool processNext(char c)
        {
            lock (LOCK)
            {
                string n = NewLine?.Invoke();
                if (n == null || n.Length == 0)
                    return false;

                previousIndex = index;

                if (n[index] == c)
                {
                    // n[0] = '\r'
                    // n[1] = '\n'
                    // index = 0
                    // n.Length = 2
                    // 0 == 1
                    if (index == n.Length - 1)
                    {
                        index = 0;
                        return true;
                    }
                    index++;
                }
                else
                {
                    index = 0;
                }
                return false;
            }
        }

        public void reset()
        {
            lock (LOCK)
            {
                index = 0;
                previousIndex = 0;
            }
        }

        public int getIndex() => index;
        public int getPreviousIndex() => previousIndex;
    }
}
