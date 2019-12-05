using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public interface ICodeGenerator
    {
        void Init(ModelInfo model, CodeGenerateInfo generatorInfo);
        void Do();
        void End();
    }
}
