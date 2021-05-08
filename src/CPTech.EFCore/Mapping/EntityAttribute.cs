using System;

namespace CPTech.EFCore.Mapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : DbMappingAttribute
    {
        public EntityAttribute(string entityName) : base(entityName)
        {
        }
    }
}
