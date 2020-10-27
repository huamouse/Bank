using System;
using System.Collections.Generic;
using System.Text;

namespace CPTech.EntityFrameworkCore.Mapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : DbMappingAttribute
    {
        public EntityAttribute(string entityName) : base(entityName)
        {
        }
    }
}
