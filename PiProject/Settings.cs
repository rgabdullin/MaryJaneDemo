using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;

namespace PiProject
{
    public static class Settings
    {
        public static PiCup PiCup { get; set; }
        public static int IntegrationTime { get; set; }
        public static DbContextOptions<DatabaseContext> SqlOptions { get; set; }

        public static string AzureMLApiKey { get; set; }
        public static string AzureMLAddress { get; set; }

        static Settings()
        {
            Debug.WriteLine("Settings class static constructor");

            PiCup = null;
            IntegrationTime = 20;

            AzureMLApiKey = "sNM0EYl3FRxKt5Iv86FEC1H+zcn2UZV5E5PuIYvTWQESla0m2ZV0L2ry0HZLmuqMMVMqG9PLQJnZUfShwwxdtw==";
            AzureMLAddress = "https://europewest.services.azureml.net/workspaces/cebad76a8f3140ceb906646e5392d5a2/services/816cdbf6c60442c0a783e84d3c59a264/execute?api-version=2.0&format=swagger";
            SqlOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite("Filename=Mobile.db").Options;
                
        }
    }
}
