using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SpectrumCollector
{
    public static class ApplicationSettings
    {
        public static SmartCup Device { get; set; } = null;
        public static string ServerURL { get; set; } = "http://yc.ruslixag.com/";
        public static DbContextOptions<DatabaseContext> SqlOptions { get; set; } = null;

        public static int NumObsDefaultValue { get; set; } = 1;
        public static int LightTimeDefaultValue { get; set; } = 25;
        public static int DelayTimeDefaultValue { get; set; } = 25;
        public static string ScoreWebServiceApiKey { get; set; } = "BtGI7M2vWrAmT0Y3CPDWuAhb9wVHXoz2sVT2GPaIh/0H9OJs//XKK9FRx18ivBpFjrrw3AUmFqR+2Wm5K8vwog==";

        static ApplicationSettings()
        {
            Debug.WriteLine("Initializing ApplicationSettings class");

            SqlOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite("Filename=SpectrumDatabase.db").Options;
        }
    }
}
