using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class FieldInfo : ICloneable
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }

        public bool IsRequied { get; set; }


        public int MaxLength { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
