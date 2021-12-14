using ConsoleTest.Models;
using Microsoft.Extensions.Configuration;
using NickX.TinyORM.Mapping.Classes.Annotation;
using NickX.TinyORM.Persistence.Connections.Classes;
using NickX.TinyORM.Persistence.Repositories.Classes;
using System;

#pragma warning disable RCS1102

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .Build();

            var conString = config.GetConnectionString("LocalhostDev");

            var conFactory = new SqlConnectionFactory(conString, true);
            var mapping = (AnnotationMapping)conFactory.Mapping;
            mapping.RegisterType<CompanyModel>();
            mapping.RegisterType<PersonModel>();

            var companyRepository = new SqlRepository<CompanyModel>(conFactory);
            var personRepository = new SqlRepository<PersonModel>(conFactory);

            #region EventHandling
            companyRepository.OnInsert += (sender, inserted) =>
            {
                Console.WriteLine("Company has been inserted: ");
                Console.WriteLine(inserted);
            };
            companyRepository.OnUpdate += (sender, inserted, deleted) =>
            {
                Console.WriteLine("Old Entity: ");
                Console.WriteLine(deleted);
                Console.WriteLine("New Entity: ");
                Console.WriteLine(inserted);
            };
            companyRepository.OnDelete += (sender, deleted) =>
            {
                Console.WriteLine("Deleted Entity:");
                Console.WriteLine(deleted);
            };
            #endregion


            //// GET ALL
            //var all = companyRepository.All();

            //// DELETE & GET SINGLE
            //var toDelete = companyRepository.Single(Guid.Parse("66393AA8-BC5C-EC11-839C-00E04CC3A503"));
            //companyRepository.Delete(toDelete);

            //// GET MULTIPLE
            //var multiple = companyRepository.Multiple(
            //    companyRepository.CreateQueryConditionBuilder()
            //        .Start(c => c.CompanyType, QueryOperators.Equals, CompanyTypes.Customer));

            //// EXISTS
            //var exists = companyRepository.Exists(companyRepository.CreateQueryConditionBuilder()
            //    .Start(c => c.CompanyType, QueryOperators.Equals, CompanyTypes.Customer));

            //// INSERT
            //var newCompany = new CompanyModel()
            //{
            //    Name = "Example Company GmbH",
            //    CompanyType = CompanyTypes.Customer,
            //    DateCreate = DateTime.Now,
            //    DateModified = DateTime.Now,
            //    Description = "Just an example company."
            //};
            //var insertedGuid = companyRepository.Insert(newCompany);

            //// UPDATE
            //var current = companyRepository.Single(Guid.Parse("C342D848-C15C-EC11-839C-00E04CC3A503"));
            // current.Name = "NOT ExampelCompany GmbH";
            // companyRepository.Update(current);


            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
