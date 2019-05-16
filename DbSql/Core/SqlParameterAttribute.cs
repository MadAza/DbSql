using System;

namespace DbSql.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlParameterAttribute : Attribute
    {
        public SqlParameterAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}
