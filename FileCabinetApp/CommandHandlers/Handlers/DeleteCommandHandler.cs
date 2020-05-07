using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Delete command handle.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "delete";
        private const string Id = "Id";
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

            var where = QueryParser.DeleteParser(parameters);

            string id;
            if (where.TryGetValue(Id, out id))
            {
                int temp;
                if (!int.TryParse(id, out temp))
                {
                    Console.WriteLine("Invalid Id value.");
                }

                this.Service.RemoveRecord(temp);

                Console.WriteLine($"Records with {temp} is deleted.");
            }
            else
            {
                var whereRecord = DataHelper.CreateRecordFromDict(where);
                var allRecords = this.Service.GetRecords();

                var records = QueryParser.GetRecorgs(whereRecord, allRecords, QueryParser.TypeCondition);

                var builder = new StringBuilder();
                foreach (var item in records)
                {
                    builder.Append($"#{item.Id}, ");
                }

                string text = builder.Length == 0 ? $"No deleted records." : $"Records {builder.ToString().TrimEnd(WhiteSpace, Comma)} are deleted.";
                this.Service.Delete(records);
                Memoization.RefreshMemoization();
                Console.WriteLine(text);
            }
        }
    }
}
