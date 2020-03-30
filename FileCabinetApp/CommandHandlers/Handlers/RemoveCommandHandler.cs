using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "remove";

        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (Command.Equals(commandRequest?.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Remove(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Remove(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var id = int.Parse(parameters, CultureInfo.InvariantCulture);
            this.Service.RemoveRecord(id);
            Console.WriteLine($"Record #{id} is removed.");
        }
    }
}
