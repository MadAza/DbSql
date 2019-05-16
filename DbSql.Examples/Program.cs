using DbSql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbSql.Examples
{
    class User
    {
        [SqlColumn("id")]
        public Guid Id { get; set; }
        [SqlColumn("name")]
        public string Name { get; set; }
        [SqlColumn("age")]
        public int Age { get; set; }
    }

    class Program
    {
        private const string CONNECTION_STRING_NAME = "defaultConnection";

        private static void Main(string[] args)
        {
            InsertExample();
            ReadExample();
            ReadExample2();

            Console.ReadLine();
        }

        private static void InsertExample()
        {
            DbController dbController = DbControllers.GetController(CONNECTION_STRING_NAME);

            const string INSERT_QUERY = "INSERT INTO [users] (id, name, age) values (@id, @name, @age)";

            dbController.Query(INSERT_QUERY, new { id = Guid.NewGuid(), name = "Example User1", age = 18 }).Execute();
            dbController.Query(INSERT_QUERY, new { id = Guid.NewGuid(), name = "Example User2", age = 21 }).Execute();
            dbController.Query(INSERT_QUERY, new { id = Guid.NewGuid(), name = "Example User3", age = 34 }).Execute();
        }

        private static void ReadExample()
        {
            DbController dbController = DbControllers.GetController(CONNECTION_STRING_NAME);

            var users = dbController.Query("SELECT * FROM [users] WHERE age > @age",
                new { age = 18 }).Read(() => new User()).ToList();

            foreach (var user in users)
            {
                Console.WriteLine($"Name: {user.Name}. Age: {user.Age}");
            }
        }

        private static void ReadExample2()
        {
            DbController dbController = DbControllers.GetController(CONNECTION_STRING_NAME);

            int usersCount = dbController.Query("SELECT COUNT(*) FROM [users]").ReadFirst<int>();

            Console.WriteLine(usersCount);
        }
    }
}
