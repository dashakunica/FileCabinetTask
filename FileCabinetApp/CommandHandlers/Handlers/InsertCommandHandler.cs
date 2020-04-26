using System;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "insert";

        public InsertCommandHandler(IFileCabinetService fileCabinetService)
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
                this.GetArguments(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Insert(int id, ValidateParametersData data)
        {
            id = id == 0 ? this.Service.CreateRecord(data) : this.Service.CreateRecordWithId(id, data);
            Console.WriteLine($"Record #{id} is created.");
        }

        private void GetArguments(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();

            var (fields, values) = QueryParser.InsertParser(parameters);
            var record = new FileCabinetRecord();

            for (int i = 0; i < fields.Count; i++)
            {
                var currentAttribute = fields[i];
                var currentValue = values[i];
                var prop = fileCabinetRecordProperties.FirstOrDefault(x => x.Name.Equals(currentAttribute, StringComparison.InvariantCultureIgnoreCase));
                if (prop != null)
                {
                    var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                    prop.SetValue(record, converter.ConvertFromInvariantString(currentValue));
                }
            }

            var data = DataHelper.CreateValidateData(record);
            this.Insert(record.Id, data);
        }
    }
}
