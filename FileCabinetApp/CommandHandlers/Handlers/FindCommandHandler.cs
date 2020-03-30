using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "find";

        private const string FirstNameString = "FIRSTNAME";
        private const string LastNameString = "LASTNAME";
        private const string DateOfBirthString = "DATEOFBIRTH";
        private const char WhiteSpace = ' ';

        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
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
                this.Find(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Find(string parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var inputs = parameter.Split(WhiteSpace, 2);
            int indexPropertie = 0;
            int indexParameter = 1;

            ReadOnlyCollection<FileCabinetRecord> records;

            if (inputs[indexPropertie].Equals(FirstNameString, StringComparison.InvariantCultureIgnoreCase))
            {
                records = this.Service.FindByFirstName(inputs[indexParameter]);
                this.printer(records);
            }
            else if (inputs[indexPropertie].Equals(LastNameString, StringComparison.InvariantCultureIgnoreCase))
            {
                records = this.Service.FindByLastName(inputs[indexParameter]);
                this.printer(records);
            }
            else if (inputs[indexPropertie].Equals(DateOfBirthString, StringComparison.InvariantCultureIgnoreCase))
            {
                records = this.Service.FindByDateOfBirth(Convert.ToDateTime(inputs[indexParameter], CultureInfo.CreateSpecificCulture("en-US")));
                this.printer(records);
            }
        }
    }
}
