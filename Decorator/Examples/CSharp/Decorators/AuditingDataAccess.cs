using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecoratorExamples.Model;

namespace DecoratorExamples.Decorators
{
    /// <summary>
    /// Naive decorator implementation, adds auditing capability to <see cref="IDataAccess"/>.
    /// </summary>
    public class AuditingDataAccess : IDataAccess
    {
        private readonly IDataAccess internalDataAccess;

        public AuditingDataAccess(IDataAccess internalDataAccess)

        {
            this.internalDataAccess = internalDataAccess;
        }


        public ExampleDataObject Get(long id)
        {
            return internalDataAccess.Get(id);
        }

        public IList<ExampleDataObject> GetAll()
        {
            return internalDataAccess.GetAll();
        }

        public ExampleDataObject Create()
        {
            FakeLogs.AuditLog("Creating new object");
            return internalDataAccess.Create();
        }

        public void Save(ExampleDataObject dataObject)
        {
            FakeLogs.AuditLog("Saving " + dataObject);
            internalDataAccess.Save(dataObject);
            FakeLogs.AuditLog(dataObject + " successfully saved to storage.");
        }
    }
}