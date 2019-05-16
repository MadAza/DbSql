using System;
using System.Reflection;

namespace DbSql.Core.Columns
{
    internal class ColumnSchema
    {
        public ColumnSchema(string name, PropertyInfo property)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public string Name { get; }
        public PropertyInfo Property { get; }
    }
}
