using System;

namespace DbSql.Core
{
    /// <summary>
    /// Помечает свойство как неиспользуемое в создании SQL-параметров
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotSqlParameterAttribute : Attribute {}
}
