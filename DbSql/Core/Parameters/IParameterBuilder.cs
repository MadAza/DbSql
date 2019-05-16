using System.Data.Common;

namespace DbSql.Core.Parameters
{
    internal interface IParameterBuilder
    {
        DbParameter[] Build(object parametersObject);
    }
}
