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

        public static (List<string> attributes, List<string> values) InsertParser(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var arguments = parameters.Split("values");

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
                    values[i] = values[i].Trim('\'', ' ');
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

            var arguments = parameters.Split("where ", 2);

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
    }
}
