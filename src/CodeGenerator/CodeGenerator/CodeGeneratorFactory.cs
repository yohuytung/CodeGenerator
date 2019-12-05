using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{

    public enum GeneratorTypeEnum : sbyte
    {
        ExtJS6
    }

    public static class GeneratorFactory
    {
        public static ICodeGenerator GetGenerator(GeneratorTypeEnum type)
        {
            ICodeGenerator r = null;
            switch (type)
            {
                case GeneratorTypeEnum.ExtJS6:
                    r = new ExtjsCodeGenerator();
                    break;
            }
            return r;
        }
    }
}
