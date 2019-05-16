using System.Data.Common;

namespace DbSql.Core.Columns
{
    internal interface IReaderMapper
    {
        T Map<T>(DbDataReader reader, T entity);
    }
}
