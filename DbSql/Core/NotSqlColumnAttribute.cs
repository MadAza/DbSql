using System;

namespace DbSql.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotSqlColumnAttribute : Attribute {}
}
