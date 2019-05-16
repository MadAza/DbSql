using System;

namespace DbSql.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlColumnAttribute : Attribute
    {
        public SqlColumnAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
