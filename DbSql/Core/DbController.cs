using System;
using System.Data.Common;
using System.Threading.Tasks;

using DbSql.Core.Columns;
using DbSql.Core.Parameters;

namespace DbSql.Core
{
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

        public Query Query()
        {
            return new Query(parameterBuilder, readerMapper, CreateConnection());
        }
        public Query Query(string text)
        {
            return new Query(parameterBuilder, readerMapper, CreateConnection()).Text(text);
        }
        public Query Query(string text, object parameters)
        {
            return new Query(parameterBuilder, readerMapper, CreateConnection())
                .Text(text).Parameters(parameters);
        }

        public async Task<Query> QueryAsync()
        {
            return new Query(parameterBuilder, readerMapper, await CreateConnectionAsync());
        }
        public async Task<Query> QueryAsync(string text)
        {
            return new Query(parameterBuilder, readerMapper, await CreateConnectionAsync()).Text(text);
        }
        public async Task<Query> QueryAsync(string text, object parameters)
        {
            return new Query(parameterBuilder, readerMapper, await CreateConnectionAsync())
                .Text(text).Parameters(parameters);
        }
    }
}