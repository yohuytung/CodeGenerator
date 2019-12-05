using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public abstract class CodeGeneratorBase : ICodeGenerator
    {
        protected ModelInfo _modelInfo;
        protected CodeGenerateInfo _generatorInfo;
        protected Dictionary<string, string> _templateGenerateDict;
        protected List<string> _tags;
        protected List<string> _fieldTags;
        protected string _templateFolder;
        public CodeGeneratorBase()
        {
            _templateFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Templates");
        }

        public virtual void Init(ModelInfo model, CodeGenerateInfo generatorInfo)
        {
            _modelInfo = model;
            _generatorInfo = generatorInfo;
        }

        public abstract void Do();

        public abstract void End();
    }
}
