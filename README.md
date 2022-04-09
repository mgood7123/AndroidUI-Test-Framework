## AndroidUI Test Framework

a C# unit testing framework, based loosely off of GoogleTest

# Table Of Contents
<!--TOC-->
- [Color output](#color-output)
- [Main Method](#main-method)
- [Exception Handling](#exception-handling)
- [Assertions and Expectations](#assertions-and-expectations)
- [Tests and TestGroups](#tests-and-testgroups)
- [Nesting](#nesting)
- [Test inheritence](#test-inheritence)
- [why this framework?](#why-this-framework)
  - [NUnit](#nunit)
  - [MSTest](#mstest)
  - [XUnit](#xunit)
  - [AndroidUI Test Framework](#androidui-test-framework)
<!--/TOC-->

# Color output

![](Pictures/Screenshot%202022-03-31%20153513.png)
![](Pictures/Screenshot%202022-03-31%20171523.png)
![](Pictures/Screenshot%202022-03-31%20174017.png)

# Main Method

you can run test via `AndroidUITestFramework.Main.Run`

`AndroidUITestFramework.Main.Run` can be called as many times as you wish without affecting previous or future runs

* `AndroidUITestFramework.Main.Run()` runs all found tests
* `AndroidUITestFramework.Main.Run(nameof(a_namespace))` runs all found tests inside the namespace `a_namespace`
* `AndroidUITestFramework.Main.Run(typeof(test))` runs the specified Test or TestGroup
* `AndroidUITestFramework.Main.Run(typeof(test), nameof(a_namespace))` runs the specified Test if it is found inside of the specified namespace `a_namespace`

# Exception Handling

`AndroidUI Test Framework` provides a `RETHROW_EXCEPTION_IF_NEEDED(Exception e)` method to rethrow an exception if it is used internally by `AndroidUI Test Framework`

if you are catching `Exception` you should call this method last

```cs
try {
    throwableMethod();
}
catch (Exception e) {
    RETHROW_EXCEPTION_IF_NEEDED(e);
    handle_exception(e);
}
```

if a `Test` or `TestGroup` throws an uncaught exception `AndroidUI Test Framework` will catch this and fail the current test or group
```
    [Running        ] Test: UNHANDLED_TEST_EXCEPTION
        UNHANDLED EXCEPTION
        Exception Type: NullReferenceException
        Reason: Object reference not set to an instance of an object.
        Location:
           at Method: UNHANDLED_TEST_EXCEPTION.Run(TestGroup nullableInstance)
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:239
    [Running  FAILED] Test: UNHANDLED_TEST_EXCEPTION
```

# Assertions and Expectations

all `Assertions` and `Expectations` operate via throwing an exception internally used by `AndroidUI Test Framework`

these are located at `AndroidUITestFramework.Tools`

* SKIP()
* > skips the current Test or TestGroup, equivilant to a silent FAIL()
* > Stops execution of the current Test or TestGroup
* > can provide an optional message:
* > > Skip("Not supported by this platform")
* FAIL()
* > fails the current Test or TestGroup
* > Stops execution of the current Test or TestGroup
* > can provide an optional message:
* > > Fail("We should never get here!")
* ExpectTrue(bool expression)
* > fails the current Test or TestGroup if the given expression is not true
* > Continues execution of the current Test or TestGroup
* > can provide an optional message:
* > > ExpectTrue(i == 5-4, "Index has become corrupted")
* AssertTrue(bool expression)
* > fails the current Test or TestGroup if the given expression is not true
* > Stops execution of the current Test or TestGroup
* > can provide an optional message:
* > > AssertTrue(i == 5-4, "Index has become corrupted")
* ExpectFalse(bool expression)
* > fails the current Test or TestGroup if the given expression is not false
* > Continues execution of the current Test or TestGroup
* > can provide an optional message:
* > > ExpectFalse(str.Equals(INTERNAL), "the given string cannot have the same name as an INTERNAL string")
* AssertFalse(bool expression)
* > fails the current Test or TestGroup if the given expression is not false
* > Stops execution of the current Test or TestGroup
* > can provide an optional message:
* > > AssertFalse(str.Equals(INTERNAL), "the given string cannot have the same name as an INTERNAL string")
* ExpectEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect.Equals(value)` returns false
* > Continues execution of the current Test or TestGroup
* > Does automatic type promotion of short, int, long, float, double
* > Does automatic conversion of char to string
* > can provide an optional message:
* > > ExpectEqual(str, str2, "the given strings do not match")
* AssertEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect.Equals(value)` returns false
* > Stops execution of the current Test or TestGroup
* > Does automatic type promotion of short, int, long, float, double
* > Does automatic conversion of char to string
* > can provide an optional message:
* > > AssertEqual(str, str2, "the given strings do not match")
* ExpectNotEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect.Equals(value)` returns true
* > Continues execution of the current Test or TestGroup
* > Does automatic type promotion of short, int, long, float, double
* > Does automatic conversion of char to string
* > can provide an optional message:
* > > ExpectNotEqual(str, INTERNAL, "the given string cannot have the same name as an INTERNAL string")
* AssertNotEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect.Equals(value)` returns true
* > Stops execution of the current Test or TestGroup
* > Does automatic type promotion of short, int, long, float, double
* > Does automatic conversion of char to string
* > can provide an optional message:
* > > AssertNotEqual(str, INTERNAL, "the given string cannot have the same name as an INTERNAL string")
* ExpectInstanceEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect != value` returns true
* > Continues execution of the current Test or TestGroup
* > can provide an optional message:
* > > ExpectInstanceEqual(str, str2, "the given strings are not the same")
* AssertInstanceEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect != value` returns true
* > Stops execution of the current Test or TestGroup
* > can provide an optional message:
* > > AssertInstanceEqual(str, str2, "the given strings are not the same")
* ExpectInstanceEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect == value` returns true
* > Continues execution of the current Test or TestGroup
* > can provide an optional message:
* > > ExpectInstanceNotEqual(value, INTERNAL, "somehow managed to obtain an instance of an INTERNAL string")
* AssertInstanceNotEqual(object value, object expect)
* > fails the current Test or TestGroup if the result of `expect == value` returns true
* > Stops execution of the current Test or TestGroup
* > can provide an optional message:
* > > AssertInstanceNotEqual(value, INTERNAL, "somehow managed to obtain an instance of an INTERNAL string")
* ExpectException&lt;EXCEPTION&gt;(Action method)
* > fails the current Test or TestGroup if the `Exception` of type `EXCEPTION` was not thrown or a different `Exception` was caught
* > Continues execution of the current Test or TestGroup if an `Exception` was not thrown, otherwise `rethrows the caught exception`
* > can provide an optional message:
* > > ExpectException<FATAL_EXCEPTION>(() => my_method() }, "expected my_method to throw FATAL_EXCEPTION")
* AssertException&lt;EXCEPTION&gt;(Action method)
* > fails the current Test or TestGroup if the `Exception` of type `EXCEPTION` was not thrown or a different `Exception` was caught
* > stops execution of the current Test or TestGroup if an `Exception` was not thrown, otherwise `rethrows the caught exception`
* > can provide an optional message:
* > > AssertException<FATAL_EXCEPTION>(() => my_method() }, "expected my_method to throw FATAL_EXCEPTION")

# Tests and TestGroups

a `Test` is a class extending from `AndroidUITestFramework.Test`

```cs
class a_test : Test
{
    public override void Run(TestGroup nullableInstance)
    {
        Console.WriteLine("I AM A TEST");
    }
}
```

a `TestGroup` is a class extending from `AndroidUITestFramework.TestGroup`

```cs
public class a_test_group : TestGroup
{
    public override void OnCreate()
    {
        // called before a test is ran
    }

    public override void OnDestroy()
    {
        // called after a test is ran
    }

    // tests for this group here
    class a_test : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("I AM A TEST INSIDE OF A TEST GROUP");
        }
    }
}
```

a `Tests` `nullableInstance` will be set to the instance of the `TestGroup` it belongs to

# Nesting

`AndroidUI Test Framework` tries very hard to find all your tests where ever they may be

a `Tests` and `TestGroups` can be `nested` inside of any `TestGroup` either directly or indirectly

```cs
public class a_test_group : TestGroup
{
    public override void OnCreate()
    {
        // called before a test is ran
    }

    public override void OnDestroy()
    {
        // called after a test is ran
    }

    class foo {
        class a_test : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Console.WriteLine("I AM A TEST INSIDE OF A TEST GROUP AND I AM STILL FOUND");
            }
        }
    }

    class sub {
        class group {
            class we_are_nested {
                public class a_test_group : TestGroup
                {
                    public override void OnCreate()
                    {
                        // called before a test is ran
                        Console.WriteLine("I AM A TEST GROUP INSIDE OF A TEST GROUP AND I AM STILL FOUND");
                    }

                    public override void OnDestroy()
                    {
                        // called after a test is ran
                    }

                    class foo {
                        class a_test : Test
                        {
                            public override void Run(TestGroup nullableInstance)
                            {
                                Console.WriteLine("I AM A TEST INSIDE OF A TEST GROUP INSIDE OF ANOTHER TEST GROUP AND I AM STILL FOUND");
                            }
                        }
                    }
                }
            }
        }
    }
}
```

the above produces the following output:
```
produces the following output:

Printing Tests Heirarchy
+Top Level
  No Tests Have Been Declared By 'Top Level'
  +a_test_group
    a_test
    +a_test_group
      a_test
      No Test Groups Have Been Declared By 'a_test_group'
Printed Tests Heirarchy

Running Tests in Alphaberical Order...
[Running        ] Group: Top Level

    [Running        ] Group: a_test_group
        [Running        ] Test: a_test
            I AM A TEST INSIDE OF A TEST GROUP AND I AM STILL FOUND
        [Running      OK] Test: a_test

        [Running        ] Group: a_test_group.a_test_group
            I AM A TEST GROUP INSIDE OF A TEST GROUP AND I AM STILL FOUND
            [Running        ] Test: a_test
                I AM A TEST INSIDE OF A TEST GROUP INSIDE OF ANOTHER TEST GROUP AND I AM STILL FOUND
            [Running      OK] Test: a_test
        [Running      OK] Group: a_test_group.a_test_group

    [Running      OK] Group: a_test_group

[Running      OK] Group: Top Level
Ran Tests in Alphaberical Order
```

# Test inheritence

a `Test` can inherit another `Test`

```cs
class BASE_TEST : AndroidUITestFramework.Test
{
    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
    {
        Console.WriteLine("TEST! 1");
    }
}

class inherited_test : BASE_TEST
{
    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
    {
        base.Run(nullableInstance);
        Console.WriteLine("TEST! INHERITED");
    }
}
```

output:
```
    [Running        ] Test: BASE_TEST
        TEST! 1
    [Running      OK] Test: BASE_TEST
    [Running        ] Test: inherited_test
        TEST! 1
        TEST! INHERITED
    [Running      OK] Test: inherited_test
```

likewise a `TestGroup` can also inherit a `TestGroup`

in most cases it will work fine provided:
* the test is part of the same test tree and groups inherit from parent group

example
```cs
class g : TestGroup 
{
    int data;
    class s : g
    {
        class Data : Test
        {
            g data;
            public override void Run(TestGroup nullableInstance)
            {
                // set our instance of data to our parent `g`
                // this is possible as `s` inherits `g`
                //
                // we may also be able to set `g.data` directly
                // but this will COPY if `g.data` is a VALUE TYPE
                data = (g)nullableInstance;
            }
        }
        class n : g
        {
            class test : Data
            {
                public override void Run(TestGroup nullableInstance)
                {
                    // this works because `n` inherits `g`
                    // which `Data` expects to recieve as its instance
                    base.Run(nullableInstance);
                    // we can quickly access `data` which is of type `g`
                    data.data = 5;
                }
            }
        }
    }
}

```

# why this framework?

have a look at https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/


|Description|NUnit|MSTest|xUnit|AndroidUI Test Framework|
|-|-|-|-|-|
|Marks a test method/individual test|[Test]|[TestMethod]|[Fact]|class t : Test|
|Indicates that a class has a group of unit tests|[TestFixture]|[TestClass]|N.A|class g : TestGroup|
|Contains the initialization code, which is triggered before every test case|[SetUp]|[TestInitialize]|Constructor|OnCreate|
|Contains the cleanup code, which is triggered after every test case|[TearDown]|[TestCleanup]|IDisposable.Dispose|OnDestroy|
|Contains method that is triggered once before test cases start|[OneTimeSetUp]|[ClassInitialize]|IClassFixture<T>|N.A|
|Contains method that is triggered once before test cases end|[OneTimeTearDown]|[ClassCleanup]|IClassFixture<T>|N.A|
|Contains per-collection fixture setup and teardown|N.A|N.A|ICollectionFixture<T>|N.A|
|Ignores a test case|[Ignore(“reason”)]|[Ignore]|[Fact(Skip=”reason”)]|SKIP()|
|Categorize test cases or classes|[Category()]|[TestCategory(“)]|[Trait(“Category”, “”)|N.A|
|Identifies a method that needs to be called before executing any test in test class/test fixture|[TestFixtureSetup]|[ClassInitialize]|N.A|N.A|
|Identifies a method that needs to be called after executing any test in test class/test fixture|[TestFixtureTearDown]|[ClassCleanUp]|N.A|N.A|
|Identifies a method that needs to be called before the execution of any tests in Test Assembly|N.A|[AssemblyInitialize]|N.A|N.A|
|Identifies a method that needs to be called after execution of tests in Test Assembly|N.A|[AssemblyCleanUp]|N.A|N.A|


<br></br>
<br></br>

`AndroidUI Test Framework` is much simpler than `NUnit`, `XUnit`, and `MSTest`

however `AndroidUI Test Framework` makes up for this with easy-to-use tests, and detailed output

for comparison, we will use tests in all frameworks 4 frameworks

## NUnit
TEST:
```cs
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
 
namespace NUnit_Test
{
    class NUnit_Demo
    {
        [SetUp]
        public void Initialize()
        {
            Console.WriteLine("Inside SetUp");
        }
 
        [TearDown]
        public void DeInitialize()
        {
            Console.WriteLine("Inside TearDown");
        }
 
        public class TestClass1
        {
            [OneTimeSetUp]
            public static void ClassInitialize()
            {
                Console.WriteLine("Inside OneTimeSetUp");
            }
 
            [OneTimeTearDown]
            public static void ClassCleanup()
            {
                Console.WriteLine("Inside OneTimeTearDown");
            }
        }
 
        [Test, Order(1)]
        public void Test_1()
        {
            Console.WriteLine("Inside TestMethod Test_1");
        }
 
        [Test, Order(2)]
        public void Test_2()
        {
            Console.WriteLine("Inside TestMethod Test_2");
        }
    }
}
```
OUTPUT:
```
Inside SetUp
Inside TestMethod Test_1
Inside TearDown
 
Inside SetUp
Inside TestMethod Test_2
Inside TearDown
```

## MSTest
TEST:
```cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
 
namespace MsTest
{
    [TestClass]
    public class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Console.WriteLine("Inside AssemblyInitialize");
        }
    }
 
    public class DeInitialize
    {
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("Inside AssemblyCleanup");
        }
    }
 
    [TestClass]
    public class TestClass1
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("Inside ClassInitialize");
        }
 
        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine("Inside ClassCleanup");
        }
 
        [TestMethod]
        public void Test_1()
        {
            Console.WriteLine("Inside TestMethod Test_1");
        }
    }
 
    [TestClass]
    public class TestClass2
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("Inside TestInitialize");
        }
 
        [TestMethod]
        public void Test_2()
        {
            Console.WriteLine("Inside TestMethod Test_2");
        }
 
        [TestCleanup]
        public void TestCleanup()
        {
            Console.WriteLine("Inside TestCleanup");
        }
    }
}
```
OUTPUT:
```
Inside ClassInitialize
Inside TestMethod Test_1
Inside AssemblyInitialize
 
