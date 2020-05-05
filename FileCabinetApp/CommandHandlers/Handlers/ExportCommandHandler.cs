using System;
using System.IO;

namespace FileCabinetApp
{
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "export";
        private const char WhiteSpace = ' ';
        private const string CsvString = "csv";
        private const string XmlString = "xml";

        public ExportCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Export(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Export(string parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var inputs = parameter.Split(WhiteSpace, 2);

            var message = inputs.Length == 2 ? this.ExportTo(inputs[0], inputs[1]) : "Export failed: invalid arguments.";

            Console.WriteLine(message);
        }

        private string ExportTo(string format, string path)
        {
            var snapshot = this.Service.MakeSnapshot();

            bool isCanceled = false;
            string message = string.Empty;
            if (File.Exists(path))
            {
                Console.Write($"File is exist - rewrite {path} [Y/n] ");
                isCanceled = !DataHelper.YesOrNo();
            }

            if (isCanceled) message = "Export canceled by the user.";

            FileStream filestream = default;
            try
            {
                filestream = File.Create(path);
            }
            catch (FileNotFoundException)
            {
                message = $"Export failed: can't open file {path}.";
            }

            using var stream = new StreamWriter(filestream);

            if (format.Equals(CsvString, StringComparison.InvariantCultureIgnoreCase))
            {
                using var writer = new FileCabinetRecordCsvWriter(stream);
                snapshot.SaveTo(writer);
                message = $"All records export into CSV file {path}";
            }

            if (format.Equals(XmlString, StringComparison.InvariantCultureIgnoreCase))
            {
                using var writer = new FileCabinetRecordXmlWriter(stream);
                snapshot.SaveTo(writer);
                message = $"All record export into XML file {path}";
            }

            return message;
        }
    }
}
