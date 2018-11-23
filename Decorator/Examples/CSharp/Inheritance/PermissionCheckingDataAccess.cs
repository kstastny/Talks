using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecoratorExamples.Model;

namespace DecoratorExamples.Inheritance
{
    public class PermissionCheckingDataAccess : DataAccess
    {
        private readonly HashSet<long> prohibitedIDs = new HashSet<long>
        {
            2,
            3
        };


        public override IList<ExampleDataObject> GetAll()
        {
            return base.GetAll()
                .Where(e => prohibitedIDs.Contains(e.Id) == false)
                .ToList();
        }

        public override ExampleDataObject Get(long id)
        {
            if (prohibitedIDs.Contains(id))
                throw new Exception("Accessing object with id " + id + " is not allowed.");

            return base.Get(id);
        }

        public override void Save(ExampleDataObject dataObject)
        {
            if (prohibitedIDs.Contains(dataObject.Id))
                throw new Exception("Accessing object with id " + dataObject.Id + " is not allowed.");

            base.Save(dataObject);
        }
    }
}