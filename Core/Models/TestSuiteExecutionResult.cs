using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TestSuiteExecutionResult
    {
        public List<TestCaseResult> TestCaseResults { get; set; }
        public int TotalTestCases { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public int ErrorCount { get; set; }
        public TestSuiteStatus OverallStatus { get; set; }
    }
}
