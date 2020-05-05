using System;

namespace FileCabinetApp
{
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "purge";

        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Purge(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Purge(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (this.Service is FileCabinetFilesystemService)
            {
                var (active, removed) = this.Service.GetStat();
                this.Service.Purge();
                Console.WriteLine($"Data file processing is completed: {removed} of {removed + active} records were purged.");
            }
        }
    }
}
