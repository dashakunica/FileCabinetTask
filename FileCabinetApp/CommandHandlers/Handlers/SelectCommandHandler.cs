using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "select";

        private const string Id = "Id";
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string DateOfBirth = "DateOfBirth";

        private const char WhiteSpace = ' ';
        private const char Comma = ',';

        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
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
                this.Select(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Select(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            (var fields, var values) = QueryParser.
        }


    }
}