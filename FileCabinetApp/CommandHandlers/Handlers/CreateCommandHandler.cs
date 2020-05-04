using System;

namespace FileCabinetApp
{
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "create";

        public CreateCommandHandler(IFileCabinetService fileCabinetService)
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
                    var record = DataHelper.GetData();
                    id = this.Service.CreateAndSetId(record);

                    isValid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            while (!isValid);

            Console.WriteLine($"Record #{id} is created.");
        }
    }
}
