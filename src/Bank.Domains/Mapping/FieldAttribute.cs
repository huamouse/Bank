using System;
using System.Collections.Generic;
using System.Text;

namespace CPTech.EntityFrameworkCore.Mapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : DbMappingAttribute
    {
        public FieldAttribute(string columnName) : base(columnName)
        {
        }
    }
}
