using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class ModelInfo
    {
        public ModelInfo()
        {
            Fields = new List<FieldInfo>();
            Desc = "功能名称";
        }
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<FieldInfo> Fields { get; set; }
    }
}
