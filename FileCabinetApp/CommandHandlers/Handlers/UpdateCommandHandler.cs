using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Update command handler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "update";

        private static readonly PropertyInfo[] FileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
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

            var newValues = DataHelper.CreateRecordFromDict(set);
            var oldRecords = DataHelper.CreateRecordFromDict(where);
            var allRecords = this.Service.GetRecords();

            var updatedRecords = QueryParser.GetRecorgs(oldRecords, allRecords, type);
            var builder = new StringBuilder();

            foreach (var item in updatedRecords)
            {
                builder.Append($"#{item.Id}, ");
                var current = this.CopyAndFillUnusedFields(newValues, item);
                this.Service.EditRecord(item.Id, current);
                Memoization.RefreshMemoization();
            }

            Console.WriteLine(builder.Length == 0 ? string.Empty : $"Records {builder.ToString().TrimEnd(' ', ',')} are updated.");
        }

        private ValidateParametersData CopyAndFillUnusedFields(FileCabinetRecord validArgs, FileCabinetRecord record)
        {
            var args = DataHelper.CreateValidateData(validArgs);
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
