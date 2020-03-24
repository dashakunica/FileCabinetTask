using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class AppCommandRequest
    {
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public string Command { get; }

        public string Parameters { get; }
    }
}
