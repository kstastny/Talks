using System;
using DecoratorExamples.Decorators;
using DecoratorExamples.Inheritance;
using DecoratorExamples.Model;

namespace DecoratorExamples
{
    class Program
    {
        static void Main(string[] args)
        {
          //  InheritanceExamples();
            DecoratorExamples();
        }

        private static void InheritanceExamples()
        {
            //var dataAccess = new DataAccess();
           // var dataAccess = new Inheritance.AuditingDataAccess();
             var dataAccess = new Inheritance.PermissionCheckingDataAccess();
            //var dataAccess = new Inheritance.AuditingPermissionCheckingDataAccess();

            ExecuteExample(dataAccess);
        }

        private static void DecoratorExamples()
        {
            IDataAccess dataAccess = new DataAccess();
            dataAccess = new Decorators.AuditingDataAccess(dataAccess);
            dataAccess = new Decorators.PermissionCheckingDataAccess(dataAccess);
           

            dataAccess = new Decorators.AutomaticLastUpdateDataAccess(dataAccess);


            //    new Decorators.AuditingDataAccess(
            //        new Decoratorse.PermissionCheckingDataAccess(
            //            new DataAccess())));


            ExecuteExample(dataAccess);
        }

        private static void ExecuteExample(IDataAccess dataAccess)
        {
            foreach (var exampleDataObject in dataAccess.GetAll())
            {
                Console.WriteLine(exampleDataObject);
            }

            var objectId4 = dataAccess.Get(4);
            Console.WriteLine(objectId4);

            try
            {
                var objectId3 = dataAccess.Get(3);
                Console.WriteLine(objectId3);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR! {0}", e.Message);
            }

            var objectNew = dataAccess.Create();
            Console.WriteLine(objectNew);
            objectNew.Name = "Red Sun";
            objectNew.LastUpdatedBy = "me";

            dataAccess.Save(objectNew);
            Console.WriteLine(objectNew);


            foreach (var exampleDataObject in dataAccess.GetAll())
            {
                Console.WriteLine(exampleDataObject);
            }
        }
    }
}