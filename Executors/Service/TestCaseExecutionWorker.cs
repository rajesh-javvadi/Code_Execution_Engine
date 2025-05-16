using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Interfaces;

namespace Executors.Service
{
    public class TestCaseExecutionWorker
    {
        private readonly TestCaseExecutionQueue _queue;
        private readonly ICodeExecutor _codeExecutor;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public TestCaseExecutionWorker(TestCaseExecutionQueue queue, ICodeExecutor codeExecutor)
        {
            _queue = queue;
            _codeExecutor = codeExecutor;
        }

        public void Start()
        {
            Task.Run(ProcessQueueAsync);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var item))
                {
                    var (id, request, testCases) = item;
                    var result = new TestSuiteExecutionResult();
                    result.TestCaseResults = new System.Collections.Generic.List<TestCaseResult>();

                    foreach (var testCase in testCases)
                    {
                        var response = await _codeExecutor.ExecuteCodeAsync(new CodeExecutionRequest
                        {
                            Code = request.Code,
                            Input = testCase.Input,
                            Language = request.Language
                        });

                        if (response.Success)
                        {
                            if (response.Output.Trim().Equals(testCase.ExpectedOutput.Trim()))
                            {
                                result.PassedCount++;
                                result.TestCaseResults.Add(new TestCaseResult { Status = TestCaseStatus.Passed });
                            }
                            else
                            {
                                result.FailedCount++;
                                result.TestCaseResults.Add(new TestCaseResult { Status = TestCaseStatus.Failed });
                            }
                        }
                        else
                        {
                            result.ErrorCount++;
                            result.TestCaseResults.Add(new TestCaseResult { Status = TestCaseStatus.CompilationError });
                        }
                    }

                    _queue.SetResult(id, result);
                }
                else
                {
                    await Task.Delay(500); // wait before checking queue again
                }
            }
        }
    }
}
