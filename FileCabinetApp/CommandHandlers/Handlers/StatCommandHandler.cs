using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Statistic command handler.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "stat";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
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
            Console.WriteLine($"Storage contains {active} records. {Environment.NewLine}Count removed records are {removed}.");
        }
    }
}
