using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiProject
{
    public class SpectralData
    {
        private int channel = -1;

        public int Id { get; set; }
        public int LedId { get; set; }
        public int Channel
        {
            get => channel;
            set
            {
                channel = value;
                Freq = GetChannelFreq(value);
            }
        }
        public float Freq { get; set; } = -1.0f;
        public float Value { get; set; } = 0.0f;

        public Measurement Measurement { get; set; }
        public int MeasurementId { get; set; }

        public static float GetChannelFreq(int channel)
        {
            if ((channel >= 0) && (channel < 18))
            {
                var lst = new float[18] { 610, 680, 730, 760, 810, 860, 560, 585, 645, 705, 900, 940, 410, 435, 460, 485, 510, 535 };
                return lst[channel];
            }
            return -1.0f;
        }
    }
}
