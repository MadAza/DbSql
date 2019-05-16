using System;
using System.Collections.Generic;

namespace DbSql.Core.Parameters
{
    internal class EntityParameterSchema
    {
        public EntityParameterSchema(Type entityType, IEnumerable<ParameterSchema> parameters)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            Parameters = new List<ParameterSchema>(parameters);
        }

        public Type EntityType { get; }
        public IReadOnlyList<ParameterSchema> Parameters { get; }
    }
}
