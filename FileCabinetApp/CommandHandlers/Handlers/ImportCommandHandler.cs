using System;
using System.IO;

namespace FileCabinetApp
{
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "import";

        private const string CsvString = "CSV";
        private const string XmlString = "XML";
        private const char WhiteSpace = ' ';

        public ImportCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Import(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Import(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var importParameters = parameters.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            var message = importParameters.Length == 2 ? this.ImportFromFormat(importParameters[0], importParameters[1]) : "Import failed: invalid arguments.";

            Console.WriteLine(message);
        }

        private string ImportFromFormat(string format, string path)
        {
            string message = string.Empty;
            if (!File.Exists(path))
            {
                message = $"Import error: file {path} not exist.";
            }

            using var stream = new StreamReader(File.OpenRead(path));

            if (format.Equals(CsvString, StringComparison.InvariantCultureIgnoreCase))
            {
                var snapshot = new FileCabinetServiceSnapshot(Array.Empty<FileCabinetRecord>());
                try
                {
                    using var reader = new FileCabinetRecordCsvReader(stream);
                    snapshot.LoadFrom(reader);
                }
                catch (Exception ex)
                {
                    message = $"Import failed: {ex.InnerException.Message}";
                }

                this.Service.Restore(snapshot);
                message = $"{snapshot?.Records.Count} were imported from {path}.";
            }

            if (format.Equals(XmlString, StringComparison.InvariantCultureIgnoreCase))
            {
                var snapshot = new FileCabinetServiceSnapshot(Array.Empty<FileCabinetRecord>());
                try
                {
                    using var reader = new FileCabinetRecordXmlReader(stream);
                    snapshot.LoadFrom(reader);
                }
                catch (Exception ex)
                {
                    message = $"Import failed: {ex.InnerException.Message}";
                }

                this.Service.Restore(snapshot);
                message = $"{snapshot?.Records.Count} were imported from {path}.";
            }

            return message;
        }
    }
}
