using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class CodeGenerateInfo
    {
        public string Company { get; set; }
        public string Category { get; set; }
        public string GenerateResultPath { get; set; }
        public string ModelFilePath { get; set; }
        public string ControllerFilePath { get; set; }
        public string ViewFilePath { get; set; }
    }
}
