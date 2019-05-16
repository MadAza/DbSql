using System.Linq;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;

using DbSql.Core.Columns;
using DbSql.Core.Parameters;

namespace DbSql.Core
{
    /// <summary>
    /// Класс для получения объектов <see cref="DbController"/>
    /// </summary>
    public static class DbControllers
    {
        static DbControllers()
        {
            dbControllers = new List<DbController>();
            locker = new object();
        }

        private static readonly List<DbController> dbControllers;
        private static readonly object locker;

        /// <summary>
        /// Возвращает объект <see cref="DbController"/> по указанной строке подключения
        /// </summary>
        /// <param name="connectionStringName">Название строки подключения</param>
        /// <returns></returns>
        public static DbController GetController(string connectionStringName)
        {
            lock(locker)
            {
                ConnectionStringSettings connectionStringSettings = 
                    ConfigurationManager.ConnectionStrings[connectionStringName];

                DbProviderFactory dbProvider = 
                    DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

                DbController dbController = 
                    dbControllers.FirstOrDefault(controller => controller.Provider == dbProvider);

                if(dbController is null)
                {
                    dbController = new DbController(
                        new ReaderMapper(),
                        new ParameterBuilder(() => dbProvider.CreateParameter()),
                        connectionStringSettings.ConnectionString,
                        dbProvider);

                    dbControllers.Add(dbController);
                }

                return dbController;
            }
        }
    }
}
