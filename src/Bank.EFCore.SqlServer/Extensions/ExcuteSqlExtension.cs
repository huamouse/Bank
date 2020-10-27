using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CPTech.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// 数据库访问帮助类
    /// </summary>
    public static class ExcuteSqlExtension
    {
        public static ICollection<T> ExecuteReader<T>(this DbContext dbContext, string sql, params object[] sqlParams) where T : new()
        {
            Type type = typeof(T);
            ICollection<T> dt = new List<T>();

            var connection = dbContext.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sqlParams.Length > 0) command.Parameters.AddRange(sqlParams);

            using var dr = command.ExecuteReader();
            if (!dr.HasRows) return dt;

            var columns = dr.GetColumnSchema().Select(e => e.ColumnName.ToLower());
            while (dr.Read())
            {
                T t = new T();
                foreach (var prop in type.GetProperties())
                {
                    string propName = prop.GetMappingName();
                    if (!columns.Contains(propName.ToLower())) continue;

                    Type propType = prop.PropertyType;
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))   //支持可空类型
                        propType = propType.GetGenericArguments()[0];

                    if (dr[propName] != null && dr[propName] != DBNull.Value && dr[propName].GetType() != propType)
                    {
                        var tryParse = propType.GetMethod("TryParse", new Type[] { typeof(string), propType.MakeByRefType() });
                        if (tryParse != null)
                        {
                            var parameters = new object[] { dr[propName].ToString(), Activator.CreateInstance(propType) };
                            bool isSuccess = (bool)tryParse.Invoke(null, parameters);
                            //成功返回转换后的值
                            if (isSuccess) prop.SetValue(t, parameters[1]);
                        }
                    }
                    else
                        prop.SetValue(t, dr[propName] is DBNull ? null : dr[propName]);
                }
                dt.Add(t);
            }
            dr.Close();

            return dt;
        }

        public static async Task<ICollection<T>> ExecuteReaderAsync<T>(this DbContext dbContext, string sql, params object[] sqlParams) where T : new()
        {
            Type type = typeof(T);
            ICollection<T> dt = new List<T>();

            var connection = dbContext.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sqlParams.Length > 0) command.Parameters.AddRange(sqlParams);

            using var dr = await command.ExecuteReaderAsync();
            if (!dr.HasRows) return dt;

            var columns = dr.GetColumnSchema().Select(e => e.ColumnName.ToLower());
            while (await dr.ReadAsync())
            {
                T t = new T();
                foreach (var prop in type.GetProperties())
                {
                    string propName = prop.GetMappingName();
                    if (!columns.Contains(propName.ToLower())) continue;

                    Type propType = prop.PropertyType;
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))   //支持可空类型
                        propType = propType.GetGenericArguments()[0];

                    if (dr[propName] != null && dr[propName] != DBNull.Value && dr[propName].GetType() != propType)
                    {
                        var tryParse = propType.GetMethod("TryParse", new Type[] { typeof(string), propType.MakeByRefType() });
                        if (tryParse != null)
                        {
                            var parameters = new object[] { dr[propName].ToString(), Activator.CreateInstance(propType) };
                            bool isSuccess = (bool)tryParse.Invoke(null, parameters);
                            //成功返回转换后的值
                            if (isSuccess) prop.SetValue(t, parameters[1]);
                        }
                    }
                    else
                        prop.SetValue(t, dr[propName] is DBNull ? null : dr[propName]);
                }
                dt.Add(t);
            }
            //await dr.CloseAsync();

            return dt;
        }

        public static Object ExecuteScalar(this DbContext dbContext, string sql, params object[] sqlParams)
        {
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sqlParams.Length > 0) command.Parameters.AddRange(sqlParams);

            return command.ExecuteScalar();
        }

        public static async Task<Object> ExecuteScalarAsync(this DbContext dbContext, string sql, params object[] sqlParams)
        {
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sqlParams.Length > 0) command.Parameters.AddRange(sqlParams);

            return await command.ExecuteScalarAsync();
        }

        public static int ExecuteNonQuery(this DbContext dbContext, string sql, params object[] sqlParams)
        {
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sqlParams.Length > 0) command.Parameters.AddRange(sqlParams);

            return command.ExecuteNonQuery();
        }

        public static async Task<int> ExecuteNonQueryAsync(this DbContext dbContext, string sql, params object[] sqlParams)
        {
            var connection = dbContext.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sqlParams.Length > 0) command.Parameters.AddRange(sqlParams);

            return await command.ExecuteNonQueryAsync();
        }
    }
}
