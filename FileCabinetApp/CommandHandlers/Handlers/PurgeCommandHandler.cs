using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Purge command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "purge";

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// Purge command handler.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
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

            if (Startup.IsFileService)
            {
                var (active, removed) = this.Service.GetStat();
                this.Service.Purge();
                Console.WriteLine($"Defragmentation of records is completed: {removed} of {removed + active} records were purged.");
            }
            else
            {
                Console.WriteLine("This command work only in file cabinet Filesystem Service.");
            }
        }
    }
}
