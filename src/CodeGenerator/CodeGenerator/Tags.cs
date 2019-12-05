using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public static class Tags
    {
        public const string CreateDate = "{{CreateDate}}";

        public const string MaxLength = "{{MaxLength}}";
        public const string IsRequied = "{{IsRequied}}";
        public const string FieldName = "{{FieldName}}";
        public const string FieldDesc = "{{FieldDesc}}";
        public const string FieldType = "{{FieldType}}";
        public const string IsDateFormat = "{{IsDateFormat}}";
        public const string FieldXType = "{{FieldXType}}";

        public const string Company = "{{Company}}";
        public const string Category = "{{Category}}";
        public const string ModelName = "{{ModelName}}";
        public const string ModelDesc = "{{ModelDesc}}";
        public const string ModelNameLower = "{{ModelNameLower}}";

        public const string ForeachFieldBegin = "{{ForeachFieldBegin}}";
        public const string ForeachFieldEnd = "{{ForeachFieldEnd}}";
    }
}
