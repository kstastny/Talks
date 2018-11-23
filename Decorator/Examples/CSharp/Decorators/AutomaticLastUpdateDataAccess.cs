using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecoratorExamples.Model;

namespace DecoratorExamples.Decorators
{
    public class AutomaticLastUpdateDataAccess : DecoratorBase
    {
        public AutomaticLastUpdateDataAccess(IDataAccess internalDataAccess) : base(internalDataAccess)
        {
        }

        public override void Save(ExampleDataObject dataObject)
        {
            dataObject.LastUpdatedBy = Environment.UserName;
            FakeLogs.LogActivity("Automatically filled in username to " + dataObject);
            base.Save(dataObject);
        }
    }
}