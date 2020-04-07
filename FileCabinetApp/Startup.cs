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
            if (!File.Exists(Path.Combine(DefaultRootDirectory, ValidationFileName)))
            {
                Console.WriteLine("missingJsonFile");
                Environment.Exit(1488);
            }

            Configuration = new ConfigurationBuilder().AddJsonFile(ValidationRules).Build();
        }

        public static IConfiguration Configuration { get; set; }

        public static string ValidationRules { get; set; } = Path.Combine(DefaultRootDirectory, ValidationFileName);
    }
}
