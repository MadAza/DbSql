using System;
using System.Collections.Generic;

namespace DbSql.Core.Columns
{
    internal class EntityColumnSchema
    {
        public EntityColumnSchema(Type entityType, IEnumerable<ColumnSchema> columns)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Columns = new List<ColumnSchema>(columns);
        }

        public Type EntityType { get; }
        public IReadOnlyList<ColumnSchema> Columns { get; }
    }
}
