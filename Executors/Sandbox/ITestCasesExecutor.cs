using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Executors.Sandbox
{
    public interface ITestCasesExecutor
    {
        public  Task<CodeExecutionResponse> ExecuteCodeAsync(CodeExecutionRequest request,string testCases);
    }
}
