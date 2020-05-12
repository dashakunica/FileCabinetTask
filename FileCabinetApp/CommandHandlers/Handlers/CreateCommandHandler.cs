using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Create command handler.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "create";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// Create command handler.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Hadle.
        /// </summary>
        /// <param name="commandRequest">Command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (Command.Equals(commandRequest?.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Create(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Create(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            int id = int.MinValue;
            bool isValid = false;
            do
            {
                try
                {
                    var record = DataHelper.RequestData();
                    id = this.Service.CreateAndSetId(record);

                    isValid = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            while (!isValid);

            Console.WriteLine($"Record with id #{id} is created.");
        }
    }
}
