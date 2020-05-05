using System;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class for command handler.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler commandHandler;

        /// <summary>
        /// Handle.
        /// </summary>
        /// <param name="commandRequest">Command handler.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                return;
            }

            if (this.commandHandler != null)
            {
                this.commandHandler.Handle(commandRequest);
            }
            else
            {
                this.PrintMissedInfo(commandRequest.Command);
            }
        }

        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            return this.commandHandler;
        }

        private void PrintMissedInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");

            StringBuilder similarCommand = new StringBuilder();

            foreach (var item in HelpCommandHandler.Commands)
            {
                if (DataHelper.GetSimilarity(command, item) > 0.5)
                {
                    similarCommand.Append($"{Environment.NewLine}{item}");
                }
            }

            if (!string.IsNullOrEmpty(similarCommand.ToString()))
            {
                Console.WriteLine($"The most similar commands are" + similarCommand);
            }
        }
    }
}
