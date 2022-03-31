﻿using System;
using System.Collections.Generic;

namespace AndroidUITestFramework
{
    public sealed partial class Main
    {
        internal sealed class TestGroupInformation
        {
            internal Type type;
            internal List<Type> tests = new();
            internal List<TestGroupInformation> testGroups = new();
            internal Type TEST_TARGET;

            internal sealed class TestInfo {
                internal Test current;
                internal bool failed = false;

                public TestInfo(Test current)
                {
                    this.current = current;
                }
            }

            internal static TestInfo CURRENT_TEST = null;

            public void PrintTests(in ConsoleWriter writer)
            {
                bool HAS_TARGET_TEST = TEST_TARGET != null;
                bool TARGET_TEST_IS_US = TEST_TARGET == type;

                writer.pushIndentSize(1);
                string name = type == null ? TOP_LEVEL_NAME : type.Name;
                writer.WriteLine("+" + name);
                writer.pushIndent();
                writer.pushIndent();
                if (tests.Count == 0)
                {
                    writer.pushForegroundColor(ConsoleColor.DarkYellow);
                    writer.WriteLine("No Tests Have Been Declared By '" + name + "'");
                    writer.popForegroundColor();
                }
                else
                {
                    foreach (Type test in tests)
                    {
                        if (!HAS_TARGET_TEST || TARGET_TEST_IS_US || TEST_TARGET == test)
                        {
                            writer.WriteLine(test.Name);
                        }
                    }
                }
                if (testGroups.Count == 0)
                {
                    writer.pushForegroundColor(ConsoleColor.DarkYellow);
                    writer.WriteLine("No Test Groups Have Been Declared By '" + name + "'");
                    writer.popForegroundColor();
                }
                else
                {
                    if (HAS_TARGET_TEST && !TARGET_TEST_IS_US)
                    {
                        writer.pushForegroundColor(ConsoleColor.DarkYellow);
                        writer.WriteLine("No Test Groups Are Being Shown By '" + name + "' Because We Are Targeting A Specific Test");
                        writer.popForegroundColor();
                    }
                    else
                    {
                        foreach (TestGroupInformation testGroup in testGroups)
                        {
                            testGroup.PrintTests(writer);
                        }
                    }
                }
                writer.popIndent();
                writer.popIndent();
                writer.popIndentSize();
            }

            public TestGroupInformation Filter(string namespace_)
            {
                if (namespace_ == null)
                {
                    return this;
                }
                TestGroupInformation tmp = new();
                if (type != null && type.Namespace == namespace_)
                {
                    tmp.type = type;
                }
                // the target could be one of our tests
                foreach (Type type in tests)
                {
                    if (type != null && type.Namespace == namespace_)
                    {
                        tmp.tests.Add(type);
                    }
                }
                // we do not directly contain the target
                foreach (TestGroupInformation group in testGroups)
                {
                    TestGroupInformation filtered = group.Filter(namespace_);
                    if (filtered != null)
                    {
                        tmp.testGroups.Add(filtered);
                    }
                }
                return (tmp.tests.Count == 0 && tmp.testGroups.Count == 0) ? null : tmp;
            }

            public TestGroupInformation Find(Type target)
            {
                TEST_TARGET = null;
                if (target == null || type == target)
                {
                    TEST_TARGET = type;
                    return this;
                }
                // the target could be one of our tests
                if (tests.Contains(target))
                {
                    // the target is one of our tests, run it
                    TEST_TARGET = target;
                    return this;
                }
                // we do not directly contain the target
                foreach (TestGroupInformation group in testGroups)
                {
                    TestGroupInformation found = group.Find(target);
                    if (found != null)
                    {
                        // we found it
                        return found;
                    }
                }
                return this;
            }

