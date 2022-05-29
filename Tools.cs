using System;
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
            public Exception reason;
            public StackTrace stack;
            public TEST_FAIL_EXCEPTION()
            {
            }

            public TEST_FAIL_EXCEPTION(StackTrace stack)
            {
                this.stack = stack;
            }

            public TEST_FAIL_EXCEPTION(string message) : base(message)
            {
            }

            public TEST_FAIL_EXCEPTION(string message, StackTrace stack) : base(message)
            {
                this.stack = stack;
            }

            public TEST_FAIL_EXCEPTION(Exception reason, string message) : base(message)
            {
                this.reason = reason;
            }

            public TEST_FAIL_EXCEPTION(Exception reason, string message, StackTrace stack) : base(message)
            {
                this.reason = reason;
                this.stack = stack;
            }

            public TEST_FAIL_EXCEPTION(string message, Exception innerException) : base(message, innerException)
            {
            }

            public TEST_FAIL_EXCEPTION(string message, Exception innerException, StackTrace stack) : base(message, innerException)
            {
                this.stack = stack;
            }

            public TEST_FAIL_EXCEPTION(Exception reason, string message, Exception innerException) : base(message, innerException)
            {
                this.reason = reason;
            }

            public TEST_FAIL_EXCEPTION(Exception reason, string message, Exception innerException, StackTrace stack) : base(message, innerException)
            {
                this.reason = reason;
                this.stack = stack;
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
        internal static void PrintStackTrace(StackFrame sf, MethodBase mb, System.IO.TextWriter writer)
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
                        {
                            writer.Write(",");
                        }
                        else
                        {
                            fFirstTyParam = false;
                        }

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
                    {
                        writer.Write(", ");
                    }
                    else
                    {
                        fFirstParam = false;
                    }

                    string typeName = "<UnknownType>";
                    if (pi[j].ParameterType != null)
                    {
                        typeName = pi[j].ParameterType.Name;
                    }

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

        static void PrintStackTrace(int offset, System.IO.TextWriter writer)
        {
            PrintStackTrace(new StackTrace(offset + 2, true), writer);
        }

        static void PrintStackTrace(StackTrace stackTrace, System.IO.TextWriter writer)
        {
            StackFrame[] frames = stackTrace.GetFrames();
            foreach (StackFrame frame in frames)
            {
                MethodBase mb = frame.GetMethod();
                if (mb != null)
                {
                    if (mb.DeclaringType.Namespace != "AndroidUITestFramework")
                    {
                        PrintStackTrace(frame, mb, writer);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public static void PrintLocation(Exception e)
        {
            PrintStackTrace(new StackTrace(e, true), Console.Out);
        }

        static void PRINT_FAIL(int offset, string reason, string message)
        {
            if (Main.TestGroupInformation.CURRENT_TEST != null)
            {
                Main.TestGroupInformation.CURRENT_TEST.failed = true;
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
            PrintStackTrace(offset, Console.Out);
            x.popForegroundColor();
        }

        static void PRINT_FAIL(int offset, Exception reason, string message)
        {
            if (Main.TestGroupInformation.CURRENT_TEST != null)
            {
                Main.TestGroupInformation.CURRENT_TEST.failed = true;
            }
            ConsoleWriter x = new();
            x.pushForegroundColor(ConsoleColor.Red);
            Console.WriteLine("Test Failed");
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.DarkYellow);
            if (reason == null)
            {
                Console.WriteLine("Reason: <No Reason Given>");
            }
            else
            {
                Console.WriteLine("Reason: " + reason.GetType().FullName + ": " + reason.Message);
                x.pushIndent();
                x.pushForegroundColor(ConsoleColor.Blue);
                Console.WriteLine("Location:");
                PrintLocation(reason);
                x.popForegroundColor();
                x.popIndent();
            }
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.Cyan);
            Console.WriteLine("Message: " + (message ?? "<No Message Given>"));
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.Blue);
            Console.WriteLine("Location:");
            PrintStackTrace(offset, Console.Out);
            x.popForegroundColor();
        }

        internal static void PrintTestFailedException(Exceptions.TEST_FAIL_EXCEPTION e)
        {
            if (Main.TestGroupInformation.CURRENT_TEST != null)
            {
                Main.TestGroupInformation.CURRENT_TEST.failed = true;
            }
            ConsoleWriter x = new();
            x.pushForegroundColor(ConsoleColor.Red);
            Console.WriteLine("Test Failed");
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.DarkYellow);
            if (e.reason == null)
            {
                Console.WriteLine("Reason: <No Reason Given>");
            }
            else
            {
                Console.WriteLine("Reason: " + e.reason.GetType().FullName + ": " + e.reason.Message);
                x.pushIndent();
                x.pushForegroundColor(ConsoleColor.Blue);
                Console.WriteLine("Location:");
                PrintLocation(e.reason);
                x.popForegroundColor();
                x.popIndent();
            }
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.Cyan);
            Console.WriteLine("Message: " + (e.Message ?? "<No Message Given>"));
            x.popForegroundColor();
            x.pushForegroundColor(ConsoleColor.Blue);
            Console.WriteLine("Location:");
            PrintStackTrace(e.stack, Console.Out);
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
            PRINT_FAIL(1, (string)null, message);
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

        static bool floating_point_type_value_equals<T>(object a, object b, Func<T, bool> isNaN, Func<T, bool> isInfinity)
        {
            T TA = (T)Convert.ChangeType(a, typeof(T), CultureInfo.InvariantCulture);
            T TB = (T)Convert.ChangeType(b, typeof(T), CultureInfo.InvariantCulture);
            return isNaN(TA) ? isNaN(TB) : isInfinity(TA) ? isInfinity(TB) : TA.Equals(TB);
        }

        static Type FLOAT_T = typeof(float);
        static Type DOUBLE_T = typeof(double);

        static bool compare_float_double(float a, double b)
        {
            if (float.IsNaN(a) && double.IsNaN(b)) return true;
            bool binf = double.IsInfinity(b);
            if (float.IsInfinity(a) && binf) return true;
            double da = (double)a; // float -> double should never fail
            double db;
            // try to downcast b to float
            float f = (float)b;
            if (float.IsInfinity(f))
            {
                if (binf) return true;
                // b downcast was infinity
                // try to upcast a float to a double
                db = (double)b;
            }
            else
            {
                db = (double)f;
            }
            return da.Equals(db);
        }

        static bool promote_and_equals<T>(object a, object b)
        {
            // special case, float vs double comparison
            Type ta = a.GetType();
            Type tb = b.GetType();
            Type t = typeof(T);
            bool ta_f = ta == FLOAT_T;
            bool ta_d = ta == DOUBLE_T;
            bool tb_f = tb == FLOAT_T;
            bool tb_d = tb == DOUBLE_T;
            bool t_f = t == FLOAT_T;
            bool t_d = t == DOUBLE_T;
            float f1;
            float f2;
            double d1;
            double d2;
            if (ta_f)
            {
                if (tb_f)
                {
                    f1 = (float)a;
                    f2 = (float)b;
                    return float.IsNaN(f1) ? float.IsNaN(f2) : float.IsInfinity(f1) ? float.IsInfinity(f2) : f1.Equals(f2);
                }
                else if (tb_d)
                {
                    f1 = (float)a;
                    d1 = (double)b;
                    return compare_float_double(f1, d1);
                }
            }
            else if (ta_d)
            {
                if (tb_d)
                {
                    d1 = (double)a;
                    d2 = (double)b;
                    return double.IsNaN(d1) ? double.IsNaN(d2) : double.IsInfinity(d1) ? double.IsInfinity(d2) : d1.Equals(d2);
                }
                else if (tb_f)
                {
                    f1 = (float)b;
                    d1 = (double)a;
                    return compare_float_double(f1, d1);
                }
            }
            else if (tb_f)
            {
                if (ta_f)
                {
                    f1 = (float)a;
                    f2 = (float)b;
                    return float.IsNaN(f1) ? float.IsNaN(f2) : float.IsInfinity(f1) ? float.IsInfinity(f2) : f1.Equals(f2);
                }
                else if (ta_d)
                {
                    f1 = (float)b;
                    d1 = (double)a;
                    return compare_float_double(f1, d1);
                }
            }
            else if (tb_d)
            {
                if (ta_d)
                {
                    d1 = (double)a;
                    d2 = (double)b;
                    return double.IsNaN(d1) ? double.IsNaN(d2) : double.IsInfinity(d1) ? double.IsInfinity(d2) : d1.Equals(d2);
                }
                else if (ta_f)
                {
                    f1 = (float)a;
                    d1 = (double)b;
                    return compare_float_double(f1, d1);
                }
            }
            if (t_f)
            {
                return floating_point_type_value_equals<float>(a, b, float.IsNaN, float.IsInfinity);
            }
            else if (t_d)
            {
                return floating_point_type_value_equals<double>(a, b, double.IsNaN, double.IsInfinity);
            }
            else
            {
                // we are not float nor double
                T TA = (T)Convert.ChangeType(a, typeof(T), CultureInfo.InvariantCulture);
                T TB = (T)Convert.ChangeType(b, typeof(T), CultureInfo.InvariantCulture);
                return TA.Equals(TB);
            }
        }

        static bool value_type_equals(object a, object b)
        {
            if (a == null || b == null)
            {
                return a == b;
            }

            if (a is sbyte || a is byte)
            {
                if (a is sbyte || a is byte)
                {
                    return promote_and_equals<sbyte>(a, b);
                }
                else if (b is short || b is ushort)
                {
                    return promote_and_equals<short>(a, b);
                }
                else if (b is int || b is uint)
                {
                    return promote_and_equals<int>(a, b);
                }
                else if (b is long || b is ulong)
                {
                    return promote_and_equals<long>(a, b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(a, b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is short || a is ushort)
            {
                if (b is sbyte || b is byte || b is short || b is ushort)
                {
                    return promote_and_equals<short>(a, b);
                }
                else if (b is int || b is uint)
                {
                    return promote_and_equals<int>(a, b);
                }
                else if (b is long || b is ulong)
                {
                    return promote_and_equals<long>(a, b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(a, b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is int || a is uint)
            {
                if (b is sbyte || b is byte || b is short || b is ushort || b is int || b is uint)
                {
                    return promote_and_equals<int>(a, b);
                }
                else if (b is long || b is ulong)
                {
                    return promote_and_equals<long>(a, b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(a, b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is long || a is ulong)
            {
                if (b is sbyte || b is byte || b is short || b is ushort || b is int || b is uint || b is long || b is ulong)
                {
                    return promote_and_equals<long>(a, b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(a, b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is float)
            {
                if (b is short || b is int || b is long || b is float)
                {
                    return promote_and_equals<float>(a, b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is double)
            {
                if (b is short || b is int || b is long || b is float || b is double)
                {
                    return promote_and_equals<double>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is char)
            {
                if (b is char)
                {
                    return promote_and_equals<char>(a, b);
                }
                else if (b is string)
                {
                    string s = (string)b;
                    return s.Length == 1 && s[0] == (char)a;
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else if (a is string)
            {
                if (b is char)
                {
                    string s = (string)a;
                    return s.Length == 1 && s[0] == (char)b;
                }
                else if (b is string)
                {
                    return promote_and_equals<string>(a, b);
                }
                else
                {
                    return a.Equals(b);
                }
            }
            else
            {
                return a.Equals(b);
            }
        }

        public static void ExpectEqual(object value, object expect, string message = null)
        {
            if (!value_type_equals(value, expect))
            {
                PRINT_FAIL(1,
                    "Expected the following values to be equal\n" +
                    ActualExpect(value, expect),
                    message);
            }
        }

        public static void AssertEqual(object value, object expect, string message = null)
        {
            if (!value_type_equals(value, expect))
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
            if (value_type_equals(value, expect))
            {
                PRINT_FAIL(1,
                    "Expected the following values to be not equal\n" +
                    ActualExpect(value, expect),
                    message);
            }
        }

        public static void AssertNotEqual(object value, object expect, string message = null)
        {
            if (value_type_equals(value, expect))
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

        public static void ExpectException<EXCEPTION>(Action method, string message = null)
            where EXCEPTION : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (Exception e)
            {
                RETHROW_EXCEPTION_IF_NEEDED(e);
                if (e.GetType() == typeof(EXCEPTION))
                {
                    return;
                }
                PRINT_FAIL(1,
                    "Expected the following Exception to be caught\n" +
                    ActualExpect(e.GetType().FullName, typeof(EXCEPTION).FullName),
                    message);
                throw;
            }
            PRINT_FAIL(1,
                "Expected the following Exception to be thrown\n" +
                typeof(EXCEPTION).FullName,
                message);
        }

        public static void AssertException<EXCEPTION>(Action method, string message = null)
            where EXCEPTION : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (Exception e)
            {
                RETHROW_EXCEPTION_IF_NEEDED(e);
                if (e.GetType() == typeof(EXCEPTION))
                {
                    return;
                }
                PRINT_FAIL(1,
                    "Expected the following Exception to be caught\n" +
                    ActualExpect(e.GetType().FullName, typeof(EXCEPTION).FullName),
                    message);
                throw;
            }
            PRINT_FAIL(1,
                "Expected the following Exception to be thrown\n" +
                typeof(EXCEPTION).FullName,
                message);
            throw new Exceptions.TEST_FAIL_EXCEPTION();
        }

        public static void ExpectNoException(Action method, string message = null)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception e)
            {
                RETHROW_EXCEPTION_IF_NEEDED(e);

                PRINT_FAIL(1, e, message);
            }
        }

        public static void AssertNoException(Action method, string message = null)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception e)
            {
                RETHROW_EXCEPTION_IF_NEEDED(e);
                throw new Exceptions.TEST_FAIL_EXCEPTION(e, message, new StackTrace(1, true));
            }
        }
    }
}
