using System;
using System.Reflection;
using CPTech.EntityFrameworkCore.Mapping;

namespace CPTech.EntityFrameworkCore.Extensions
{
    public static class DBMappingExtension
    {
        public static string GetMappingName<T>(this T t) where T : MemberInfo
        {
            if (t.IsDefined(typeof(DbMappingAttribute), true))
            {
                var attribute = t.GetCustomAttribute<DbMappingAttribute>();
                return attribute.GetMappingName();
            }
            else
            {
                return t.Name;
            }
        }


        public static string GetMappingEntityName(this Type type)
        {
            if (type.IsDefined(typeof(EntityAttribute), true))
            {
                var attribute = type.GetCustomAttribute<EntityAttribute>();
                return attribute.GetMappingName();
            }
            else
            {
                return type.Name;
            }
        }
        public static string GetMappingFieldName(this PropertyInfo prop)
        {
            if (prop.IsDefined(typeof(FieldAttribute), true))
            {
                var attribute = prop.GetCustomAttribute<FieldAttribute>();
                return attribute.GetMappingName();
            }
            else
            {
                return prop.Name;
            }
        }
    }
}