Inside TestInitialize
Inside TestMethod Test_2
Inside TestCleanup
```

## XUnit
TEST:
```cs
using System;
using Xunit;
 
namespace xUnit_Test
{
    public class xUnit_Tests : IDisposable
    {
        public xUnit_Tests()
        {
            Console.WriteLine("Inside SetUp Constructor");
        }
 
        public void Dispose()
        {
            Console.WriteLine("Inside CleanUp or Dispose method");
        }
    }
 
    public class UnitTest_1 : IClassFixture<xUnit_Tests>
    {
        [Fact]
        public void Test_1()
        {
            Console.WriteLine("Inside Test_1");
        }
    }
    public class UnitTest_2 : IClassFixture<xUnit_Tests>
    {
        [Fact]
        public void Test_2()
        {
            Console.WriteLine("Inside Test_2");
        }
    }
}
```
OUTPUT:
```
Inside SetUp Constructor
Inside Test_1
 
Inside Test_2
Inside CleanUp or Dispose method
```

## AndroidUI Test Framework
TEST:
```cs
using AndroidUITestFramework;

namespace AndroidUITestFramework_Test
{
    public class AndroidUITestFramework_Tests : TestGroup
    {
        public override void OnCreate()
        {
            Console.WriteLine("Inside OnCreate");
        }

