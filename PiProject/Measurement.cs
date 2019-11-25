using System;
using System.Collections.Generic;
using System.Linq;

namespace PiProject
{
    public class Measurement
    {
        public int Id { get; set; }
        public DateTime CreationStamp { get; set; } = DateTime.Now;

        public bool IsTrain { get; set; } = false;

        public Dataset Dataset { get; set; } = null;
        public Label Label { get; set; } = null;
        public int? DatasetId { get; set; }
        public int? LabelId { get; set; }

        public List<SpectralData> Data { get; set; } = new List<SpectralData>();

        public List<float> DataToList()
        {
            return Data.OrderBy(a => a.Freq).Select(a => a.Value).ToList();
        }

        public List<Dictionary<string, string>> ToJSON()
        {
            var dict = new Dictionary<string, string>();
            foreach (var data in Data)
                dict[$"{data.Channel}"] = $"{data.Value}";
            dict["Dataset"] = Dataset.Name;

            return new List<Dictionary<string, string>> { dict };
        }
    }
}