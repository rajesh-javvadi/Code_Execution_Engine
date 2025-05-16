using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    public interface ITestCaseExecutor
    {
        Task<TestSuiteExecutionResult> ExecuteTestCases(CodeExecutionRequest codeExecutionRequest, List<TestCase> testCases);
        Task<TestSuiteExecutionResult> GetTestCaseExecutionResult(Guid id);
    }
}
