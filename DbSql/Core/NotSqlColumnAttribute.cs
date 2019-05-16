using System;

namespace DbSql.Core
{
    /// <summary>
    /// Помечает свойство как неиспользуемое при чтении данных из ответа на SQL-запрос
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotSqlColumnAttribute : Attribute {}
}
