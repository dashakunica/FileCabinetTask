using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "select";
        private const string Id = "Id";

        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        /// <param name="printer">Printer.</param>
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

            string id;
            if (where.TryGetValue(Id, out id))
            {
                int temp;
                if (!int.TryParse(id, out temp))
                {
                    Console.WriteLine("Invalid Id value.");
                }

                this.Service.RemoveRecord(temp);
            }

            string type = where["type"];

            var oldRecords = this.CreateValidateArgs(where);
            var allRecords = this.Service.GetRecords();

            var selectedRecords = QueryParser.GetRecorgs(oldRecords, allRecords, type);
            this.printer(selectedRecords);
            Console.WriteLine("Completed successfully.");
        }

        private ValidateParametersData CreateValidateArgs(Dictionary<string, string> propNewValues)
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