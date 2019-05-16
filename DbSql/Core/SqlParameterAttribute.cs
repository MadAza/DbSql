using System;

namespace DbSql.Core
{
    /// <summary>
    /// Указывает, что данное свойство должно конвертироваться в параметр с указанным именем
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlParameterAttribute : Attribute
    {
        public SqlParameterAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; set; }
    }
}
