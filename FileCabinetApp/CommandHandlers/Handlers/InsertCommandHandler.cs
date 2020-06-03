using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Insert command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "insert";

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
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
                this.GetArguments(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Insert(int id, ValidateParametersData data)
        {
            try
            {
                id = id == 0 ? this.Service.CreateAndSetId(data) : this.Service.CreateRecord(id, data);
                Console.WriteLine($"Record #{id} is created.");
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
            catch (FormatException fe)
            {
                Console.WriteLine(fe.Message);
            }
            catch (OverflowException oe)
            {
                Console.WriteLine(oe.Message);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
            }
        }

        private void GetArguments(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();

            var (fields, values) = QueryParser.InsertParser(parameters);
            if (fields != null || values != null)
            {
                var record = new FileCabinetRecord();
                bool isValid = true;

                for (int i = 0; i < fields.Count; i++)
                {
                    var currentPropertie = fields[i];
                    var currentValue = values[i];
                    var prop = fileCabinetRecordProperties.FirstOrDefault(x => x.Name.Equals(currentPropertie, StringComparison.InvariantCultureIgnoreCase));
                    if (prop != null)
                    {
                        var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                        try
                        {
                            prop.SetValue(record, converter.ConvertFromInvariantString(currentValue));
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine($"Cannot convert propertie {prop.Name}. Please correct your input.");
                            isValid = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Cannot convert propertie {prop.Name}. Invalid format.");
                            isValid = false;
                        }
                    }
                }

                if (isValid)
                {
                    var data = DataHelper.CreateValidateData(record);
                    this.Insert(record.Id, data);
                }
            }
        }
    }
}
