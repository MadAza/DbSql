using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

using DbSql.Core.Columns;
using DbSql.Core.Parameters;

namespace DbSql.Core
{
    /// <summary>
    /// Класс представляющий собой один SQL-запрос. <para/>
    /// Внимание, каждый объект <see cref="Query"/> может использоваться только один раз
    /// </summary>
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

        /// <summary>
        /// Устанавливает текст SQL-запроса
        /// </summary>
        /// <param name="text">Текст SQL-запроса</param>
        /// <returns>Возвращается текущий объект</returns>
        public Query Text(string text)
        {
            this.text = text;
            return this;
        }

        /// <summary>
        /// Задает параметры SQL-запроса
        /// </summary>
        /// <param name="parameters">Параметры SQL-запроса</param>
        /// <returns>Возвращается текущий объект</returns>
        public Query Parameters(object parameters)
        {
            this.parameters = parameters;
            return this;
        }

        /// <summary>
        /// Выполняет SQL-запрос и возвращает ответ указанного типа в качестве списка<para/>
        /// Если объект должен вернуть сложный объект, то нужно передать в аргументы делегат создающий этот объект
        /// </summary>
        /// <typeparam name="T">Тип результата</typeparam>
        /// <param name="entityFactory">Делегат создающий объект</param>
        /// <returns>Список ответов указанного типа/returns>
        public IEnumerable<T> Read<T>(Func<T> entityFactory = null)
        {
            CheckNotDisposed();
            Func<T> localEntityFactory = entityFactory;

            List<T> entities = new List<T>();

            using (DbCommand command = CreateCommand())
            {               
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return entities;
                       
                    while(reader.Read())
                    {
                        T entity = localEntityFactory is null ? default : localEntityFactory();                     
                        entities.Add(readerMapper.Map(reader, entity));
                    }
                }
            }

            Dispose();
            return entities;
        }

        /// <summary>
        /// Выполняет SQL-запрос и возвращает ответ первый ответ указанного типа<para/>
        /// Если объект должен вернуть сложный объект, то нужно передать в аргументы делегат создающий этот объект
        /// </summary>
        /// <typeparam name="T">Тип результата</typeparam>
        /// <param name="entityFactory">Делегат создающий объект</param>
        /// <returns>Ответ указанного типа</returns>
        public T ReadFirst<T>(Func<T> entityFactory = null)
        {
            CheckNotDisposed();
            Func<T> localEntityFactory = entityFactory;

            T entity;

            using (DbCommand command = CreateCommand())
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return default;

                    entity = localEntityFactory is null ? default : localEntityFactory();
                    reader.Read();
                    entity = readerMapper.Map(reader, entity);
                }
            }

            Dispose();
            return entity;
        }

        /// <summary>
        /// Асинхронная версия метода <see cref="Read{T}(Func{T})"/>
        /// </summary>
        public async Task<IEnumerable<T>> ReadAsync<T>(Func<T> entityFactory = null)
        {
            CheckNotDisposed();
            Func<T> localEntityFactory = entityFactory;

            List<T> entities = new List<T>();

            using (DbCommand command = CreateCommand())
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) return entities;
          
                    while (await reader.ReadAsync())
                    {
                        T entity = localEntityFactory is null ? default : localEntityFactory();
                        entities.Add(readerMapper.Map(reader, entity));
                    }        
                }
            }

            Dispose();
            return entities;
        }
        /// <summary>
        /// Асинхронная версия метода <see cref="ReadFirst{T}(Func{T})"/>
        /// </summary>
        public async Task<T> ReadFirstAsync<T>(Func<T> entityFactory = null)
        {
            CheckNotDisposed();
            Func<T> localEntityFactory = entityFactory;

            T entity;

            using (DbCommand command = CreateCommand())
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) return default;

                    entity = localEntityFactory is null ? default : localEntityFactory();

                    await reader.ReadAsync();
                    entity = readerMapper.Map(reader, entity);
                }
            }

            Dispose();
            return entity;
        }

        /// <summary>
        /// Выполняет SQL-запрос, не возвращающий ответ
        /// </summary>
        /// <returns>Количество затронутых строк</returns>
        public int Execute()
        {
            CheckNotDisposed();
            using (DbCommand command = CreateCommand())
            {
                int result = command.ExecuteNonQuery();
                Dispose();
                return result;
            }
        }

        /// <summary>
        /// Асинхронная версия метода <see cref="Execute"/>
        /// </summary>
        public async Task<int> ExecuteAsync()
        {
            CheckNotDisposed();
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

        private void CheckNotDisposed()
        {
            if(disposed)
            {
                throw new ObjectDisposedException(nameof(connection));
            }
        }

        /// <summary>
        /// Закрывает соединение с базой данных
        /// </summary>
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
