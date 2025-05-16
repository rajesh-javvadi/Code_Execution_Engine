//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Core.Interfaces;
//using Core.Models;
//using Newtonsoft.Json;

//namespace Executors.Sandbox
//{
//    public class DefaultExecutionResultProcessor : IExecutionResultProcessor
//    {
//        public TestCaseResult ProcessTestCaseResult(
//            CodeExecutionResponse executionResponse,
//            object expectedOutput,
//            int timeLimitMs,
//            int memoryLimitMb)
//        {
//            var result = new TestCaseResult
//            {
//                ActualOutput = executionResponse.Output,
//                ErrorMessage = executionResponse.Error,
//                ExecutionTimeMs = 0, 
//                MemoryUsageMb = 0   
//            };

//            if (!executionResponse.Success)
//            {
//                result.Status = executionResponse.Error.Contains("Time limit exceeded")
//                    ? TestCaseStatus.TimeLimitExceeded
//                    : TestCaseStatus.RuntimeError;
//                return result;
//            }

//            try
//            {
//                var actual = JsonConvert.DeserializeObject(executionResponse.Output);
//                var expected = expectedOutput;

//                if (CompareOutputs(actual, expected))
//                {
//                    result.Status = TestCaseStatus.Passed;
//                }
//                else
//                {
//                    result.Status = TestCaseStatus.Failed;
//                }
//            }
//            catch (Exception ex)
//            {
//                result.Status = TestCaseStatus.RuntimeError;
//                result.ErrorMessage = $"Output parsing failed: {ex.Message}";
//            }

//            return result;
//        }
//        private bool CompareOutputs(object actual, object expected)
//        {
//            if (actual == null && expected == null) return true;
//            if (actual == null || expected == null) return false;

//            if (actual is double actualDouble && expected is double expectedDouble)
//            {
//                return Math.Abs(actualDouble - expectedDouble) < 1e-9;
//            }

//            if (actual.GetType() != expected.GetType()) return false;

//            if (actual is IEnumerable<object> actualEnumerable && expected is IEnumerable<object> expectedEnumerable)
//            {
//                return actualEnumerable.SequenceEqual(expectedEnumerable, new ObjectComparer());
//            }

//            return actual.Equals(expected);
//        }

//        private class ObjectComparer : IEqualityComparer<object>
//        {
//            public new bool Equals(object x, object y) => x?.Equals(y) ?? y == null;
//            public int GetHashCode(object obj) => obj?.GetHashCode() ?? 0;
//        }

//    }
//}
