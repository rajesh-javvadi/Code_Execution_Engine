using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Executors.Service
{
    public class TestCaseExecutionQueue
    {
        private readonly ConcurrentQueue<(Guid Id, CodeExecutionRequest Request, List<TestCase> TestCases)> _queue = new ConcurrentQueue<(Guid, CodeExecutionRequest, List<TestCase>)>();
        private readonly ConcurrentDictionary<Guid, TestSuiteExecutionResult> _results = new ConcurrentDictionary<Guid, TestSuiteExecutionResult>();

        public Guid Enqueue(CodeExecutionRequest request, List<TestCase> testCases)
        {
            var id = Guid.NewGuid();
            _queue.Enqueue((id, request, testCases));
            _results[id] = null; // placeholder for result
            return id;
        }

        public bool TryDequeue(out (Guid Id, CodeExecutionRequest Request, List<TestCase> TestCases) item)
        {
            return _queue.TryDequeue(out item);
        }

        public void SetResult(Guid id, TestSuiteExecutionResult result)
        {
            _results[id] = result;
        }

        public bool TryGetResult(Guid id, out TestSuiteExecutionResult result)
        {
            return _results.TryGetValue(id, out result);
        }
    }
}
