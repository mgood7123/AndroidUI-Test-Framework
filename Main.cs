using System;
using System.Collections.Generic;

namespace AndroidUITestFramework
{
    public sealed partial class Main
    {
        private static Type type = typeof(Test);
        private static Type typeGroup = typeof(TestGroup);
        private const string TOP_LEVEL_NAME = "Top Level";

        private static Type findAssignableBase(Type type, Type baseClass)
        {
            Type currentType = type?.DeclaringType;
            while (currentType != null)
            {
                if (baseClass.IsAssignableFrom(currentType))
                {
                    return currentType;
                }
                currentType = currentType.DeclaringType;
            }
            return null;
        }

        private static TestGroupInformation buildTopLevelTree(in List<Type> tests, in List<Type> groups, in TestGroupInformation parent)
        {
            TestGroupInformation root = parent ?? new();

            if (parent == null)
            {
                root.type = null;
            }

            // if there is no parent group then we are looking for types
            // that are NOT nested inside a TestGroup
            Type parentType = parent?.type;

            foreach (Type p in tests)
            {
                if (findAssignableBase(p, typeGroup) == parentType && type.IsAssignableFrom(p))
                {
                    root.tests.Add(p);
                }
            }

            // if there is no parent group then we are looking for typeGroups
            // that are NOT nested inside a TestGroup
            List<Type> rootGroups = new();
            foreach (Type p in groups)
            {
                if (findAssignableBase(p, typeGroup) == parentType && typeGroup.IsAssignableFrom(p))
                {
                    rootGroups.Add(p);
                }
            }

            foreach (Type group in rootGroups)
            {
                TestGroupInformation subGroup = new();
                subGroup.type = group;
                root.testGroups.Add(buildTopLevelTree(in tests, in groups, in subGroup));
            }
            return root;
        }

        private static TestGroupInformation DiscoverTests()
        {
            List<Type> types = new();
            var a1 = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var a in a1)
            {
                foreach (var p in a.DefinedTypes)
                {
                    if (p.IsClass && !p.IsAbstract && ( p != type && p != typeGroup))
                    {
                        types.Add(p);
                    }
                }
            }

            List<Type> tests = new();
            foreach (Type p in types)
            {
                if (type.IsAssignableFrom(p))
                {
                    tests.Add(p);
                }
            }

            List<Type> groups = new();
            foreach (Type p in types)
            {
                if (typeGroup.IsAssignableFrom(p))
                {
                    groups.Add(p);
                }
            }

            return buildTopLevelTree(tests, groups, null);
        }

        public static bool Run()
        {
            return Run(null, null);
        }

        public static bool Run(Type type)
        {
            return Run(type, null);
        }

        public static bool Run(string namespace_)
        {
            return Run(null, namespace_);
        }

        public static bool Run(Type type, string namespace_)
        {
            ConsoleWriter consoleWriter = new();
            Console.WriteLine("Starting AndroidUI Test Framework...");
            Console.WriteLine("Started AndroidUI Test Framework\n");

            Console.WriteLine("Discovering Tests...");
            var tests = DiscoverTests();
            if (tests == null)
            {
                Console.WriteLine("No tests found");
                return false;
            }
            var ns = tests.Filter(namespace_);
            if (ns == null)
            {
                if (namespace_ == null)
                {
                    Console.WriteLine("No tests found");
                    return false;
                }
                Console.WriteLine("No tests found in namespace '" + namespace_ + "'");
                return false;
            }
            TestGroupInformation root = ns.Find(type);
            if (root == null)
            {
                if (type == null)
                {
                    Console.WriteLine("No tests found");
                    return false;
                }
                if (namespace_ == null)
                {
                    Console.WriteLine("No tests found for type '" + type.Name + "'");
                    return false;
                }
                Console.WriteLine("No test of type '" + type.Name + "' found in namespace '" + namespace_ + "'");
                return false;
            }
            Console.WriteLine("Discovered Tests\n");

            Console.WriteLine("Printing Tests Heirarchy");
            root?.PrintTests(new ConsoleWriter());
            Console.WriteLine("Printed Tests Heirarchy\n");

            if (root != null)
            {
                Console.WriteLine("Running Tests in Alphaberical Order...");
                bool r = root.Run(true);
                Console.WriteLine("Ran Tests in Alphaberical Order");
                return r;
            }
            else return false;
        }
    }
}
