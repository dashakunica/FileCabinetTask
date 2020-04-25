using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "update";

        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Update(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Update(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var items = QueryParser.UpdateParser(parameters);

            var set = items.propNewValuesPair;
            var where = items.propWhereValuesPair;


        }
    }
}
