# DbSql

DbSql это легковесная библиотека для работы с базой данных.

Для работы с базами данных используется класс DbController.
Чтобы получить объект DbController используется класс DbControllers, который возвращает DbController по названию строки подключения.

### Например:


```cs
DbController controller = DbControllers.GetController("yourConnectionStringName");
```

### Чтение данных

Создадим класс для представления сущности в базе данных.

```cs
class User
{
  [SqlColumn("id")]
  [SqlParameter("id")]
  public Guid Id { get; set; }

  [SqColumn("name")]
  [SqlParameter("name")]
  public string Name { get; set; }
  
  [SqlColumn("age")]
  [SqlParameter("age")]
  public int Age { get; set; }
}
```
А теперь выберем пользователей, которые старше 18 лет.

```cs
var users = controller.Query("select * from users where age > @age", new { age = 18 }).Read(() => new User());
```

Выберем количество всех пользователей

```cs
int usersCount = controller.Query("select count(*) users").ReadFirst<int>();
```

### Выполнение запросов

Добавим пользователя

```cs
controller.Query("insert into users (id, name, age) values(@id, @name, @age)", 
          new User { Id = Guid.NewGuid(), Name = "Simple User", Age = 21 } ).Execute();
```
Удалим несовершеннолетних пользователей

```cs
controller.Query("delete from users where age < @required_age", new { required_age = 18 }).Execute();
```
