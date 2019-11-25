using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiProject
{
    public class Label
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
        public List<Measurement> Measurements { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
