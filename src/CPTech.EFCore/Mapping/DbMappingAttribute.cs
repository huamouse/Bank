using System;

namespace CPTech.EFCore.Mapping
{
    public class DbMappingAttribute : Attribute
    {
        private readonly string mappingName;

        public DbMappingAttribute(string mappingName)
        {
            this.mappingName = mappingName;
        }

        public string GetMappingName()
        {
            return this.mappingName;
        }
    }
}
