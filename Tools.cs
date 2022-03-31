﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace AndroidUITestFramework
{
    internal static class Exceptions
    {
        internal class TEST_FAIL_EXCEPTION : Exception
        {
            public TEST_FAIL_EXCEPTION()
            {
            }

            public TEST_FAIL_EXCEPTION(string message) : base(message)
            {
            }

            public TEST_FAIL_EXCEPTION(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected TEST_FAIL_EXCEPTION(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        internal class TEST_SKIPPED_EXCEPTION : Exception
        {
            public TEST_SKIPPED_EXCEPTION()
            {
            }

            public TEST_SKIPPED_EXCEPTION(string message) : base(message)
            {
            }

            public TEST_SKIPPED_EXCEPTION(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected TEST_SKIPPED_EXCEPTION(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

    public static class Tools
    {
        internal static void printStackTrace(StackFrame sf, MethodBase mb, System.IO.TextWriter writer)
        {
            bool displayFilenames = true;   // we'll try, but demand may fail
            string word_At = "at";
            string inFileLineNum = "{0}:{1}";
            if (mb != null)
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, "   {0} ", word_At));
                writer.Write("Method: ");
                Type t = mb.DeclaringType;
                // if there is a type (non global method) print it
                if (t != null)
                {
                    writer.Write(t.FullName.Replace('+', '.'));
                    writer.Write(".");
                }
                writer.Write(mb.Name);

                // deal with the generic portion of the method
                if (mb is MethodInfo && ((MethodInfo)mb).IsGenericMethod)
                {
                    Type[] typars = ((MethodInfo)mb).GetGenericArguments();
                    writer.Write("[");
                    int k = 0;
                    bool fFirstTyParam = true;
                    while (k < typars.Length)
                    {
                        if (fFirstTyParam == false)
                            writer.Write(",");
                        else
                            fFirstTyParam = false;

                        writer.Write(typars[k].Name);
                        k++;
                    }
                    writer.Write("]");
                }

                // arguments printing
                writer.Write("(");
                ParameterInfo[] pi = mb.GetParameters();
                bool fFirstParam = true;
                for (int j = 0; j < pi.Length; j++)
                {
                    if (fFirstParam == false)
                        writer.Write(", ");
                    else
                        fFirstParam = false;

                    string typeName = "<UnknownType>";
                    if (pi[j].ParameterType != null)
                        typeName = pi[j].ParameterType.Name;
                    writer.Write(typeName + " " + pi[j].Name);
                }
                writer.Write(")");

                writer.Write(Environment.NewLine);
                writer.Write(string.Format(CultureInfo.InvariantCulture, "     {0} ", "in"));
                writer.Write("Location: ");

                // source location printing
                if (displayFilenames && (sf.GetILOffset() != -1))
                {
                    // If we don't have a PDB or PDB-reading is disabled for the module,
                    // then the file name will be null.
                    string fileName = null;

                    // Getting the filename from a StackFrame is a privileged operation - we won't want
                    // to disclose full path names to arbitrarily untrusted code.  Rather than just omit
                    // this we could probably trim to just the filename so it's still mostly usefull.
                    try
                    {
                        fileName = sf.GetFileName();
                    }
#if FEATURE_CAS_POLICY
                        catch (NotSupportedException)
                        {
                            // Having a deprecated stack modifier on the callstack (such as Deny) will cause
                            // a NotSupportedException to be thrown.  Since we don't know if the app can
                            // access the file names, we'll conservatively hide them.
                            displayFilenames = false;
                        }
#endif // FEATURE_CAS_POLICY
                    catch (System.Security.SecurityException)
                    {
                        // If the demand for displaying filenames fails, then it won't
                        // succeed later in the loop.  Avoid repeated exceptions by not trying again.
                        displayFilenames = false;
                    }

                    // tack on " in c:\tmp\MyFile.cs:line 5"
                    writer.Write(' ');
                    writer.Write(string.Format(CultureInfo.InvariantCulture, inFileLineNum, fileName ?? "<filename unknown>", sf.GetFileLineNumber()));
                }

#if FEATURE_EXCEPTIONDISPATCHINFO
                    if (sf.GetIsLastFrameFromForeignExceptionStackTrace())
                    {
                        writer.Write(Environment.NewLine);
                        writer.Write"Exception_EndStackTraceFromPreviousThrow");
                    }
#endif // FEATURE_EXCEPTIONDISPATCHINFO
            }

            writer.Write(Environment.NewLine);
        }

        static void printStackTrace(int offset, System.IO.TextWriter writer)
        {
            StackTrace stackTrace = new StackTrace(offset + 2, true);
            StackFrame[] frames = stackTrace.GetFrames();
            foreach(StackFrame frame in frames)
            {
                MethodBase mb = frame.GetMethod();
                if (mb != null)
                {
                    if (mb.DeclaringType.Namespace != "AndroidUITestFramework")
                    {
                        printStackTrace(frame, mb, writer);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        static void PRINT_FAIL(int offset, string reason, string message)
        {
            if (AndroidUITestFramework.Main.TestGroupInformation.CURRENT_TEST != null)
            {
                AndroidUITestFramework.Main.TestGroupInformation.CURRENT_TEST.failed = true;
            }
            ConsoleWriter x = new();
            x.pushForegroundColor(ConsoleColor.Red);
            Console.WriteLine("Test Failed");
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.DarkYellow);
            Console.WriteLine("Reason: " + (reason ?? "<No Reason Given>"));
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.Cyan);
            Console.WriteLine("Message: " + (message ?? "<No Message Given>"));
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.Blue);
            Console.WriteLine("Location:");
            printStackTrace(offset, Console.Out);
            x.popForegroundColor();
        }

        public static void RETHROW_EXCEPTION_IF_NEEDED(Exception e)
        {
            switch (e)
            {
                case Exceptions.TEST_FAIL_EXCEPTION:
                case Exceptions.TEST_SKIPPED_EXCEPTION:
                    throw e;
            }
        } 

        public static void SKIP(string message = null)
        {
            if (message != null)
            {
                ConsoleWriter x = new();
                x.pushForegroundColor(ConsoleColor.Red);
                Console.WriteLine("Test Skipped");
                x.popForegroundColor();
                x.pushForegroundColor(ConsoleColor.Cyan);
                Console.WriteLine("Message: " + (message ?? "<No Message Given>"));
                x.popForegroundColor();
            }
            throw new Exceptions.TEST_SKIPPED_EXCEPTION();
        }

        public static void FAIL(string message = null) {
            PRINT_FAIL(1, null, message);
            throw new Exceptions.TEST_FAIL_EXCEPTION();
        }

        static string StringOrNull(object o) => o is null ? "<null>" : o.ToString();

        static string ActualExpect(object value, object expect)
        {
            return 
                "      Actual: " + StringOrNull(value) + "\n" +
                "    Expected: " + StringOrNull(expect);
        }

        public static void ExpectTrue(bool expression, string message = null)
        {
            if (!expression)
            {
                PRINT_FAIL(1, "Expected the expression to be true, it was false", message);
            }
        }

        public static void AssertTrue(bool expression, string message = null)
        {
            if (!expression)
            {
                PRINT_FAIL(1, "Expected the expression to be true, it was false", message);
                throw new Exceptions.TEST_FAIL_EXCEPTION();
            }
        }

        public static void ExpectFalse(bool expression, string message = null)
        {
            if (expression)
            {
                PRINT_FAIL(1, "Expected the expression to be false, it was true", message);
            }
        }

        public static void AssertFalse(bool expression, string message = null)
        {
            if (expression)
            {
                PRINT_FAIL(1, "Expected the expression to be false, it was true", message);
                throw new Exceptions.TEST_FAIL_EXCEPTION();
            }
        }

        public static void ExpectEqual(object value, object expect, string message = null)
        {
            if (!expect.Equals(value))
            {
                PRINT_FAIL(1,
                    "Expected the following values to be equal\n" +
                    ActualExpect(value, expect),
                    message);
            }
        }

        public static void AssertEqual(object value, object expect, string message = null)
        {
            if (!expect.Equals(value))
            {
                PRINT_FAIL(1,
                    "Expected the following values to be equal\n" +
                    ActualExpect(value, expect),
                    message);
                throw new Exceptions.TEST_FAIL_EXCEPTION();
            }
        }

        public static void ExpectNotEqual(object value, object expect, string message = null)
        {
            if (expect.Equals(value))
            {
                PRINT_FAIL(1,
                    "Expected the following values to be not equal\n" +
                    ActualExpect(value, expect),
                    message);
            }
        }

        public static void AssertNotEqual(object value, object expect, string message = null)
        {
            if (expect.Equals(value))
            {
                PRINT_FAIL(1,
                    "Expected the following values to be not equal\n" +
                    ActualExpect(value, expect),
                    message);
                throw new Exceptions.TEST_FAIL_EXCEPTION();
            }
        }

        public static void ExpectInstanceEqual(object value, object expect, string message = null)
        {
            if (expect != value)
            {
                PRINT_FAIL(1,
                    "Expected the following instances to be equal\n" +
                    ActualExpect(value, expect),
                    message);
            }
        }

        public static void AssertInstanceEqual(object value, object expect, string message = null)
        {
            if (expect != value)
            {
                PRINT_FAIL(1,
                    "Expected the following instances to be equal\n" +
                    ActualExpect(value, expect),
                    message);
                throw new Exceptions.TEST_FAIL_EXCEPTION();
            }
        }

        public static void ExpectInstanceNotEqual(object value, object expect, string message = null)
        {
            if (expect == value)
            {
                PRINT_FAIL(1,
                    "Expected the following instances to be not equal\n" +
                    ActualExpect(value, expect),
                    message);
            }
        }

        public static void AssertInstanceNotEqual(object value, object expect, string message = null)
        {
            if (expect == value)
            {
                PRINT_FAIL(1,
                    "Expected the following instances to be not equal\n" +
                    ActualExpect(value, expect),
                    message);
                throw new Exceptions.TEST_FAIL_EXCEPTION();
            }
        }
    }
}