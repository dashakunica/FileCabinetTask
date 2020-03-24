using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string Command = "exit";
        private Action<bool> action;

        public ExitCommandHandler(Action<bool> action)
        {
            this.action = action;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (Command.Equals(commandRequest?.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Exit(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Exit(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Console.WriteLine("Exiting an application...");
            this.action(false);
        }
    }
}
