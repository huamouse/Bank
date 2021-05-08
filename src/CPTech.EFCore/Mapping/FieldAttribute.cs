using System;

namespace CPTech.EFCore.Mapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : DbMappingAttribute
    {
        public FieldAttribute(string columnName) : base(columnName)
        {
        }
    }
}
