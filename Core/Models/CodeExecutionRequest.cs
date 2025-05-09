using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CodeExecutionRequest
    {
        public string Language { get; set; }
        public string Code { get; set; }
        public string Input { get; set; }
    }
}
