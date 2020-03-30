using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "list";

        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (Command.Equals(commandRequest?.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.List(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void List(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.printer(this.Service.GetRecords());
        }
    }
}
