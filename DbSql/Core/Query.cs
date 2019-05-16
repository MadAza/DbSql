using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

using DbSql.Core.Columns;
using DbSql.Core.Parameters;

namespace DbSql.Core
{
    public class Query : IDisposable
    {
        internal Query(IParameterBuilder parameterBuilder, IReaderMapper readerMapper, DbConnection connection)
        {
            this.parameterBuilder = parameterBuilder ?? throw new ArgumentNullException(nameof(parameterBuilder));
            this.readerMapper = readerMapper ?? throw new ArgumentNullException(nameof(readerMapper));
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            
            disposed = false;
            locker = new object();
        }
  
        private readonly IParameterBuilder parameterBuilder;
        private readonly IReaderMapper readerMapper;
        private readonly DbConnection connection;

        private readonly object locker;
        private bool disposed;

        private string text;
        private object parameters;

        public Query Text(string text)
        {
            this.text = text;
            return this;
        }
        public Query Parameters(object parameters)
        {
            this.parameters = parameters;
            return this;
        }

        public IEnumerable<T> Read<T>(Func<T> entityFactory = null)
        {
            Func<T> localEntityFactory = entityFactory;

            List<T> entities = new List<T>();

            using (DbCommand command = CreateCommand())
            {               
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return entities;

                    T entity = localEntityFactory is null ? default : localEntityFactory();         

                    while(reader.Read())
                    {
                        readerMapper.Map(reader, entity);
                        entities.Add(entity);
                    }
                }
            }

            Dispose();
            return entities;
        }
        public T ReadFirst<T>(Func<T> entityFactory = null)
        {
            Func<T> localEntityFactory = entityFactory;

            T entity;

            using (DbCommand command = CreateCommand())
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return default;

                    entity = localEntityFactory is null ? default : localEntityFactory();
                    reader.Read();
                    readerMapper.Map(reader, entity);
                }
            }

            Dispose();
            return entity;
        }

        public async Task<IEnumerable<T>> ReadAsync<T>(Func<T> entityFactory = null)
        {
            Func<T> localEntityFactory = entityFactory;

            List<T> entities = new List<T>();

            using (DbCommand command = CreateCommand())
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) return entities;

                    T entity = localEntityFactory is null ? default : localEntityFactory();
          
                    while (await reader.ReadAsync())
                    {
                        entities.Add(readerMapper.Map(reader, entity));
                    }        
                }
            }

            Dispose();
            return entities;
        }
        public async Task<T> ReadFirstAsync<T>(Func<T> entityFactory = null)
        {
            Func<T> localEntityFactory = entityFactory;

            T entity;

            using (DbCommand command = CreateCommand())
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) return default;

                    entity = localEntityFactory is null ? default : localEntityFactory();

                    await reader.ReadAsync();
                    readerMapper.Map(reader, entity);
                }
            }

            Dispose();
            return entity;
        }

        public int Execute()
        {
            using (DbCommand command = CreateCommand())
            {
                int result = command.ExecuteNonQuery();
                Dispose();
                return result;
            }
        }
        public async Task<int> ExecuteAsync()
        {
            using (DbCommand command = CreateCommand())
            {
                int result  = await command.ExecuteNonQueryAsync();
                Dispose();
                return result;
            }
        }

        private DbCommand CreateCommand()
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            DbCommand command = connection.CreateCommand();
            command.CommandText = text;

            if (!(parameters is null))
            {
                foreach (DbParameter parameter in parameterBuilder.Build(parameters))
                {
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        public void Dispose()
        {
            lock(locker)
            {
                if (!disposed)
                {
                    disposed = true;
                    connection.Dispose();
                }
            }            
        }
    }
}
