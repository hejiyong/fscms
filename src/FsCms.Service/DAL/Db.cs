using FreeSql;
using FsCms.Service.Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FsCms.Service.DAL
{
    public static class Db
    {
        public static System.Collections.Generic.Dictionary<string, IFreeSql> ConnectionPool = new System.Collections.Generic.Dictionary<string, IFreeSql>();

        private static string getConnectionString(string sDatabaseType)
        {
            return AppSettingsManager.Get($"DbContexts:{sDatabaseType}:ConnectionString");
        }

        private static IFreeSql SelectDBType(DataType enum_dbtype)
        {
            var dbtype = enum_dbtype.ToString();
            if (!ConnectionPool.ContainsKey(dbtype))
            {
                var freesql = new FreeSql.FreeSqlBuilder()
                     .UseConnectionString(enum_dbtype, getConnectionString(dbtype))
                     .UseAutoSyncStructure(true)
                     .UseMonitorCommand(
                        cmd =>
                        {
                            Trace.WriteLine(cmd.CommandText);
                        }, //监听SQL命令对象，在执行前
                        (cmd, traceLog) =>
                        {
                            Console.WriteLine(traceLog);
                        }) //监听SQL命令对象，在执行后
                    .UseLazyLoading(true)
                    .Build();

                freesql.Aop.ConfigEntityProperty = (s, e) =>
                {
                    //默认设置主键
                    if (e.Property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Any())
                    {
                        e.ModifyResult.IsPrimary = true;
                    }

                    //设置maxlength
                    if (e.Property.PropertyType == typeof(string))
                    {
                        var strLen = e.Property.GetCustomAttribute<System.ComponentModel.DataAnnotations.MaxLengthAttribute>();
                        if (strLen != null)
                        {
                            e.ModifyResult.DbType = freesql.CodeFirst.GetDbInfo(e.Property.PropertyType)?.dbtype + "(" + strLen.Length + ")";
                        }
                    }
                };
                ConnectionPool.Add(dbtype, freesql);
            }
            return ConnectionPool[dbtype];
        }

        public static IFreeSql DB(this DataType t)
        {
            return SelectDBType(t);
        }
    }
}
