﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Update command handler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "update";

        private static readonly PropertyInfo[] FileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
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

            var (propNewValuesPair, propWhereValuesPair) = QueryParser.UpdateParser(parameters);

            if (propNewValuesPair != null || propWhereValuesPair != null)
            {
                var set = propNewValuesPair;
                var where = propWhereValuesPair;

                var newValues = DataHelper.CreateRecordFromDict(set);
                var oldRecords = DataHelper.CreateRecordFromDict(where);
                if (newValues != null && oldRecords != null)
                {
                    var updatedRecords = this.Service.FindRecords(oldRecords, QueryParser.TypeCondition);
                    var builder = new StringBuilder();

                    foreach (var item in updatedRecords)
                    {
                        builder.Append($"#{item.Id}, ");
                        var current = this.TrimFields(newValues, item);
                        try
                        {
                            this.Service.EditRecord(item.Id, current);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine(ex.Message + "Invalid parameters.");
                            builder.Clear();
                        }

                        Memoization.RefreshMemoization();
                    }

                    string message = string.Empty;
                    if (builder.Length == 0)
                    {
                        message = "There is no selected records to update matching this condition.";
                    }
                    else if (set.TryGetValue("id", out string value))
                    {
                        Console.WriteLine("Unfortunately, you cannot update id.");
                    }
                    else
                    {
                        message = updatedRecords.Count() == 1
                            ? $"Record {builder.ToString().TrimEnd(' ', ',')} is updated."
                            : $"Records {builder.ToString().TrimEnd(' ', ',')} are updated.";
                    }

                    Console.WriteLine(message);
                }
            }
        }

        private ValidateParametersData TrimFields(FileCabinetRecord validArgs, FileCabinetRecord record)
        {
            var args = DataHelper.CreateValidateData(validArgs);
            var defaultValidateArgs = new ValidateParametersData();
            var current = args.Clone();
            foreach (var item in ValidateParametersProperties)
            {
                if (Equals(item.GetValue(args), item.GetValue(defaultValidateArgs)))
                {
                    var recordProp = FileCabinetRecordProperties.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase));
                    item.SetValue(current, recordProp.GetValue(record));
                }
            }

            return current;
        }
    }
}
