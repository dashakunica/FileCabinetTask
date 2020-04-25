﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

        private const string Id = "id";
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string DateOfBirth = "dateofbirth";
        private const string Salary = "salary";
        private const string Bonuses = "bonuses";
        private const string AccountType = "accounttype";

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

                var records = AndOrParser(arguments[1]);
                where = records.Item1;
            }

            return (set, where);
        }

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
                var fieldsToReplace = arguments[0].Split(Comma);
                //set = GetDictionary(fieldsToReplace);

                var records = AndOrParser(arguments[1]);
            }

            return (properties, where);
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

        private static (Dictionary<string, string>, string) AndOrParser(string arguments)
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
            return (valuesPairs, type);
        }
    }
}