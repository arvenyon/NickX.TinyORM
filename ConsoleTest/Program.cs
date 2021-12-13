using ConsoleTest.Models;
using NickX.TinyORM.Mapping.Classes.Annotation;
using NickX.TinyORM.Mapping.Classes.Fluent;
using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Persistence.Connections.Classes;
using NickX.TinyORM.Persistence.Queries;
using NickX.TinyORM.Persistence.Repositories.Classes;
using System;

#pragma warning disable RCS1102

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var conFactory = new SqlConnectionFactory(@"Server=localhost\dev;Database=__AnnotationMappingTEST;Integrated Security=True", true);
            var mapping = (AnnotationMapping)conFactory.Mapping;
            mapping.RegisterType<CompanyModel>();
            mapping.RegisterType<PersonModel>();

            var companyRepository = new SqlRepository<CompanyModel>(conFactory);
            var personRepository = new SqlRepository<PersonModel>(conFactory);

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

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
