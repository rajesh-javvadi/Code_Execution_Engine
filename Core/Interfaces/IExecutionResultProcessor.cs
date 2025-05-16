using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for processing execution results into test case results
    /// </summary>
    public interface IExecutionResultProcessor
    {
        TestCaseResult ProcessTestCaseResult(
            CodeExecutionResponse executionResponse,
            object expectedOutput,
            int timeLimitMs,
            int memoryLimitMb);
    }
}
