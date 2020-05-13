using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Delete command handle.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "delete";
        private const char WhiteSpace = ' ';
        private const char Comma = ',';

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// Delete command handler.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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
                this.Delete(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Delete(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Dictionary<string, string> where = QueryParser.DeleteParser(parameters);

            if (where != null)
            {
                FileCabinetRecord whereRecord = DataHelper.CreateRecordFromDict(where);
                var allRecords = this.Service.GetRecords();

                var records = QueryParser.GetRecorgs(whereRecord, allRecords, QueryParser.TypeCondition);

                var builder = new StringBuilder();
                int countMatchedRecords = 0;
                foreach (var item in records)
                {
                    builder.Append($"#{item.Id}, ");
                    countMatchedRecords++;
                }

                string text = countMatchedRecords == 0
                    ? $"No deleted records."
                    : $"{(countMatchedRecords == 1 ? "Record" : "Records")} " +
                    $"{builder.ToString().TrimEnd(WhiteSpace, Comma)} " +
                    $"{(countMatchedRecords == 1 ? "is" : "are")} deleted.";

                this.Service.Delete(records);
                Memoization.RefreshMemoization();
                Console.WriteLine(text);
            }
        }
    }
}
