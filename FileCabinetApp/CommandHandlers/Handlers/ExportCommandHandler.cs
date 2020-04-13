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

            if (format.Equals(CsvString, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    snapshot.SaveToCsv(new StreamWriter(path, false, System.Text.Encoding.Unicode));
                    message = $"All records are exported to file {path}";
                }
                catch (FileNotFoundException)
                {
                    message = $"Export failed: can't open file {path}.";
                }
            }

            if (format.Equals(XmlString, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    snapshot.SaveToXml(new StreamWriter(path, false, System.Text.Encoding.Unicode));
                    message = $"All records are exported to file {path}";
                }
                catch (FileNotFoundException)
                {
                    message = $"Export failed: can't open file {path}.";
                }
            }

            return message;
        }
    }
}
