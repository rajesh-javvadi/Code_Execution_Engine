using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TestCase
    {
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
        //public int TimeLimitMs { get; set; } = 1000;
        //public int MemoryLimitMb { get; set; } = 50;
    }
}
