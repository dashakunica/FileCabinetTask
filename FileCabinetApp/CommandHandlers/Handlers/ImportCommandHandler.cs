using System;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Import command.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "import";

        private const string CsvString = "CSV";
        private const string XmlString = "XML";
        private const char WhiteSpace = ' ';

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (importParameters.Length == 2)
            {
                this.ImportFromFormat(importParameters[0], importParameters[1]);
            }
            else
            {
                QueryParser.ShowErrorMessage(Command);
            }
        }

        private void ImportFromFormat(string format, string path)
        {
            string message = string.Empty;
            if (!File.Exists(path))
            {
                message = $"Import error: file {path} does not exist.";
            }

            FileStream fileStream = default;
            try
            {
                fileStream = new FileStream(path, FileMode.Open);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Export failed: can't open file {path}.");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (fileStream != null)
            {
                using var stream = new StreamReader(fileStream);
                var snapshot = new FileCabinetServiceSnapshot(Array.Empty<FileCabinetRecord>());

                if (format.Equals(CsvString, StringComparison.InvariantCultureIgnoreCase))
                {
                    using var reader = new FileCabinetRecordCsvReader(stream);
                    message = this.LoadFrom(reader, path, snapshot);
                }

                if (format.Equals(XmlString, StringComparison.InvariantCultureIgnoreCase))
                {
                    using var reader = new FileCabinetRecordXmlReader(stream);
                    message = this.LoadFrom(reader, path, snapshot);
                }
            }

            Console.WriteLine(message);
        }

        private string LoadFrom(IFileCabinetReader reader, string path, FileCabinetServiceSnapshot snapshot)
        {
            try
            {
                snapshot.LoadFrom(reader);
                this.Service.Restore(snapshot);
            }
            catch (ArgumentException ex)
            {
                return $"Import failed: {ex.Message}";
            }

            var builder = new StringBuilder();
            foreach (var item in snapshot.Logger)
            {
                builder.AppendLine(item);
            }

            builder.Append($"{snapshot?.Records.Count - snapshot.Logger.Count} records were imported from {path}.");
            return builder.ToString();
        }
    }
}
