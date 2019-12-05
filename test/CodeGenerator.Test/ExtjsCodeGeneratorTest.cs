using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CodeGenerator.Test
{
    public class ExtjsCodeGeneratorTest
    {
        private ExtjsCodeGenerator _generator;
        public ExtjsCodeGeneratorTest()
        {
            _generator = new ExtjsCodeGenerator();
            var model = new ModelInfo()
            {
                Name = "Order",
                Fields = new List<FieldInfo>
                {
                    new FieldInfo
                    {
                        Name = "OrderNo",
                        DataType = "string",
                        Description = "订单号",
                        IsRequied = true,
                        MaxLength = 30
                    },
                    new FieldInfo
                    {
                        Name = "OrderCost",
                        DataType = "decimal",
                        Description = "订单费用",
                        IsRequied = false,
                        MaxLength = 30
                    }
                }
            };
            CodeGenerateInfo info = new CodeGenerateInfo
            {
                Category = "Datum",
                Company = "ZDT",
                GenerateResultPath = @"C:\CodeGenerate",
                ControllerFilePath = @"C:\CodeGenerate\app\view\Datum",
                ModelFilePath = @"C:\CodeGenerate\app\model\Datum",
                ViewFilePath = @"C:\CodeGenerate\classic\src\view\Datum"
            };
            _generator.Init(model, info);
        }

        [Fact(DisplayName = "MatchTags")]
        public void MatchTags()
        {
            var template = "GridView.js";
            var file = Tags.ModelName + template;
            _generator.GenerateCodeByTemplate(template, file);
        }
    }
}
