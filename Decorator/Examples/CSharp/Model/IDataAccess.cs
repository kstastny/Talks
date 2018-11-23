using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorExamples.Model
{
    public interface IDataAccess
    {
        /// <summary>
        /// Returns data object by identifier, or null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ExampleDataObject Get(long id);

        /// <summary>
        /// Returns all data objects.
        /// </summary>
        /// <returns></returns>
        IList<ExampleDataObject> GetAll();

        /// <summary>
        /// Creates new data object.
        /// </summary>
        /// <returns></returns>
        ExampleDataObject Create();

        /// <summary>
        /// Saves data object to data store.
        /// </summary>
        /// <param name="dataObject"></param>
        void Save(ExampleDataObject dataObject);
    }
}