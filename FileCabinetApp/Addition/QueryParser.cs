using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    public static class QueryParser
    {
        private const char WhiteSpace = ' ';
        private const char SingleQuote = '\'';
        private const char Equal = '=';
        private const char Comma = ',';
        private const char LeftBracket = '(';
        private const char RightBracket = ')';

        private const string Values = "values";
        private const string Where = "where ";
        private const string Set = "set";
        private const string And = "and";
        private const string Or = "or";

        public static (List<string> attributes, List<string> values) InsertParser(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var arguments = parameters.Split(Values);

            var fields = new List<string>();
            var values = new List<string>();

            if (arguments.Length < 2)
            {
                Console.WriteLine("Invalid Insert query. Please enter insert command in exampel:[insert (id, firstname, lastname, dateofbirth) values ('1', 'John', 'Doe', '5/18/1986')]");
            }
            else if (arguments.Length == 2)
            {
                fields = arguments[0].Split(Comma, RightBracket, LeftBracket).ToList();
                values = arguments[1].Split(Comma, RightBracket, LeftBracket).ToList();

                fields.RemoveAll(x => x.Trim(RightBracket, LeftBracket, WhiteSpace).Length == 0);
                values.RemoveAll(x => x.Trim().Length == 0);

                for (int i = 0; i < values.Count; i++)
                {
                    values[i] = values[i].Trim(SingleQuote, WhiteSpace);
                }
            }

            return (fields, values);
        }

        public static (string, string) DeleteParser(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var arguments = parameters.Split(Where, 2);

            string field = string.Empty;
            string value = string.Empty;

            if (arguments.Length < 2)
            {
                Console.WriteLine("Invalid delete query. Please enter delete command in exampel:[delete where id = '1']");
            }
            else if (arguments.Length == 2)
            {
                var valuesPair = arguments[1].Split(Equal).ToList();
                field = arguments[0].Trim(WhiteSpace);
                value = arguments[1].Trim(WhiteSpace, SingleQuote);
            }

            return (field, value);
        }

        public static (string, string) UpdateParser(string parameters)
        {
            if (parameters.Substring(0, 3).Equals(Set, StringComparison.InvariantCulture))
            {
                parameters = parameters.Remove(0, 3);
            }
            else
            {
                Console.WriteLine("Invalid update query. Please enter delete command in exampel:" +
                    "[update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith']");
            }

            var arguments = parameters.Split(Where, 2);

            if (arguments.Length < 2)
            {
                Console.WriteLine("Invalid update query. Please enter delete command in exampel:" +
                    "[update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith']");
            }
            else
            {
                var fieldsToReplace = arguments[0].Split(Comma);
                var fieldsAndValuesToReplace = fieldsToReplace.Select(x => x.Split(Equal).Select(y => y.Trim(SingleQuote, WhiteSpace)));

                var values = arguments[1].Split(And);

                string[] values;
                string type = null;

                List<int> recordId = null;
                List<string> recordFirstName = null;
                List<string> recordLastName = null;
                List<DateTime> recordDate = null;
                List<char> recordSex = null;
                List<decimal> recordWeight = null;
                List<short> recordHeight = null;

                var valuesPairs = values.Select(x => x.Split(Equal).Select(y => y.Trim(SingleQuote, WhiteSpace)));

                foreach (var pair in valuesPairs)
                {
                    if (!FillFields(ref recordId, ref recordFirstName, ref recordLastName, ref recordDate, ref recordSex, ref recordWeight, ref recordHeight, pair.First(), pair.Last().Trim('\'')))
                    {
                        return;
                    }
                }

                IEnumerable<FileCabinetRecord> mustBeUpdated;
                if (type.Equals("and", StringComparison.InvariantCulture))
                {
                    mustBeUpdated = this.SelectAnd(recordId, recordFirstName, recordLastName, recordDate, recordSex, recordWeight, recordHeight);
                }
                else
                {
                    mustBeUpdated = this.SelectOr(recordId, recordFirstName, recordLastName, recordDate, recordSex, recordWeight, recordHeight);
                }

                if (mustBeUpdated is null)
                {
                    return;
                }
                else
                {
                    this.Service.Update(mustBeUpdated, fieldsAndValuesToReplace);
                }
            }
        }
    }
}
