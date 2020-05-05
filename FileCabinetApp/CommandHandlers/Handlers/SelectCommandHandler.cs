using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace FileCabinetApp
{
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "select";

        private const string Id = "Id";

        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        private static readonly PropertyInfo[] FileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
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

            var (properties, where) = QueryParser.SelectParser(parameters);

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

            var oldRecords = CreateValidateArgs(where);
            var allRecords = this.Service.GetRecords();

            var selectedRecords = QueryParser.GetRecorgs(oldRecords, allRecords, type);
            this.printer(selectedRecords);
            Console.WriteLine("Completed successfully.");
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
    }
}