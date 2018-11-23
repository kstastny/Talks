using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DecoratorExamples.Model
{
    public class DataAccess : IDataAccess
    {
        private readonly IList<ExampleDataObject> testData =
            new List<ExampleDataObject>
            {
                CreateTestData(1, "Lion", "system"),
                CreateTestData(2, "Direwolf", "system"),
                CreateTestData(3, "Dragon", "system"),
                CreateTestData(4, "Kraken", "system"),
                CreateTestData(5, "Stag", "system"),
                //Rose, Sun, Falcon
            };

        private static ExampleDataObject CreateTestData(long id, string name, string createdByName)
        {
            return new ExampleDataObject
            {
                Id = id,
                Name = name,
                LastUpdatedBy = createdByName
            };
        }

        public virtual ExampleDataObject Create()
        {
            FakeLogs.LogActivity("Creating new data object");
            var obj = new ExampleDataObject();


            return obj;
        }

        public virtual ExampleDataObject Get(long id)
        {
            FakeLogs.LogActivity("Locating data object with ID " + id);
            return testData.FirstOrDefault(e => e.Id == id);
        }

        public virtual IList<ExampleDataObject> GetAll()
        {
            FakeLogs.LogActivity("Returning all data objects");
            return new ReadOnlyCollection<ExampleDataObject>(testData);
        }

        public virtual void Save(ExampleDataObject dataObject)
        {
            FakeLogs.LogActivity("Saving data object " + dataObject);
            if (dataObject.Id <= 0)
            {
                dataObject.Id = testData.Count + 1;
                testData.Add(dataObject);
            }
            else
            {
                var index = GetIndexById(dataObject.Id);
                if (index >= 0)
                {
                    testData[index] = dataObject;
                }
                else
                {
                    throw new Exception("Data object " + dataObject + " was not found and cannot be saved.");
                }
            }
        }

        private int GetIndexById(long id)
        {
            for (int index = 0; index < testData.Count; index++)
            {
                var exampleDataObject = testData[index];
                if (exampleDataObject.Id == id)
                    return index;
            }

            return -1;
        }
    }
}