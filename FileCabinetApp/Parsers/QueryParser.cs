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

        private const string Delete = "delete";
        private const string Export = "export";
        private const string Import = "import";
        private const string Update = "update";
        private const string Select = "select";
        private const string Insert = "insert";

        private static readonly PropertyInfo[] FileCabinetProperties = typeof(FileCabinetRecord).GetProperties();

        /// <summary>
        /// Gets type of condition (And/Or).
        /// </summary>
        /// <value>
        /// Type of condition (And/Or).
        /// </value>
        public static string TypeCondition { get; private set; } = And;

        /// <summary>
        /// Insert command parser.
        /// </summary>
        /// <param name="parameters">Input message.</param>
        /// <returns>Parsed parameters.</returns>
        public static (List<string> attributes, List<string> values) InsertParser(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                ShowErrorMessage(Insert);
                return (null, null);
            }

            var arguments = parameters.Split(Values);

            var fields = new List<string>();
            var values = new List<string>();

            if (arguments.Length < 2)
            {
                ShowErrorMessage(Insert);
                return (null, null);
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

                for (int i = 0; i < fields.Count; i++)
                {
                    fields[i] = fields[i].Trim(WhiteSpace);
                }
            }

            if (fields.Count != FileCabinetProperties.Length || values.Count != fields.Count)
            {
                Console.WriteLine("Query should contains all properties and values for them.");
                ShowErrorMessage(Insert);
                return (null, null);
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
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(parameters))
            {
                ShowErrorMessage(Delete);
                return null;
            }

            var arguments = parameters.Split(Where, 2);

            if (arguments.Length < 2)
            {
                ShowErrorMessage(Delete);
                return null;
            }
            else if (arguments.Length == 2)
            {
                result = AndOrParser(arguments[1]);
            }

            if (result.Keys.Count != result.Values.Count)
            {
                ShowErrorMessage(Delete);
                return null;
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
            Dictionary<string, string> set = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> where = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(parameters))
            {
                ShowErrorMessage(Update);
                return (null, null);
            }

            if (parameters.Length < 3)
            {
                ShowErrorMessage(Update);
                return (null, null);
            }

            if (parameters.Substring(0, 3).Equals(Set, StringComparison.InvariantCulture))
            {
                parameters = parameters.Remove(0, 3);
            }
            else
            {
                ShowErrorMessage(Update);
                return (null, null);
            }

            var arguments = parameters.Split(Where, 2);
            if (arguments.Length < 2)
            {
                ShowErrorMessage(Update);
                return (null, null);
            }
            else
            {
                var fieldsToReplace = arguments[0].Split(Comma);
                set = GetDictionary(fieldsToReplace);

                where = AndOrParser(arguments[1]);
            }

            if (set.Values.Count != set.Keys.Count ||
                where.Values.Count != where.Keys.Count)
            {
                ShowErrorMessage(Update);
                return (null, null);
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
            Dictionary<string, string> where = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(parameters))
            {
                ShowErrorMessage(Select);
                return (null, null);
            }

            var arguments = parameters.Split(Where, 2);
            properties = arguments[0].Split(Comma).ToList();
            if (arguments.Length == 2)
            {
                where = AndOrParser(arguments[1]);
            }

            if (where.Keys.Count != where.Values.Count)
            {
                ShowErrorMessage(Select);
                return (null, null);
            }

            for (int i = 0; i < properties.Count; i++)
            {
                properties[i] = properties[i].Trim(WhiteSpace);
            }

            return (properties, where);
        }

        /// <summary>
        /// Shows error message.
        /// </summary>
        /// <param name="query">Query string.</param>
        public static void ShowErrorMessage(string query)
        {
            Console.WriteLine($"Invalid {query} query.");

            string example = string.Empty;
            switch (query)
            {
                case Delete:
                    example = "[delete where id = '1']";
                    break;
                case Export:
                    example = "[export csv e:\\filename.csv]";
                    break;
                case Import:
                    example = "[import csv d:\\data\\records.csv]";
                    break;
                case Select:
                    example = "[select id, firstname, lastname where firstname = 'John' and lastname = 'Doe']";
                    break;
                case Update:
                    example = "[update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1']";
                    break;
                case Insert:
                    example = "[insert (id, firstname, lastname, dateofbirth, salary, accounttype, bonuses) values ('1', 'John', 'Doe', '5/18/1986', '3500', 'd', '12')]";
                    break;
            }

            Console.WriteLine($"Please enter {query} command in example: {Environment.NewLine}{example}");
        }

        private static Dictionary<string, string> GetDictionary(string[] fieldsToReplace)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var fieldsAndValuesToReplace = fieldsToReplace.Select(x => x.Split(Equal).Select(y => y.Trim(SingleQuote, WhiteSpace)));
            foreach (var pair in fieldsAndValuesToReplace)
            {
                var key = pair.First();
                var value = pair.Last();
                try
                {
                    result.Add(key, value);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("The search condition can be set only for different properties.");
                }
            }

            return result;
        }

        private static Dictionary<string, string> AndOrParser(string arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var valuesAnd = arguments.Split(And, StringSplitOptions.RemoveEmptyEntries);
            var valuesOr = arguments.Split(Or, StringSplitOptions.RemoveEmptyEntries);
            string[] values;

            if (valuesAnd.Length < valuesOr.Length)
            {
                values = valuesOr;
                TypeCondition = Or;
            }
            else
            {
                values = valuesAnd;
                TypeCondition = And;
            }

            var valuesPairs = GetDictionary(values);
            return valuesPairs;
        }
    }
}
