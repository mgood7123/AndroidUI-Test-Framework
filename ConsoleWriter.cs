using System;
using System.Collections.Generic;

namespace AndroidUITestFramework
{
    public class ConsoleWriter : StringWriter
    {
        Stack<ConsoleColor> foregroundStack;
        Stack<ConsoleColor> backgroundStack;
        static bool modifyColors = true;
        bool wasTrue;
        static LinkedList<System.IO.TextWriter> CURRENT = new();


        public ConsoleWriter() : base()
        {
            foregroundStack = new Stack<ConsoleColor>();
            backgroundStack = new Stack<ConsoleColor>();
            foregroundStack.Push(Console.ForegroundColor);
            backgroundStack.Push(Console.BackgroundColor);
            wasTrue = false;
            if (CURRENT.Count == 0)
            {
                CURRENT.AddLast(Console.Out);
            }
            CURRENT.AddLast(this);
            Console.SetOut(this);
        }

        ~ConsoleWriter()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (CURRENT.Last.Value == this)
            {
                CURRENT.RemoveLast();
                Console.SetOut(CURRENT.Last.Value);
                Console.WriteLine("RESTORED OUT");
            }
            else
            {
                // we are not current, but we exist inside the CURRENT stack
                CURRENT.Remove(this);
            }
            base.Dispose(disposing);
        }

        public void pushForegroundColor(ConsoleColor color)
        {
            foregroundStack.Push(color);
        }

        public void popForegroundColor()
        {
            if (foregroundStack.Count > 1)
            {
                foregroundStack.Pop();
            }
        }

        public void pushBackgroundColor(ConsoleColor color)
        {
            backgroundStack.Push(color);
        }

        public void popBackgroundColor()
        {
            if (backgroundStack.Count > 1)
            {
                backgroundStack.Pop();
            }
        }

        public override void Flush()
        {
            ConsoleColor oldF = ConsoleColor.Gray;
            ConsoleColor oldB = ConsoleColor.Black;
            bool color_changed = false;
            if (modifyColors) {
                modifyColors = false;
                wasTrue = true;
                oldF = Console.ForegroundColor;
                oldB = Console.BackgroundColor;
                ConsoleColor newF = foregroundStack.Peek();
                ConsoleColor newB = backgroundStack.Peek();
                color_changed = oldF != newF || oldB != newB;
                if (color_changed) {
                    Console.ForegroundColor = newF;
                    Console.BackgroundColor = newB;
                }
            }
            Console.SetOut(CURRENT.Find(this).Previous.Value);
            Console.Write(base.ToString());
            if (color_changed)
            {
                Console.BackgroundColor = oldB;
                Console.ForegroundColor = oldF;
            }
            if (wasTrue)
            {
                wasTrue = false;
                modifyColors = true;
                Console.SetOut(this);
            }
            base.Flush();
        }
    }
}
