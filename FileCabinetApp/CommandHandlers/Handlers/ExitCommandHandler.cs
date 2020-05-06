using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Exit command handle.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private const string Command = "exit";
        private Action<bool> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="action">Action.</param>
        public ExitCommandHandler(Action<bool> action)
        {
            this.action = action;
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
                this.Exit(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Exit(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Console.WriteLine("Exiting of application.");
            this.action(false);
        }
    }
}
