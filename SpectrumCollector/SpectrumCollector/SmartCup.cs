using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SpectrumCollector
{
    public class SmartCup
    {
        StreamSocket socket;
        public DataReader Reader { get; set; }
        public DataWriter Writer { get; set; }

        public SmartCup(StreamSocket connection)
        {
            socket = connection;
            Reader = new DataReader(socket.InputStream);
            Writer = new DataWriter(socket.OutputStream);

            Reader.ByteOrder = ByteOrder.LittleEndian;
            Writer.ByteOrder = ByteOrder.LittleEndian;
        }

        public async Task LEDControl(int pin, int value)
        {
            if((pin >= 0) && (value >= 0) && ((pin < 8) && (value < 3) || (pin == 8) && (value < 2)))
            {
                string msg = $"a{pin}{value}.";
                Debug.WriteLine($"Sending message '{msg}'");

                try { 
                    Writer.WriteString(msg);
                    await Writer.StoreAsync();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine($"Error while writing: '{ee.Message}'");
                    ApplicationSettings.Device = null;
                }
            }
        }

        public async Task<List<float>> ReadSensors()
        {
            try {
                Writer.WriteString("b...");
                

                await Writer.StoreAsync();
            }
            catch (Exception ee)
            {
                Debug.WriteLine($"Error while writing: '{ee.Message}'");
                ApplicationSettings.Device = null;
            }

            List<float> result = new List<float>();

            Debug.WriteLine($"Length: {Reader.UnconsumedBufferLength}");
            try
            {
                await Reader.LoadAsync(4 * 3);
            }
            catch (Exception ee)
            {
                Debug.WriteLine($"Error while reading: '{ee.Message}'");
                ApplicationSettings.Device = null;
            }
            Debug.WriteLine($"Length: {Reader.UnconsumedBufferLength}");

            for (int i = 0; i < 3; ++i)
            {
                float value = Reader.ReadSingle();
                result.Add(value);
            }

            return result;
        }

        public async Task<Measurement> GetSpectrum(int numObs, int lightTime, int delayTime)
        {
            // string msg = $"c{(char)('a' + numObs)}{(char)lightTime}{(char)delayTime}";
            // Debug.WriteLine($"Sending command: '{msg}'");

            try
            {
                byte[] msg = new byte[4];

                msg[0] = 0x6;
                msg[1] = 5;
                msg[2] = 25;
                msg[3] = 255;

                Writer.WriteBytes(msg);
                await Writer.StoreAsync();
            }
            catch (Exception ee) {
                Debug.WriteLine($"Error while writing: '{ee.Message}'");
                ApplicationSettings.Device = null;
            }

            
            try {
                await Reader.LoadAsync(sizeof(float) * 36);
            }
            catch (Exception ee) {
                Debug.WriteLine($"Error while reading: '{ee.Message}'");
                ApplicationSettings.Device = null;
            }

            var mes = new Measurement
            {
                CreationStamp = DateTime.Now,
                Dataset = "default",
                Label = "auto",
                Description = "No description",
            };
            var res = new ProcessingResult
            {
                Measurement = mes,
                Description = "Processing...",
                Status = 0,
            };

            List<SpectrumInfo> data = new List<SpectrumInfo>();
            for (int i = 0; i < 36; ++i)
            {
                float value = Reader.ReadSingle();
                Debug.WriteLine($"Got value #{i + 1}: {value}");
                data.Add(new SpectrumInfo
                {
                    Measurement = mes,
                    Channel = i,
                    Value = value
                });
            }

            mes.Data = data;
            mes.Result = res;

            Debug.WriteLine("Got SpectrumData!");
            return mes;
        }

        public static async Task ConnectSmartCup()
        {
            while (ApplicationSettings.Device == null)
            {
                BluetoothConnectionDialog dialog = new BluetoothConnectionDialog();
                await dialog.ShowAsync();
                ApplicationSettings.Device = new SmartCup(dialog.ConnectionSocket);
            }
        }
    }
}
