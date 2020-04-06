using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    public static class Startup
    {
        private static string DefaultRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string ValidationFileName = @"validation-rules.json";

        static Startup()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile(ValidationRules).Build();
        }

        public static IConfiguration Configuration { get; set; }

        public static string ValidationRules { get; set; } = Path.Combine(DefaultRootDirectory, ValidationFileName);
    }
}