        public override void OnDestroy()
        {
            Console.WriteLine("Inside OnDestroy");
        }

        public class UnitTest_1 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Console.WriteLine("Inside Test_1");
            }
        }

        public class UnitTest_2 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Console.WriteLine("Inside Test_2");
            }
        }
    }
}
```
OUTPUT:
```
Starting AndroidUI Test Framework...
Started AndroidUI Test Framework

Discovering Tests...
Discovered Tests

Printing Tests Heirarchy
+Top Level
  No Tests Have Been Declared By 'Top Level'
  +AndroidUITestFramework_Tests
    UnitTest_1
    UnitTest_2
    No Test Groups Have Been Declared By 'AndroidUITestFramework_Tests'
Printed Tests Heirarchy

Running Tests in Alphaberical Order...
[Running        ] Group: Top Level

    [Running        ] Group: AndroidUITestFramework_Tests
        Inside OnCreate
        [Running        ] Test: UnitTest_1
            Inside Test_1
        [Running      OK] Test: UnitTest_1
        Inside OnDestroy
        Inside OnCreate
        [Running        ] Test: UnitTest_2
            Inside Test_2
        [Running      OK] Test: UnitTest_2
        Inside OnDestroy
    [Running      OK] Group: AndroidUITestFramework_Tests

[Running      OK] Group: Top Level
Ran Tests in Alphaberical Order

