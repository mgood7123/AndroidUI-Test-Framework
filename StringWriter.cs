// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  StringWriter
** 
** <OWNER>Microsoft</OWNER>
**
** Purpose: For writing text to a string
**
**
===========================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
#if FEATURE_ASYNC_IO
using System.Threading.Tasks;
#endif

namespace AndroidUITestFramework
{
    public class StringWriter : System.IO.TextWriter
    {
        private static volatile UnicodeEncoding m_encoding = null;
        private Stack<int> indentLevel;
        private Stack<int> indentSize;
        private bool _isOpen;
        private StringBuilder _sb;

        NewLineConverter newLineConverter;

        bool flushed;

        public StringWriter()
        {
            indentLevel = new();
            indentSize = new();
            indentLevel.Push(0);
            indentSize.Push(4);
            _isOpen = true;
            _sb = new StringBuilder();
            newLineConverter = new(this);
            newLineConverter.addConversion("\r\n", Environment.NewLine);
            newLineConverter.addConversion("\n", Environment.NewLine);
            flushed = true;
        }

        public override Encoding Encoding
        {
            get
            {
                if (m_encoding == null)
                {
                    m_encoding = new UnicodeEncoding(false, false);
                }
                return m_encoding;
            }
        }

        public override void Flush()
        {
            base.Flush();
            Clear();
        }

        public void Clear()
        {
            _sb.Clear();
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            // Do not destroy _sb, so that we can extract this after we are
            // done writing (similar to MemoryStream's GetBuffer & ToArray methods)
            _isOpen = false;
            base.Dispose(disposing);
        }

        // Writes a character to the underlying string buffer.
        //
        public override void Write(char value)
        {
            if (!_isOpen)
                WriterClosed();

            if (flushed)
            {
                flushed = false;
                int indentLevel = this.indentLevel.Peek();
                int indentSize = this.indentSize.Peek();
                for (int i = 0; i < indentLevel; i++)
                {
                    for (int _ = 0; _ < indentSize; _++)
                    {
                        _sb.Append(' ');
                    }
                }
            }
            (string str, bool isNewLine) = newLineConverter.processNext(value);
            if (str != null)
            {
                _sb.Append(str);
                if (isNewLine)
                {
                    Flush();
                    flushed = true;
                }
            }
        }

        internal static void WriterClosed()
        {
            throw new ObjectDisposedException(null, "Writer Closed");
        }

        // Writes a range of a character array to the underlying string buffer.
        // This method will write count characters of data into this
        // StringWriter from the buffer character array starting at position
        // index.
        //
        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Null Buffer");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Need Non-Negative Num");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need Non-Negative Numm");
            if (buffer.Length - index < count)
                throw new ArgumentException("Invalid Offset Length");
            Contract.EndContractBlock();

            if (!_isOpen)
                WriterClosed();

            for (int i = index; i < count; i++) Write(buffer[i]);
        }

        // Writes a string to the underlying string buffer. If the given string is
        // null, nothing is written.
        //
        public override void Write(string value)
        {
            if (!_isOpen)
                WriterClosed();
            if (value != null)
            {
                foreach(char c in value)
                    Write(c);
            }
        }


#if FEATURE_ASYNC_IO
        #region Task based Async APIs
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task WriteAsync(char value)
        {
            Write(value);
            return Task.CompletedTask;
        }
 
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task WriteAsync(String value)
        {
            Write(value);
            return Task.CompletedTask;
        }
 
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            Write(buffer, index, count);
            return Task.CompletedTask;
        }
 
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task WriteLineAsync(char value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }
 
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task WriteLineAsync(String value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }
 
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            WriteLine(buffer, index, count);
            return Task.CompletedTask;
        }
 
        [HostProtection(ExternalThreading = true)]
        [ComVisible(false)]
        public override Task FlushAsync()
        {
            return Task.CompletedTask;
        }
        #endregion
#endif //FEATURE_ASYNC_IO

        // Returns a string containing the characters written to this TextWriter
        // so far.
        //
        public override string ToString()
        {
            return _sb.ToString();
        }

        public void pushIndent()
        {
            indentLevel.Push(indentLevel.Peek() + 1);
        }

        public void popIndent()
        {
            if (indentLevel.Count > 1)
            {
                indentLevel.Pop();
            }
        }

        public int getIndentLevel()
        {
            return indentLevel.Peek();
        }

        public void pushIndentSize(int size)
        {
            indentSize.Push(size);
        }

        public void popIndentSize()
        {
            if (indentSize.Count > 1)
            {
                indentSize.Pop();
            }
        }

        public int getIndentSize()
        {
            return indentSize.Peek();
        }

        public static StringWriter operator +(StringWriter left, StringWriter right)
        {
            left.Write(right);
            return left;
        }

        public static StringWriter operator +(StringWriter left, string right)
        {
            left.Write(right);
            return left;
        }

        public static StringWriter operator +(StringWriter left, char right)
        {
            left.Write(right);
            return left;
        }

        public static StringWriter operator +(StringWriter left, object right)
        {
            left.Write(right.ToString());
            return left;
        }
    }
}
