using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers.Handlers
{
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "delete";

        private const string Id = "Id";
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string DateOfBirth = "DateOfBirth";

        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Delete(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Delete(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            (string field, string value) = QueryParser.DeleteParser(parameters);

            var record = new FileCabinetRecord();
            if (field.Equals(Id, StringComparison.InvariantCultureIgnoreCase))
            {
                int temp;
                if (!int.TryParse(value, out temp))
                {
                    Console.WriteLine("idException");
                }

                this.Service.RemoveRecord(temp);
            }

            var records = GetRecords(field, value);
            this.Service.Delete(records);
        }

        public IEnumerable<FileCabinetRecord> GetRecords(string field, string value)
        {
            IEnumerable<FileCabinetRecord> records = default;

            if (field.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
            {
                records = this.Service.FindByFirstName(value);
            }
            else if (field.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
            {
                records = this.Service.FindByLastName(value);
            }
            else if (field.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
            {
                DateTime temp;
                if (!DateTime.TryParse(value, out temp))
                {
                    Console.WriteLine("dateOfBirthException");
                }

                records = this.Service.FindByDateOfBirth(temp);
            }
            else
            {
                Console.WriteLine("unknownArgument", field);
            }

            return records;
        }
    }
}
