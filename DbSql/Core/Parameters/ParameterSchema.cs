using System;
using System.Data;
using System.Reflection;

namespace DbSql.Core.Parameters
{
    internal class ParameterSchema
    {
        public ParameterSchema(string name, DbType type, PropertyInfo property)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public string Name { get; }
        public DbType Type { get; }
        public PropertyInfo Property { get; }
    }
}