D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\bin\Debug\net6.0\AndroidUITest.exe (process 22372) exited with code 0.
Press any key to close this window . . .
```

<br></br>
as you can see above, `AndroidUI Test Framework` prints detail info about the test heirarchy and tests being run and their status

below is a sample of our capabilities

```
Starting AndroidUI Test Framework...
Started AndroidUI Test Framework

Discovering Tests...
Discovered Tests

Printing Tests Heirarchy
+Top Level
  BASE_TEST
  inherited_test
  EXPECTATIONS
  B_TEST
  UNHANDLED_TEST_EXCEPTION
  B_TEST
  +Group
    ASSERTION
    TEST_INSIDE_ANOTHER_CLASS
    +GROUP_INSIDE_ANOTHER_CLASS
      HOTDOG
      No Test Groups Have Been Declared By 'GROUP_INSIDE_ANOTHER_CLASS'
  +PrintGroup
    PrintTest
    SkipTest
    No Test Groups Have Been Declared By 'PrintGroup'
  +SkipGroup
    SkipTest
    No Test Groups Have Been Declared By 'SkipGroup'
  +FailGroup
    Test
    No Test Groups Have Been Declared By 'FailGroup'
  +FailGroup2
    Test
    No Test Groups Have Been Declared By 'FailGroup2'
  +UNHANDLED_GROUP_EXCEPTION_CREATE
    UNHANDLED_TEST_EXCEPTION
    No Test Groups Have Been Declared By 'UNHANDLED_GROUP_EXCEPTION_CREATE'
  +UNHANDLED_GROUP_EXCEPTION_DESTROY
    UNHANDLED_TEST_EXCEPTION
    No Test Groups Have Been Declared By 'UNHANDLED_GROUP_EXCEPTION_DESTROY'
  +UNHANDLED_GROUP_EXCEPTION_DESTROY2
    UNHANDLED_TEST_EXCEPTION
    No Test Groups Have Been Declared By 'UNHANDLED_GROUP_EXCEPTION_DESTROY2'
  +UNHANDLED_GROUP_EXCEPTION_2
    UNHANDLED_TEST_EXCEPTION_1
    UNHANDLED_TEST_EXCEPTION_2
    No Test Groups Have Been Declared By 'UNHANDLED_GROUP_EXCEPTION_2'
  +a_test_group
    a_test
    +a_test_group
      a_test
      No Test Groups Have Been Declared By 'a_test_group'
  +AndroidUITestFramework_Tests
    UnitTest_1
    UnitTest_2
    No Test Groups Have Been Declared By 'AndroidUITestFramework_Tests'
