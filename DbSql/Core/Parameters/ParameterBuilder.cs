using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Data.Common;
using System.Collections.Generic;

namespace DbSql.Core.Parameters
{
    internal class ParameterBuilder : IParameterBuilder
    {
        public ParameterBuilder(Func<DbParameter> parameterFactory)
        {
            cachedSchemas = new BlockingCollection<EntityParameterSchema>();
            this.parameterFactory = parameterFactory ?? throw new ArgumentNullException(nameof(parameterFactory));
        }

        private readonly BlockingCollection<EntityParameterSchema> cachedSchemas;
        private readonly Func<DbParameter> parameterFactory;

        public DbParameter[] Build(object parametersObject)
        {
            if(parametersObject is null)
            {
                throw new ArgumentNullException(nameof(parametersObject));
            }

            Type type = parametersObject.GetType();

            if(cachedSchemas.Any(schema => schema.EntityType == type))
            {
                return BuildFromCache(parametersObject);
            }

            cachedSchemas.Add(CreateSchema(parametersObject));

            return BuildFromCache(parametersObject);
        }

        private DbParameter[] BuildFromCache(object parametersObject)
        {
            EntityParameterSchema entitySchema = cachedSchemas
                .First(schema => schema.EntityType == parametersObject.GetType());

            DbParameter[] parameters = new DbParameter[entitySchema.Parameters.Count];

            for(int i = 0; i < entitySchema.Parameters.Count; i++)
            {
                DbParameter parameter = parameterFactory();

                parameter.ParameterName = entitySchema.Parameters[i].Name;
                parameter.DbType = entitySchema.Parameters[i].Type;
                parameter.Value = entitySchema.Parameters[i].Property.GetValue(parametersObject);

                parameters[i] = parameter;
            }

            return parameters;
        }

        private EntityParameterSchema CreateSchema(object parametersObject)
        {
            List<ParameterSchema> parameters = new List<ParameterSchema>();

            Type entityType = parametersObject.GetType();       
            foreach(PropertyInfo property in entityType.GetProperties())
            {
                if(property.GetCustomAttribute<NotSqlParameterAttribute>() is null)
                {
                    SqlParameterAttribute parameterAttribute = 
                        property.GetCustomAttribute<SqlParameterAttribute>();

                    string name = parameterAttribute is null ? property.Name : parameterAttribute.Name;
                    DbType type = SqlTypeMap.Types[property.PropertyType];

                    parameters.Add(new ParameterSchema(name, type, property));
                }
            }

            return new EntityParameterSchema(entityType, parameters);
        }
    }
}
