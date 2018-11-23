using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorExamples.Model
{
    public class ExampleDataObject
    {
        public long Id { get; set; }

        /// <summary>
        /// Name of the object
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Who did create or update the object
        /// </summary>
        public string LastUpdatedBy { get; set; }

        public override string ToString()
        {
            return $"[{Id}],Name={Name},Last updated by {LastUpdatedBy}";
        }
    }
}
