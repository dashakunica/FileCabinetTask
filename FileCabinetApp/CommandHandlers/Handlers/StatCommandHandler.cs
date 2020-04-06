using System;

namespace FileCabinetApp
{
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "stat";

        public StatCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Stat(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Stat(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var (active, removed) = this.Service.GetStat();
            Console.WriteLine($"Contains {active} records.");

            if (this.Service is FileCabinetFilesystemService)
            {
                Console.WriteLine($"Count removed records {removed} .");
            }
        }
    }
}
