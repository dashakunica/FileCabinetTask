using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "delete";

        private const string Id = "Id";
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string DateOfBirth = "DateOfBirth";

        private const char WhiteSpace = ' ';
        private const char Comma = ',';

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
            var builder = GetId(records);

            string text = builder.Length == 0 ? $"No deleted records." : $"Records {builder.ToString().TrimEnd(WhiteSpace, Comma)} are deleted.";
            this.Service.Delete(records);
        }

        private IEnumerable<FileCabinetRecord> GetRecords(string field, string value)
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

        private StringBuilder GetId(IEnumerable<FileCabinetRecord> records)
        {
            var builder = new StringBuilder();
            foreach (var item in records)
            {
                builder.Append($"#{item.Id}, ");
            }

            return builder;
        }
    }
}
