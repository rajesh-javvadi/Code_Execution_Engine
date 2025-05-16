using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;

namespace Executors.Service
{
    public class TestCaseExecutor : ITestCaseExecutor
    {
        private readonly ICodeExecutor _codeExecutor;
        private readonly TestCaseExecutionQueue _executionQueue;

        public TestCaseExecutor(ICodeExecutor codeExecutor, TestCaseExecutionQueue executionQueue)
        {
            _codeExecutor = codeExecutor;
            _executionQueue = executionQueue;
        }


        public Task<TestSuiteExecutionResult> ExecuteTestCases(CodeExecutionRequest codeExecutionRequest, List<TestCase> testCases)
        {
            var id = _executionQueue.Enqueue(codeExecutionRequest, testCases);
            // Return a placeholder result indicating the request is queued
            var placeholderResult = new TestSuiteExecutionResult
            {
                TestCaseResults = new List<TestCaseResult>(),
                PassedCount = 0,
                FailedCount = 0,
                ErrorCount = 0
            };
            return Task.FromResult(placeholderResult);
        }

        public Task<TestSuiteExecutionResult> GetTestCaseExecutionResult(Guid id)
        {
            if (_executionQueue.TryGetResult(id, out var result))
            {
                return Task.FromResult(result);
            }
            else
            {
                return Task.FromResult<TestSuiteExecutionResult>(null);
            }
        }
    }
}
