using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit
{
    public delegate void Check<T>(T checkObject);
    public partial class AssertCollection
    {
        public static void CollectionHasAny<T>(IEnumerable<T> collection, params Check<T>[] checks)
        {
            CollectionHasAny(collection, new NullOutputHelper(), checks);
        }

        public static void CollectionHasAny<T>(IEnumerable<T> collection, params NamedCheck<T>[] checks)
        {
            CollectionHasAny(collection, new NullOutputHelper(), checks);
        }

        public static void CollectionHasAny<T>(IEnumerable<T> collection, ITestOutputHelper output, params Check<T>[] checks)
        {
            int i = 1;
            IEnumerable<NamedCheck<T>> namedchecks = checks.Select(c => new NamedCheck<T>($"Check {i++}", c));
            CollectionHasAny(collection, output, namedchecks.ToArray());
        }
        public static void CollectionHasAny<T>(IEnumerable<T> collection, ITestOutputHelper output, params NamedCheck<T>[] checks)
        {
            output = output ?? new NullOutputHelper();

            foreach(var check in checks)
            {
                output.WriteLine($"Running {check.Name}");
                bool failed = true;
                XunitException lastError = null;
                int i = 1;
                foreach(var entry in collection)
                {
                    try
                    {
                        output.WriteLine($"  Checking element {i}");
                        check.Check(entry);
                        failed = false;
                        break;
                    }
                    catch(XunitException ex)
                    {
                        lastError = ex;
                    }
                    catch(Exception ex)
                    {
                        throw new TestCaseErrorException("Assert.CollectionHasAny threw an unexpected Exception", ex);
                    }
                    i++;
                }
                if (failed)
                    throw new CollectionException(false, true, $"No Element found matching check {i}: {lastError.Message}", "Test passed", "Test failed");
                output.WriteLine($"  -> Element {i} matches criteria");
            }
        }
    }

    public class TestCaseErrorException : XunitException
    {
        public TestCaseErrorException(string userMessage, Exception exception) : base(userMessage, exception)
        {

        }
    }

    public class CollectionException : AssertActualExpectedException
    {
        public CollectionException(object expected, object actual, string userMessage, string expectedTitle = null, string actualTitle = null) : base(expected, actual, userMessage, expectedTitle, actualTitle) { }
    }

    public class NamedCheck<T>
    {
        public NamedCheck(string name, Check<T> action) {
            Name = name;
            Check = action;
        }

        public string Name { get; }
        public Check<T> Check { get; }
    }

    public class NullOutputHelper : ITestOutputHelper
    {
        public void WriteLine(string message)
        {
            
        }

        public void WriteLine(string format, params object[] args)
        {
            
        }
    }
}
