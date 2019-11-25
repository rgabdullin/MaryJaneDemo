using System.ComponentModel.DataAnnotations.Schema;

namespace SpectrumCollector
{
    // [Table("SC_SpectrumInfo")]
    public class SpectrumInfo
    {
        public int Id { get; set; }
        public int Channel { get; set; }
        public float Value { get; set; }

        public Measurement Measurement { get; set; }
        public int MeasurementId { get; set; }

        public SpectrumInfo(int channel = -1, float value = 0.0f)
        {
            Channel = channel;
            Value = value;
        }
    }
}
