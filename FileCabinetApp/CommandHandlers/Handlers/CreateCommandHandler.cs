using System;
using System.Collections.Generic;
using System.Text;

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
                    id = this.Service.CreateRecord((
                        record.FirstName,
                        record.LastName,
                        record.DateOfBirth,
                        record.Bonuses,
                        record.Salary,
                        record.AccountType));

                    isValid = true;
                }
                catch (InvalidOperationException ioe)
                {
                    Console.WriteLine(ioe.Message);
                }
                catch (FormatException fe)
                {
                    Console.WriteLine(fe.Message);
                }
                catch (OverflowException oe)
                {
                    Console.WriteLine(oe.Message);
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine(ane.Message);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }
            }
            while (!isValid);

            Console.WriteLine($"Record #{id} is created.");
        }
    }
}
