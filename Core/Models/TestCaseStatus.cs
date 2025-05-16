using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public enum TestCaseStatus
    {
        Passed,
        Failed,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        RuntimeError,
        CompilationError
    }
}
