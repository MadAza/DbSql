using System;
using System.Data.Common;
using System.Threading.Tasks;

using DbSql.Core.Columns;
using DbSql.Core.Parameters;

namespace DbSql.Core
{
    /// <summary>
    /// Класс для создания объектов <see cref="Core.Query"/>
    /// </summary>
    public class DbController
    {
        internal DbController(IReaderMapper readerMapper, IParameterBuilder parameterBuilder,
          string connectionString, DbProviderFactory providerFactory)
        {
            this.readerMapper = readerMapper ?? throw new ArgumentNullException(nameof(readerMapper));
            this.parameterBuilder = parameterBuilder ?? throw new ArgumentNullException(nameof(parameterBuilder));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            Provider = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
        }

        private readonly IReaderMapper readerMapper;
        private readonly IParameterBuilder parameterBuilder;
        private readonly string connectionString;

        internal DbProviderFactory Provider { get; }

        private DbConnection CreateConnection()
        {
            DbConnection connection = Provider.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
        private async Task<DbConnection> CreateConnectionAsync()
        {
            DbConnection connection = Provider.CreateConnection();
            connection.ConnectionString = connectionString;
            await connection.OpenAsync();
            return connection;
        }

        /// <summary>
        /// Создает запрос к базе данных без текста и параметров 
        /// </summary>
        /// <returns>SQL-запрос</returns>
        public Query Query()
        {
            return new Query(parameterBuilder, readerMapper, CreateConnection());
        }
        /// <summary>
        /// Создает запрос к базе данных с текстом, но без параметров
        /// </summary>
        /// <param name="text">Текст SQL-запроса</param>
        /// <returns>SQL-запрос</returns>
        public Query Query(string text)
        {
            return new Query(parameterBuilder, readerMapper, CreateConnection()).Text(text);
        }
        /// <summary>
        /// Создает запрос к базе данных с текстом и параметрами
        /// </summary>
        /// <param name="text">Текст SQL-запроса</param>
        /// <param name="parameters">Параметры SQL-запроса</param>
        /// <returns>SQL-запрос</returns>
        public Query Query(string text, object parameters)
        {
            return new Query(parameterBuilder, readerMapper, CreateConnection())
                .Text(text).Parameters(parameters);
        }

        /// <summary>
        /// Асинхронная версия метода <see cref="Query"/>
        /// </summary>
        public async Task<Query> QueryAsync()
        {
            return new Query(parameterBuilder, readerMapper, await CreateConnectionAsync());
        }
        /// <summary>
        /// Асинхронная версия метода <see cref="Query(string)"/>
        /// </summary>
        public async Task<Query> QueryAsync(string text)
        {
            return new Query(parameterBuilder, readerMapper, await CreateConnectionAsync()).Text(text);
        }
        /// <summary>
        /// Асинхронная версия метода <see cref="Query(string, object)"/>
        /// </summary>
        public async Task<Query> QueryAsync(string text, object parameters)
        {
            return new Query(parameterBuilder, readerMapper, await CreateConnectionAsync())
                .Text(text).Parameters(parameters);
        }
    }
}