            public bool Run(bool shortAlphabetically = false, string PARENT_GROUP = null)
            {
                bool HAS_TARGET_TEST = TEST_TARGET != null;
                bool TARGET_TEST_IS_US = TEST_TARGET == type;

                List<Type> t;
                if (shortAlphabetically)
                {
                    t = new List<Type>(tests);
                    t.Sort((Type a, Type b) => string.Compare(a.Name, b.Name));
                }
                else
                {
                    t = tests;
                }

                string GROUP_NAME = "";
                string PARENT_GROUP_NAME = "";
                if (type == null)
                {
                    GROUP_NAME = TOP_LEVEL_NAME;
                }
                else
                {
                    GROUP_NAME = type.Name;
                    if (PARENT_GROUP != null)
                    {
                        PARENT_GROUP_NAME = PARENT_GROUP + ".";
                    }
                }

                ConsoleWriter console = new();

                Console.SetOut(console);

                console.pushForegroundColor(ConsoleColor.Green);
                console.WriteLine("[Running        ] Group: " + PARENT_GROUP_NAME + GROUP_NAME);
                console.popForegroundColor();

                console.pushIndent();

                bool group_failed = false;
                bool group_skipped = false;
                bool test_failed_inside_group = false;

                foreach (Type test in t)
                {
                    if (!HAS_TARGET_TEST || TARGET_TEST_IS_US || TEST_TARGET == test)
                    {
                        string TEST_NAME = test.Name;

                        TestGroup testGroupInstance = null;

                        if (!group_skipped)
                        {
                            if (type != null)
                            {
                                testGroupInstance = (TestGroup)Activator.CreateInstance(type);
                                try
                                {
                                    testGroupInstance.OnCreate();
                                }
                                catch (Exceptions.TEST_FAIL_EXCEPTION)
                                {
                                    group_failed = true;
                                }
                                catch (Exceptions.TEST_SKIPPED_EXCEPTION)
                                {
                                    group_skipped = true;
                                }
                                catch (Exception e)
                                {
                                    group_failed = true;
                                    Console.WriteLine("UNHANDLED EXCEPTION:\n" + e.ToString());
                                }
                            }
                        }
                        if (!group_skipped && !group_failed)
                        {
                            console.pushForegroundColor(ConsoleColor.Green);
                            console.WriteLine("[Running        ] Test: " + TEST_NAME);
                            console.popForegroundColor();

                            console.pushIndent();

                            CURRENT_TEST = new((Test)Activator.CreateInstance(test));

                            bool test_failed = false;
                            bool test_skipped = false;
                            try
                            {
                                CURRENT_TEST.current.Run(testGroupInstance);
                            }
                            catch (Exceptions.TEST_FAIL_EXCEPTION)
                            {
                                test_failed = true;
                            }
                            catch (Exceptions.TEST_SKIPPED_EXCEPTION)
                            {
                                test_skipped = true;
                            }
                            catch (Exception e)
                            {
                                test_failed = true;
                                Console.WriteLine("UNHANDLED EXCEPTION:\n" + e.ToString());
                            }

                            if (!test_skipped)
                            {
                                if (CURRENT_TEST.failed)
                                {
                                    test_failed = true;
                                }
                            }
                            CURRENT_TEST = null;

                            console.popIndent();
                            if (!test_skipped)
                            {
                                if (test_failed)
                                {
                                    console.pushForegroundColor(ConsoleColor.Red);
                                    console.WriteLine("[Running  FAILED] Test: " + TEST_NAME);
                                }
                                else
                                {
                                    console.pushForegroundColor(ConsoleColor.Green);
                                    console.WriteLine("[Running      OK] Test: " + TEST_NAME);
                                }
                            }
                            else
                            {
                                console.pushForegroundColor(ConsoleColor.Green);
                                console.WriteLine("[Running SKIPPED] Test: " + TEST_NAME);
                            }
                            console.popForegroundColor();
                            if (test_failed)
                            {
                                test_failed_inside_group = true;
                            }
                        }
                        if (!group_skipped && !group_failed)
                        {
                            if (testGroupInstance != null)
                            {
                                try
                                {
                                    testGroupInstance.OnDestroy();
                                }
                                catch (Exceptions.TEST_FAIL_EXCEPTION)
                                {
                                    group_failed = true;
                                }
                                catch (Exceptions.TEST_SKIPPED_EXCEPTION)
                                {
                                }
                                catch (Exception e)
                                {
                                    group_failed = true;
                                    Console.WriteLine("UNHANDLED EXCEPTION:\n" + e.ToString());
                                }
                            }
                        }
                    }
                }

                if (!HAS_TARGET_TEST || TARGET_TEST_IS_US)
                {
                    foreach (TestGroupInformation group in testGroups)
                    {
                        console.WriteLine();
                        if (!group.Run(shortAlphabetically, type == null ? null : GROUP_NAME))
                        {
                            group_failed = true;
                        }
                    }
                    if (testGroups.Count != 0)
                    {
                        console.WriteLine();
                    }
                }

                console.popIndent();

                if (!group_skipped)
                {
                    if (group_failed || test_failed_inside_group)
                    {
                        console.pushForegroundColor(ConsoleColor.Red);
                        console.WriteLine("[Running  FAILED] Group: " + PARENT_GROUP_NAME + GROUP_NAME);
                    }
                    else
                    {
                        console.pushForegroundColor(ConsoleColor.Green);
                        console.WriteLine("[Running      OK] Group: " + PARENT_GROUP_NAME + GROUP_NAME);
                    }
                }
                else
                {
                    console.pushForegroundColor(ConsoleColor.Green);
                    console.WriteLine("[Running SKIPPED] Group: " + PARENT_GROUP_NAME + GROUP_NAME);
                }
                console.popForegroundColor();
                TEST_TARGET = null;
                return !group_failed;
            }
        }
    }
}
