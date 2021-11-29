using ConsoleTest.Models;
using NickX.TinyORM.Mapping.Classes.Fluent;
using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Persistence.Connections.Classes;
using NickX.TinyORM.Persistence.Queries;
using NickX.TinyORM.Persistence.Repositories.Classes;
using System;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mapping = new FluentMapping()
                .MapTable<CompanyModel>()
                    .SetPrimaryKey(p => p.Guid, null, default, DefaultValues.UniqueIdentifier)
                    .MapColumn(p => p.DateCreate, false, default, DefaultValues.Timestamp)
                    .MapColumn(p => p.DateModified, false, default, DefaultValues.Timestamp)
                    .PackUp()
                .MapTable<PersonModel>()
                    .SetPrimaryKey(p => p.Guid, null, default, DefaultValues.UniqueIdentifier)
                    .MapColumn(p => p.DateCreate, false, default, DefaultValues.Timestamp)
                    .MapColumn(p => p.DateModified, false, default, DefaultValues.Timestamp)
                    .AddForeignKey<CompanyModel>(p => p.CompanyGuid, p => p.Guid)
                    .PackUp();

            var conFactory = new SqlConnectionFactory(@"Server=localhost\dev;Database=CompaniesDB;Integrated Security=True", mapping, true);

            var companyRepository = new SqlRepository<CompanyModel>(conFactory);
            var personRepository = new SqlRepository<PersonModel>(conFactory);

            foreach (var company in companyRepository.All())
            {
                Console.WriteLine("------------- [Company] -------------");
                Console.WriteLine(company);

                var condition = personRepository.CreateQueryConditionBuilder()
                    .Start(p => p.CompanyGuid, QueryOperators.Equals, company.Guid);
                foreach (var person in personRepository.Multiple(condition))
                {
                    Console.WriteLine("----------------------------------------------------------- [Person]");
                    Console.WriteLine(person);
                }
            }

            //var company = new CompanyModel()
            //{
            //    Name = "Geekbuild GmbH",
            //    CompanyType = CompanyTypes.Customer
            //};

            //var guid = (Guid)companyRepository.Insert(company);

            //var person = new PersonModel()
            //{
            //    Firstname = "Nick",
            //    Lastname = "Huegin",
            //    Title = "Dr.",
            //    IsContactPerson = true,
            //    CompanyGuid = guid
            //};

            //personRepository.Insert(person);


            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
