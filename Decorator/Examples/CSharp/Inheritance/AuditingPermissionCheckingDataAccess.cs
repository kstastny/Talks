using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecoratorExamples.Model;

namespace DecoratorExamples.Inheritance
{
    public class AuditingPermissionCheckingDataAccess : PermissionCheckingDataAccess
    {
        public override ExampleDataObject Get(long id)
        {
            return base.Get(id);
        }

        public override IList<ExampleDataObject> GetAll()
        {
            return base.GetAll();
        }

        public override ExampleDataObject Create()
        {
            FakeLogs.AuditLog("Creating new object");
            return base.Create();
        }

        public override void Save(ExampleDataObject dataObject)
        {
            FakeLogs.AuditLog("Saving " + dataObject);
            base.Save(dataObject);
        }
    }
}