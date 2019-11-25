using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiProject
{
    public class Dataset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public List<Label> Labels { get; set; }
        public List<Measurement> Measurements { get; set; }
    }
}
