using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "update";

        private static readonly PropertyInfo[] FileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (Command.Equals(commandRequest?.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Update(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Update(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var items = QueryParser.UpdateParser(parameters);

            var set = items.propNewValuesPair;
            var where = items.propWhereValuesPair;

            string type = where["type"];

            var newValues = CreateValidateArgs(set);
            var oldRecords = CreateValidateArgs(where);
            var allRecords = this.Service.GetRecords();

            var updatedRecords = QueryParser.GetRecorgs(oldRecords, allRecords, type);
            var builder = new StringBuilder();

            foreach (var item in updatedRecords)
            {
                builder.Append($"#{item.Id}, ");
                var current = CopyAndFillUnusedFields(newValues, item);
                this.Service.EditRecord(item.Id, current);
                Memoization.RefreshMemoization();
            }

            Console.WriteLine(builder.Length == 0 ? string.Empty : $"Records {builder.ToString().TrimEnd(' ', ',')} are updated.");
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

        private static ValidateParametersData CopyAndFillUnusedFields(ValidateParametersData args, FileCabinetRecord record)
        {
            var defaultValidateArgs = new ValidateParametersData();
            var current = args.Clone();
            foreach (var item in ValidateParametersProperties)
            {
                if (Equals(item.GetValue(args), item.GetValue(defaultValidateArgs)))
                {
                    var recordProp = FileCabinetRecordProperties.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase));
                    item.SetValue(current, recordProp.GetValue(record));
                }
            }

            return current;
        }
    }
}
