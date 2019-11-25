using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SpectrumCollector
{
    public class Measurement
    {
        public int Id { get; set; }

        public string Dataset { get; set; } = "default";
        public string Label { get; set; } = "No label";
        public string Description { get; set; } = "No description";
        public DateTime CreationStamp { get; set; } = DateTime.Now;

        // навигационные свойства 
        public List<SpectrumInfo> Data { get; set; } = new List<SpectrumInfo>();
        public ProcessingResult Result { get; set; } = null;

        public List<float> DataToList()
        {
            return Data.OrderBy(a => a.Channel).Select(a => a.Value).ToList();
        }

        public List<Dictionary<string, string>> ToJSON()
        {
            var dict = new Dictionary<string, string>();
            foreach(var data in Data)
                dict[$"Ch{data.Channel}"] = $"{data.Value}";

            return new List<Dictionary<string, string>> { dict };
        }
    }
}