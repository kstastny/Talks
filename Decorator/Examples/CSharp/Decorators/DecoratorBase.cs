using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecoratorExamples.Model;

namespace DecoratorExamples.Decorators
{
    /// <summary>
    /// Base class for implementing decorator pattern for <see cref="IDataAccess"/>.
    /// </summary>
    public class DecoratorBase : IDataAccess
    {
        private readonly IDataAccess internalDataAccess;

        public DecoratorBase(IDataAccess internalDataAccess)
        {
            this.internalDataAccess = internalDataAccess;
        }

        public virtual ExampleDataObject Get(long id)
        {
            return internalDataAccess.Get(id);
        }

        public virtual IList<ExampleDataObject> GetAll()
        {
            return internalDataAccess.GetAll();
        }

        public virtual ExampleDataObject Create()
        {
            return internalDataAccess.Create();
        }

        public virtual void Save(ExampleDataObject dataObject)
        {
            internalDataAccess.Save(dataObject);
        }
    }
}
