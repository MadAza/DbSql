using System;

namespace DbSql.Core
{
    /// <summary>
    /// Указывает, что данное свойство должно получать значение из столбца с указанным именем
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlColumnAttribute : Attribute
    {
        public SqlColumnAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Имя Sql столбца
        /// </summary>
        public string Name { get; }
    }
}
