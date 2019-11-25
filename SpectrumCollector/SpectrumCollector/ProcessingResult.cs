using System.ComponentModel.DataAnnotations.Schema;

namespace SpectrumCollector
{
    // [Table("SC_ProcessingResult")]
    public class ProcessingResult
    {
        public int Id { get; set; }
        public string Description { get; set; } = "No description";
        public int Status { get; set; } = 0;

        public Measurement Measurement { get; set; } = null;
        public int MeasurementId { get; set; }
    }
}