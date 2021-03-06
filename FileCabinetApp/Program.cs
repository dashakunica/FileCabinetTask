﻿using System;

namespace FileCabinetApp
{
    /// <summary>
    /// API.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Darya Kunickaya";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        /// <summary>
        /// Entry poin of app.
        /// </summary>
        /// <param name="args">Args.</param>
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException($"{nameof(args)} cannot be null.");
            }

            Startup.Run(args);

            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}.");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                var commandHandler = Startup.CreateCommandHandlers();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                const int parametersIndex = 1;
                string parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest(command, parameters));
                Console.WriteLine();
            }
            while (Startup.IsRunning);
        }
    }
}