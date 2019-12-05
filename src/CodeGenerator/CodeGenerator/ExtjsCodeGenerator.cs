using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public class ExtjsCodeGenerator : CodeGeneratorBase
    {
        private const string TEMPLATE_ADDCONTROLLER = "AddController.js";
        private const string TEMPLATE_CONTROLLER = "Controller.js";
        private const string TEMPLATE_MODEL = "Model.js";
        private const string TEMPLATE_VIEWMODEL = "ViewModel.js";
        private const string TEMPLATE_PANEL = "Panel.js";
        private const string TEMPLATE_GRIDVIEW = "GridView.js";
        private const string TEMPLATE_ADDVIEW = "AddView.js";
        private const string TEMPLATE_EXTJS4 = "Extjs4.js";

        public ExtjsCodeGenerator()
        {
            _templateGenerateDict = new Dictionary<string, string>()
            {
                {TEMPLATE_ADDCONTROLLER, Tags.ModelName + TEMPLATE_ADDCONTROLLER},
                {TEMPLATE_CONTROLLER, Tags.ModelName + TEMPLATE_CONTROLLER},
                {TEMPLATE_MODEL, Tags.ModelName + ".js"},
                {TEMPLATE_VIEWMODEL, Tags.ModelName + TEMPLATE_MODEL},
                {TEMPLATE_PANEL, Tags.ModelName + ".js"},
                {TEMPLATE_GRIDVIEW, Tags.ModelName + "Grid.js"},
                {TEMPLATE_ADDVIEW, Tags.ModelName + "Add.js"},
                {TEMPLATE_EXTJS4, Tags.ModelName + ".js"},
            };

            _tags = new List<string>
            {
                Tags.Category,
                Tags.Company,
                Tags.ForeachFieldBegin,
                //Tags.ForeachFieldEnd,
                Tags.ModelName,
                Tags.ModelNameLower,
                Tags.ModelDesc,
                Tags.CreateDate
            };

            _fieldTags = new List<string>
            {
                Tags.FieldDesc,
                Tags.FieldName,
                Tags.FieldType,
                Tags.IsRequied,
                Tags.IsDateFormat,
                Tags.FieldXType,
                Tags.ModelName,
                Tags.MaxLength
            };
        }
        public override void Init(ModelInfo model, CodeGenerateInfo generatorInfo)
        {
            base.Init(model, generatorInfo);
        }

        public override void Do()
        {
            _modelInfo.Fields.ForEach(f =>
            {
                f.DataType = SwitchFieldTypeStr(f.DataType);
            });

            try
            {
                foreach (KeyValuePair<string, string> keyValuePair in _templateGenerateDict)
                {
                    GenerateCodeByTemplate(keyValuePair.Key, keyValuePair.Value);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void GenerateCodeByTemplate(string t, string generateFile)
        {
            using (var reader = new StreamReader(Path.Combine(_templateFolder, t)))
            {
                var content = reader.ReadToEnd();
                string r = ReplaceTemplateTags(content);
                var folderPath = GetTemplateGeneratePath(t);
                var fileName = ReplaceTemplateTags(generateFile);
                var filePath = Path.Combine(folderPath, fileName);
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.Write(r);
                    writer.Flush();
                    writer.Close();
                }
                reader.Close();
            }
        }

        private string ReplaceTemplateTags(string content)
        {
            var r = content;

            _tags.ForEach(tag =>
            {
                if (tag == Tags.ForeachFieldBegin)
                {
                    Regex regex = new Regex(Tags.ForeachFieldBegin + @"([\s\S]*?)" + Tags.ForeachFieldEnd);
                    if (regex.IsMatch(r))
                    {
                        var mcs = regex.Matches(r);
                        foreach (Match mc in mcs)
                        {
                            var mcValue = mc.Groups[1].Value;
                            List<string> loopList = new List<string>();
                            _modelInfo.Fields.ForEach(f =>
                            {
                                var temp = mcValue;
                                _fieldTags.ForEach(tag2 =>
                                {
                                    Regex fieldRegex = new Regex(tag2);
                                    if (fieldRegex.IsMatch(temp))
                                    {
                                        var v = GetFieldTagValue(f, tag2);
                                        temp = fieldRegex.Replace(temp, v);
                                    }
                                });

                                loopList.Add(temp);
                            });

                            r = r.Replace(mc.Groups[0].Value, string.Join(",", loopList));
                        }
                    }
                }
                else
                {
                    Regex regex = new Regex(tag);

                    if (regex.IsMatch(r))
                    {
                        var v = GetTagValue(tag);
                        r = regex.Replace(r, v);
                    }
                }
            });

            return r;
        }

        private string GetTagValue(string tag)
        {
            var v = string.Empty;
            switch (tag)
            {
                case Tags.Company:
                    v = _generatorInfo.Company;
                    break;
                case Tags.Category:
                    v = _generatorInfo.Category;
                    break;
                case Tags.ModelName:
                    v = _modelInfo.Name;
                    break;
                case Tags.ModelDesc:
                    v = _modelInfo.Desc;
                    break;
                case Tags.ModelNameLower:   // 首字母小写
                    if (_modelInfo.Name.Length > 0)
                    {
                        v = _modelInfo.Name.Substring(0, 1).ToLower() + _modelInfo.Name.Substring(1);
                    }
                    break;
                case Tags.CreateDate:
                    v = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    break;
            }
            return v;
        }
        private string GetFieldTagValue(FieldInfo field, string tag)
        {
            var v = string.Empty;
            switch (tag)
            {
                case Tags.FieldDesc:
                    v = field.Description;
                    break;
                case Tags.FieldName:
                    v = field.Name;
                    break;
                case Tags.FieldType:
                    if (!string.IsNullOrEmpty(field.DataType))
                    {
                        v = $", type: '{field.DataType}'";
                    }
                    break;
                case Tags.MaxLength:
                    v = field.MaxLength.ToString();
                    break;
                case Tags.IsRequied:
                    v = (!field.IsRequied).ToString().ToLower();
                    break;
                case Tags.IsDateFormat:
                    if (field.DataType == "date")
                    {
                        v = "xtype: 'datecolumn',format: 'Y-m-d H:i',";
                    }
                    break;
                case Tags.FieldXType:
                    switch (field.DataType)
                    {
                        case "date":
                            v = "xtype: 'datefield',format: 'Y-m-d H:i',";
                            break;
                        case "int":
                            v = "xtype: 'numberfield',";
                            break;
                        case "float":
                            v = "xtype: 'numberfield', decimalPrecision: 4,";
                            break;
                        default:
                            v = "xtype: 'textfield',";
                            break;
                    }
                    break;
                default:
                    v = GetTagValue(tag);
                    break;
            }
            return v ?? string.Empty;
        }
        private string GetTemplateGeneratePath(string template)
        {
            var v = string.Empty;
            switch (template)
            {
                case TEMPLATE_ADDCONTROLLER:
                case TEMPLATE_VIEWMODEL:
                case TEMPLATE_CONTROLLER:
                    v = _generatorInfo.ControllerFilePath;
                    break;
                case TEMPLATE_ADDVIEW:
                case TEMPLATE_PANEL:
                case TEMPLATE_GRIDVIEW:
                    v = _generatorInfo.ViewFilePath;
                    break;
                case TEMPLATE_MODEL:
                    v = _generatorInfo.ModelFilePath;
                    break;
                default:
                    v = _generatorInfo.GenerateResultPath;
                    break;
            }
            return v;
        }
        public override void End()
        {

        }

        private string SwitchFieldTypeStr(string typeName)
        {
            var r = string.Empty;
            switch (typeName.ToLower())
            {
                case "int64":
                case "int32":
                case "int16":
                case "byte":
                    r = "int";
                    break;
                case "decimal":
                case "double":
                    r = "float";
                    break;
                case "datetime":
                    r = "date";
                    break;
                case "boolean":
                    r = "bool";
                    break;
                default:
                    r = typeName.ToLower();
                    break;
            }

            return r;
        }
    }
}
