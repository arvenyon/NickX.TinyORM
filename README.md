# TinyORM - Simple, lightweight .NET MicroORM for SQL-Server

## Features
* Fluent or Attribute Mapping between classes and SQL tables
* Generic repository pattern as persistence layer, already implemented
* Attach to existing databases
* Database creation based on your mapping & definitions
* Events for CRUD operations which you can hook into

## Dependencies
* [FastMember >=1.5.0](https://www.nuget.org/packages/FastMember/)
* [System.Data.SqlClient >=4.8.3](https://www.nuget.org/packages/System.Data.SqlClient/)
* [System.Linq.Expressions >=4.3.0](https://www.nuget.org/packages/System.Linq.Expressions/)
* [NuGet Gallery >= 2.13.14](https://www.nuget.org/packages/Humanizer/2.13.14)

## Super Quick Start
1) Get your SQL ConnectionString
```c#
var conString = "Database=myDatabase;Server=myServer,1433;User Id=myUser;Password=mySuperSecretPassword123"
```

2) Define your Mapping via Fluent API
```c#
var fluentMapping = new FluentMapping()
    .MapTable<Foo>("tblFoos")
		.MapColumn(c => c.Id, allowsNull: false, length: default, DefaultValues.AutoIncrement)
		.MapColumn(c => c.Firstname, "AnotherNameForYourColumn")
		.MapColumn(c => c.DateCreate, allowsNull: false, length: default, DefaultValues.Timestamp)
		.SetPrimaryKey(c => c.Id)
		.PackUp()
	.MapTable<Bar>()
		.MapColumn(c => c.Id, allowsNull: false, length: default, DefaultValues.AutoIncrement)
		.MapColumn(c => c.FooId, allowsNull: false, length: default, DefaultValues.None);
		.SetPrimaryKey(c => c.Id)
		.AddForeignKey<Foo>(b => b.FooId, f => f.Id)
		.PackUp();
```

3) Initialize a Connection Factory
```c#
var conFactory = new SqlConnectionFactory(
	connectionString: conString,
	mapping: fluentMapping,
	createDb: true
);
```

4) Grab your repositories and start manipulating data
```c#
var foos = new SqlRepository<Foo>(conFactory);

var newFoo = new Foo()
{
	Firstname = "Biggus",
	Lastname = "Dickus"
};

int insertedKey = (int)foos.Insert(newFoo);
```
## Simplified Overview
<img src="https://i.ibb.co/h2GwcfD/Tiny-ORM-Simplified-Visualization.png" />

## CRUD Operations
All well known CRUD functionality is packed into repositories.
You can instantiate a repository for any type that you've mapped via your FluentMapping:
```c#
var foos = new SqlRepository<Foo>(sqlConnectionFactory);
```

### Select
```c#
var foos = new SqlRepository<Foo>(sqlConnectionFactory);

// Get All
var allFoos = foos.All();

// Get an object by Key
var fooByKey = foos.Single(14);

// Get a single object by condition
var condition = foos.CreateQueryConditionBuilder()
	.Start(f => f.Firstname, QueryOperators.Equals, "Biggus")
	.And(f => f.Lastname, QueryOperators.Equals, "Dickus");
var fooByCondition = foos.Single(condition);

// Get multiple by condition
var condition = foos.CreateQueryConditionBuilder()
	.Start(f => f.DateCreate, QueryOperators.LowerThanOrEquals, DateTime.Now.AddYears(-1))
	.Or(f => f.Status, QueryOperators.Equals, Status.Inactive);
var foosByCondition = foos.Multiple(condition);
	
```

### Insert
```c#
var foos = new SqlRepository<Foo>(sqlConnectionFactory);

var newFoo = new Foo()
{
	Firstname = "Incontinentia",
	Lastname = "Buttocks"
};
int insertedId = (int)foos.Insert(newFoo);
```

### Update
```c#
var foos = new SqlRepository<Foo>(sqlConnectionFactory);

var existingFoo = foos.Single(14);
existingFoo.Firstname = "AnotherFirstname";

foos.Update(existingFoo);
```

### Delete
```c#
var foos = new SqlRepository<Foo>(sqlConnectionFactory);

var condition = foos.CreateQueryConditionBuilder()
	.Start(f => f.Status, QueryOperators.Equals, Status.Inactive);
var toDelete = foos.Single(condition);

foos.Delete(toDelete);
```

### Exists
```c#
var foos = new SqlRepository<Foo>(sqlConnectionFactory);

var condition = foos.CreateQueryConditionBuilder()
	.Start(f => f.Status, QueryOperators.Equals, Status.Inactive);
	
if (foos.Exists(condition))
{
	// go on and do stuff
}
```

## Query Condition
Queries often depend on conditions. In order to not need to use any strong typed SQL statements anywhere, repositories provide a QueryConditionBuilder for their mapped Type.

### Usage
At this point you can define your conditions via (again) a Fluent API.
```c#
var fooRepository = new SqlRepository<Foo>(sqlConnectionFactory);

var condition = foos.CreateQueryConditionBuilder()
	.Start(f => f.Status, QueryOperators.Equals, Status.Inactive);
```

### QueryOperators
Following QueryOperators are available.
```c#
public enum QueryOperators
{
	Equals,
	NotEquals,
	Contains,
	NotContains,
	StartsWith,
	NotStartsWith,
	EndsWith,
	NotEndsWith,
	GreaterThan,
	LessThan,
	GreaterThanOrEqual,
	LessThanOrEqual
}
```

## Events
Insert, Update & Delete operations all have events attached to them. They're provided by the repository.

> Inserted & Updated will always be the Type you've gotten the repository for
```c#
var foos = new SqlRepository<Foo>(conFactory);

// after insert
foos.OnInsert += (sender, inserted) =>
{
	Console.WriteLine("Inserted Foo:");
	Console.WriteLine(inserted);
}

// after update
foos.OnUpdate += (sender, inserted, deleted) => 
{
	Console.WriteLine("Updated Foo:");
	Console.WriteLine(inserted);
	Console.WriteLine("Old Foo:");
	Console.WriteLine(deleted);
}

// after delete
foos.OnDelete += (sender, deleted) =>
{
	Console.WriteLine("Deleted Foo:");
	Console.WriteLine(deleted);
}
```

## Issues, Bugs & Requests
Please consider creating an issue if you encounter any problems, bugs, errors or inconveniences. I will happily continue supporting this project.

## Disclaimer
This is a personal project which initially started out as a project for learning purposes. Take this information for whatever you want it to. :)
