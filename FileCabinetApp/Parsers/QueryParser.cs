using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Query parser.
    /// </summary>
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

        private static readonly PropertyInfo[] ValidateParametersProperties = typeof(ValidateParametersData).GetProperties();

        /// <summary>
        /// Insert command parser.
        /// </summary>
        /// <param name="parameters">Input message.</param>
        /// <returns>Parsed parameters.</returns>
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

        /// <summary>
        /// Delete command parser.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Parsed parameter.</returns>
        public static Dictionary<string, string> DeleteParser(string parameters)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var arguments = parameters.Split(Where, 2);

            if (arguments.Length < 2)
            {
                Console.WriteLine("Invalid delete query. Please enter delete command in exampel:[delete where id = '1']");
            }
            else if (arguments.Length == 2)
            {
                result = AndOrParser(arguments[0]);
            }

            return result;
        }

        /// <summary>
        /// Update command parser.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Parsed parameter.</returns>
        public static (Dictionary<string, string> propNewValuesPair, Dictionary<string, string> propWhereValuesPair) UpdateParser(string parameters)
        {
            Dictionary<string, string> set = new Dictionary<string, string>();
            Dictionary<string, string> where = new Dictionary<string, string>();

            string errorText = "Invalid update query. Please enter delete command in exampel:" +
                    "[update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith']";

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Length < 3)
            {
                Console.WriteLine(errorText);
            }

            if (parameters.Substring(0, 3).Equals(Set, StringComparison.InvariantCulture))
            {
                parameters = parameters.Remove(0, 3);
            }
            else
            {
                Console.WriteLine(errorText);
            }

            var arguments = parameters.Split(Where, 2);
            if (arguments.Length < 2)
            {
                Console.WriteLine(errorText);
            }
            else
            {
                var fieldsToReplace = arguments[0].Split(Comma);
                set = GetDictionary(fieldsToReplace);

                where = AndOrParser(arguments[1]);
            }

            return (set, where);
        }

        /// <summary>
        /// Select command parser.
        /// </summary>
        /// <param name="parameters">Input parameters.</param>
        /// <returns>Parsed parameters.</returns>
        public static (List<string> properties, Dictionary<string, string> propWhereValuesPair) SelectParser(string parameters)
        {
            List<string> properties = new List<string>();
            Dictionary<string, string> where = new Dictionary<string, string>();

            string errorText = "select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'";

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var arguments = parameters.Split(Where, 2);
            if (arguments.Length < 2)
            {
                Console.WriteLine(errorText);
            }
            else
            {
                properties = arguments[0].Split(Comma).ToList();
                where = AndOrParser(arguments[1]);
            }

            return (properties, where);
        }

        /// <summary>
        /// Get records by predicates.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="allRecords">All records.</param>
        /// <param name="type">Type of condition (and/or).</param>
        /// <returns>Records.</returns>
        public static IEnumerable<FileCabinetRecord> GetRecorgs(ValidateParametersData record, IEnumerable<FileCabinetRecord> allRecords, string type)
        {
            IEnumerable<FileCabinetRecord> result = allRecords;

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (allRecords is null)
            {
                throw new ArgumentNullException(nameof(allRecords));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string query = record.ToString() + type;
            var findResult = Memoization.Saved.Find(x => x.Item1.Equals(query, StringComparison.InvariantCultureIgnoreCase));
            if (findResult != null)
            {
                return findResult.Item2;
            }

            if (type.Equals(And, StringComparison.InvariantCultureIgnoreCase))
            {
                result = SelectAnd(record, allRecords);
                Memoization.Saved.Add(new Tuple<string, IEnumerable<FileCabinetRecord>>(query, result));
                return result;
            }
            else
            {
                result = SelectOr(record, allRecords);
                Memoization.Saved.Add(new Tuple<string, IEnumerable<FileCabinetRecord>>(query, result));
                return result;
            }
        }

        private static Dictionary<string, string> GetDictionary(string[] fieldsToReplace)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var fieldsAndValuesToReplace = fieldsToReplace.Select(x => x.Split(Equal).Select(y => y.Trim(SingleQuote, WhiteSpace)));
            foreach (var pair in fieldsAndValuesToReplace)
            {
                var key = pair.First();
                var value = pair.Last();
                result.Add(key, value);
            }

            return result;
        }

        private static Dictionary<string, string> AndOrParser(string arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var valuesAnd = arguments.Split(And);
            var valuesOr = arguments.Split(Or);
            string[] values;
            string type = null;

            if (valuesAnd.Length < valuesOr.Length)
            {
                values = valuesOr;
                type = Or;
            }
            else
            {
                values = valuesAnd;
                type = And;
            }

            var valuesPairs = GetDictionary(values);
            valuesPairs.Add("type", type);
            return valuesPairs;
        }

        private static IEnumerable<FileCabinetRecord> SelectAnd(ValidateParametersData record, IEnumerable<FileCabinetRecord> allRecords)
        {
            var result = allRecords.ToList();

            foreach (var prop in ValidateParametersProperties)
            {
                if (prop.GetValue(record) != null)
                {
                    result.RemoveAll(x => prop.GetValue(record).Equals(x.FirstName));
                }
            }

            return result;
        }

        private static IEnumerable<FileCabinetRecord> SelectOr(ValidateParametersData record, IEnumerable<FileCabinetRecord> allRecords)
        {
            var result = new List<FileCabinetRecord>();

            foreach (var prop in ValidateParametersProperties)
            {
                if (prop.GetValue(record) != null)
                {
                    result.AddRange(allRecords.Where(x => prop.GetValue(record).Equals(x.FirstName)).Where(y => !result.Contains(y)));
                }
            }

            return result;
        }
    }
}