Printed Tests Heirarchy

Running Tests in Alphaberical Order...
[Running        ] Group: Top Level
    [Running        ] Test: B_TEST
        TEST!
    [Running      OK] Test: B_TEST
    [Running        ] Test: B_TEST
        TEST! INSIDE NAMESPACE
    [Running      OK] Test: B_TEST
    [Running        ] Test: BASE_TEST
        TEST! 1
    [Running      OK] Test: BASE_TEST
    [Running        ] Test: EXPECTATIONS
        EXPECTATIONS!
        Test Failed
        Reason: Expected the expression to be false, it was true
        Message: <No Message Given>
        Location:
           at Method: EXPECTATIONS.Run(TestGroup nullableInstance)
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:60
        Test Failed
        Reason: Expected the expression to be true, it was false
        Message: <No Message Given>
        Location:
           at Method: EXPECTATIONS.Run(TestGroup nullableInstance)
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:61
        Test Failed
        Reason: Expected the following instances to be equal
              Actual: EXPECTATIONS
            Expected: <null>
        Message: <No Message Given>
        Location:
           at Method: EXPECTATIONS.Run(TestGroup nullableInstance)
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:62
    [Running  FAILED] Test: EXPECTATIONS
    [Running        ] Test: inherited_test
        TEST! 1
        TEST! INHERITED
    [Running      OK] Test: inherited_test
    [Running        ] Test: UNHANDLED_TEST_EXCEPTION
        UNHANDLED EXCEPTION
        Exception Type: NullReferenceException
        Reason: Object reference not set to an instance of an object.
        Location:
           at Method: UNHANDLED_TEST_EXCEPTION.Run(TestGroup nullableInstance)
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:240
    [Running  FAILED] Test: UNHANDLED_TEST_EXCEPTION

    [Running        ] Group: Group
        [Running        ] Test: ASSERTION
            ASSERTION!
            Test Failed
            Reason: Expected the expression to be false, it was true
            Message: <No Message Given>
            Location:
               at Method: Group.ASSERTION.Run(TestGroup nullableInstance)
                 in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:10
        [Running  FAILED] Test: ASSERTION
        [Running        ] Test: TEST_INSIDE_ANOTHER_CLASS
            TEST! 3
        [Running      OK] Test: TEST_INSIDE_ANOTHER_CLASS

        [Running        ] Group: Group.GROUP_INSIDE_ANOTHER_CLASS
            [Running        ] Test: HOTDOG
                TEST! 4
            [Running      OK] Test: HOTDOG
        [Running      OK] Group: Group.GROUP_INSIDE_ANOTHER_CLASS

    [Running  FAILED] Group: Group

    [Running        ] Group: PrintGroup
        TEST GROUP CREATE
        [Running        ] Test: PrintTest
            TEST
        [Running      OK] Test: PrintTest
        TEST GROUP DESTROY
        TEST GROUP CREATE
        [Running        ] Test: SkipTest
        [Running SKIPPED] Test: SkipTest
        TEST GROUP DESTROY
    [Running      OK] Group: PrintGroup

    [Running        ] Group: SkipGroup
    [Running SKIPPED] Group: SkipGroup

    [Running        ] Group: FailGroup
        TEST GROUP CREATE
        Test Failed
        Reason: <No Reason Given>
        Message: <No Message Given>
        Location:
           at Method: FailGroup.OnCreate()
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:142
    [Running  FAILED] Group: FailGroup

    [Running        ] Group: FailGroup2
        TEST GROUP CREATE
        [Running        ] Test: Test
            TEST
        [Running      OK] Test: Test
        TEST GROUP DESTROY
        Test Failed
        Reason: <No Reason Given>
        Message: <No Message Given>
        Location:
           at Method: FailGroup2.OnDestroy()
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:169
    [Running  FAILED] Group: FailGroup2

    [Running        ] Group: UNHANDLED_GROUP_EXCEPTION_CREATE
        UNHANDLED EXCEPTION
        Exception Type: NullReferenceException
        Reason: Object reference not set to an instance of an object.
        Location:
           at Method: UNHANDLED_GROUP_EXCEPTION_CREATE.OnCreate()
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:187
    [Running  FAILED] Group: UNHANDLED_GROUP_EXCEPTION_CREATE

    [Running        ] Group: UNHANDLED_GROUP_EXCEPTION_DESTROY
        [Running        ] Test: UNHANDLED_TEST_EXCEPTION
        [Running      OK] Test: UNHANDLED_TEST_EXCEPTION
        UNHANDLED EXCEPTION
        Exception Type: NullReferenceException
        Reason: Object reference not set to an instance of an object.
        Location:
           at Method: UNHANDLED_GROUP_EXCEPTION_DESTROY.OnDestroy()
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:205
    [Running  FAILED] Group: UNHANDLED_GROUP_EXCEPTION_DESTROY

    [Running        ] Group: UNHANDLED_GROUP_EXCEPTION_DESTROY2
        [Running        ] Test: UNHANDLED_TEST_EXCEPTION
            UNHANDLED EXCEPTION
            Exception Type: NullReferenceException
            Reason: Object reference not set to an instance of an object.
            Location:
               at Method: UNHANDLED_GROUP_EXCEPTION_DESTROY2.UNHANDLED_TEST_EXCEPTION.Run(TestGroup nullableInstance)
                 in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:230
        [Running  FAILED] Test: UNHANDLED_TEST_EXCEPTION
        UNHANDLED EXCEPTION
        Exception Type: NullReferenceException
        Reason: Object reference not set to an instance of an object.
        Location:
           at Method: UNHANDLED_GROUP_EXCEPTION_DESTROY2.OnDestroy()
             in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:222
    [Running  FAILED] Group: UNHANDLED_GROUP_EXCEPTION_DESTROY2

    [Running        ] Group: UNHANDLED_GROUP_EXCEPTION_2
        [Running        ] Test: UNHANDLED_TEST_EXCEPTION_1
            UNHANDLED EXCEPTION
            Exception Type: NullReferenceException
            Reason: Object reference not set to an instance of an object.
            Location:
               at Method: UNHANDLED_GROUP_EXCEPTION_2.UNHANDLED_TEST_EXCEPTION_1.Run(TestGroup nullableInstance)
                 in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:251
        [Running  FAILED] Test: UNHANDLED_TEST_EXCEPTION_1
        [Running        ] Test: UNHANDLED_TEST_EXCEPTION_2
            UNHANDLED EXCEPTION
            Exception Type: NullReferenceException
            Reason: Object reference not set to an instance of an object.
            Location:
               at Method: UNHANDLED_GROUP_EXCEPTION_2.UNHANDLED_TEST_EXCEPTION_2.Run(TestGroup nullableInstance)
                 in Location:  D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\A_TEST.cs:260
        [Running  FAILED] Test: UNHANDLED_TEST_EXCEPTION_2
    [Running  FAILED] Group: UNHANDLED_GROUP_EXCEPTION_2

    [Running        ] Group: a_test_group
        [Running        ] Test: a_test
            I AM A TEST INSIDE OF A TEST GROUP AND I AM STILL FOUND
        [Running      OK] Test: a_test

        [Running        ] Group: a_test_group.a_test_group
            I AM A TEST GROUP INSIDE OF A TEST GROUP AND I AM STILL FOUND
            [Running        ] Test: a_test
                I AM A TEST INSIDE OF A TEST GROUP INSIDE OF ANOTHER TEST GROUP AND I AM STILL FOUND
            [Running      OK] Test: a_test
        [Running      OK] Group: a_test_group.a_test_group

    [Running      OK] Group: a_test_group

    [Running        ] Group: AndroidUITestFramework_Tests
        Inside OnCreate
        [Running        ] Test: UnitTest_1
            Inside Test_1
        [Running      OK] Test: UnitTest_1
        Inside OnDestroy
        Inside OnCreate
        [Running        ] Test: UnitTest_2
            Inside Test_2
        [Running      OK] Test: UnitTest_2
        Inside OnDestroy
    [Running      OK] Group: AndroidUITestFramework_Tests

[Running  FAILED] Group: Top Level
Ran Tests in Alphaberical Order

D:\IMPORTANT\source\repos\WindowsProject1\AndroidUITest\bin\Debug\net6.0\AndroidUITest.exe (process 27852) exited with code 0.
Press any key to close this window . . .
```