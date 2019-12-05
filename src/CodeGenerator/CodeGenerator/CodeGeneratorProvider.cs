using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeGenerator
{
    public class CodeGeneratorProvider
    {
        private ICodeGenerator _iCodeGenerator;
        public CodeGeneratorProvider(GeneratorTypeEnum type)
        {
            _iCodeGenerator = GeneratorFactory.GetGenerator(type);
        }

        public void Start(ModelInfo model, CodeGenerateInfo generatorInfo)
        {
            try
            {
                _iCodeGenerator.Init(model, generatorInfo);
                _iCodeGenerator.Do();
                _iCodeGenerator.End();
                MessageBox.Show("生成成功！");
            }
            catch (Exception ex)
            {
                // TODO 唐友辉 2017-10-01 1:13 AM
                // log
            }
        }
    }
}
