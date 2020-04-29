using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

namespace FileCabinetApp
{
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "delete";

        private const string Id = "Id";

        private const char WhiteSpace = ' ';
        private const char Comma = ',';

        private static readonly PropertyInfo[] FileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

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

            var where = QueryParser.DeleteParser(parameters);

            string idValue;
            if (where.TryGetValue(Id, out idValue))
            {
                int temp;
                if (!int.TryParse(idValue, out temp))
                {
                    Console.WriteLine("Invalid Id value.");
                }

                this.Service.RemoveRecord(temp);
            }

            string type = where["type"];
            var whereRecord = CreateValidateArgs(where);
            var allRecords = this.Service.GetRecords();

            var records = QueryParser.GetRecorgs(whereRecord, allRecords, type);

            var builder = GetId(records);

            string text = builder.Length == 0 ? $"No deleted records." : $"Records {builder.ToString().TrimEnd(WhiteSpace, Comma)} are deleted.";
            this.Service.Delete(records);
            Memoization.RefreshMemoization();
            Console.WriteLine(text);
        }

        private static ValidateParametersData CreateValidateArgs(Dictionary<string, string> propNewValues)
        {
            var arg = new ValidateParametersData();
            foreach (var item in propNewValues)
            {
                var prop = ValidateParametersProperties.FirstOrDefault(x => x.Name.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase));
                var converter = TypeDescriptor.GetConverter(prop?.PropertyType);
                prop.SetValue(arg, converter.ConvertFromString(item.Value));
            }

            return arg;
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
