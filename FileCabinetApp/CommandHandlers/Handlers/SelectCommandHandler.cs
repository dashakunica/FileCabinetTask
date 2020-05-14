using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "select";

        private readonly Action<IEnumerable<FileCabinetRecord>, List<string>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        /// <param name="printer">Printer.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>, List<string>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
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
                this.Select(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Select(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var (properties, where) = QueryParser.SelectParser(parameters);

            if (properties != null && where != null)
            {
                var oldRecords = DataHelper.CreateRecordFromDict(where);
                var allRecords = this.Service.GetRecords();

                if (allRecords == null || !allRecords.Any())
                {
                    Console.WriteLine("There are no records in storage, right now.");
                }

                if (where is null || !where.Any())
                {
                    this.printer(allRecords, properties);
                }
                else
                {
                    var selectedRecords = QueryParser.GetRecorgs(oldRecords, allRecords, QueryParser.TypeCondition);

                    if (selectedRecords == null || !selectedRecords.Any())
                    {
                        Console.WriteLine("There are no selected records matching this condition.");
                    }
                    else
                    {
                        this.printer(selectedRecords, properties);
                    }
                }
            }
        }
    }
}