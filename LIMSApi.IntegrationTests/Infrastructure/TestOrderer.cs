using Xunit.Abstractions;
using Xunit.Sdk;

namespace LIMSApi.IntegrationTests.Infrastructure;

/// <summary>
/// Orders test cases alphabetically by method name.
/// Used with [TestCaseOrderer] to ensure tests run in T01→T02→...→T09 order.
/// Within a class, methods prefixed with Step01_, Step02_ etc. will run in order.
/// </summary>
public class AlphabeticalOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var sorted = testCases.OrderBy(tc => tc.TestMethod.Method.Name).ToList();
        return sorted;
    }
}
