using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CodeTestSuiteRequest
    {
        public string Code { get; set; }
        public string Language { get; set; }
        public List<TestCase> TestCases { get; set; }
    }

}
