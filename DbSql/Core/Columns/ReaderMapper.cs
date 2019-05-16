using System;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace DbSql.Core.Columns
{
    internal class ReaderMapper : IReaderMapper
    {
        public ReaderMapper()
        {
            cachedSchemas = new BlockingCollection<EntityColumnSchema>();
        }

        private readonly BlockingCollection<EntityColumnSchema> cachedSchemas;

        public T Map<T>(DbDataReader reader, T entity)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            Type entityType = typeof(T);

            if (SqlTypeMap.Types.ContainsKey(entityType))
            {
                 return (T)reader[0];
            }

            if(cachedSchemas.Any(schema => schema.EntityType == entityType))
            {
                return MapFromCache(reader, entity);             
            }

            cachedSchemas.Add(CreateSchema(entityType));

            return MapFromCache(reader, entity);
        }

        private T MapFromCache<T>(DbDataReader reader, T entity)
        {
            EntityColumnSchema entitySchema = cachedSchemas.First(schema => schema.EntityType == typeof(T));

            string[] columnNames = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName).ToArray();

            foreach(ColumnSchema column in entitySchema.Columns)
            {
                if(columnNames.Contains(column.Name))
                {
                    column.Property.SetValue(entity, reader[column.Name]);
                }
            }

            return entity;
        }

        private EntityColumnSchema CreateSchema(Type type)
        {
            List<ColumnSchema> columns = new List<ColumnSchema>();

            foreach(PropertyInfo property in type.GetProperties())
            {
                if(property.GetCustomAttribute<NotSqlColumnAttribute>() is null)
                {
                    SqlColumnAttribute sqlColumn =
                        property.GetCustomAttribute<SqlColumnAttribute>();

                    string name = sqlColumn is null ? property.Name : sqlColumn.Name;

                    columns.Add(new ColumnSchema(name, property));
                }
            }

            return new EntityColumnSchema(type, columns);
        }
    }
}
