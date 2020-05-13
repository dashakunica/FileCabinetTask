using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Table printer.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    public class TablePrinter<T>
    {
        private const char Plus = '+';
        private const char VerticalBorder = '|';
        private const char HorizontalBorder = '-';
        private const char WhiteSpace = ' ';

        private enum SymbolType
        {
            Header,
            HorizontalBorderLine,
            Values,
        }

        /// <summary>
        /// Gets printer.
        /// </summary>
        /// <value>
        /// Printer.
        /// </value>
        public StringBuilder TextPrinter { get; } = new StringBuilder();

        private PropertyInfo[] RecordProperties { get; } = typeof(T).GetProperties();

        /// <summary>
        /// Table printer.
        /// </summary>
        /// <param name="records">Records.</param>
        /// <param name="properties">What we should print.</param>
        public void ToTableFormat(IEnumerable<T> records, List<string> properties)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var columnLength = this.MaxLengthOfFields(records);
            var createHeader = true;

            foreach (var item in records)
            {
                if (createHeader)
                {
                    this.ToRow(item, columnLength, SymbolType.HorizontalBorderLine, properties);
                    this.ToRow(item, columnLength, SymbolType.Header, properties);
                    this.ToRow(item, columnLength, SymbolType.HorizontalBorderLine, properties);
                    createHeader = false;
                }

                this.ToRow(item, columnLength, SymbolType.Values, properties);
                this.ToRow(item, columnLength, SymbolType.HorizontalBorderLine, properties);
            }
        }

        private void ToRow(T record, Dictionary<string, int> columnLength, SymbolType type, List<string> propertiesToPrint)
        {
            this.TextPrinter.Clear();
            char symbol = (type == SymbolType.Header || type == SymbolType.Values) ? WhiteSpace : HorizontalBorder;
            char border = (type == SymbolType.Header || type == SymbolType.Values) ? VerticalBorder : Plus;

            foreach (var prop in this.RecordProperties)
            {
                var mappingProp = this.RecordProperties.First(x => x.Name == prop.Name);

                if (propertiesToPrint is null || propertiesToPrint.FindIndex(x => x.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    string value = type == SymbolType.Header ? prop.Name :
                    (type == SymbolType.Values) ? string.Format(CultureInfo.InvariantCulture, GetFormat(prop.PropertyType), prop.GetValue(record)) :
                    new string(HorizontalBorder, columnLength[prop.Name]);

                    if (prop.PropertyType.IsValueType)
                    {
                        this.TextPrinter.Append($"{border}{value.PadRight(columnLength[prop.Name], symbol)}");
                    }
                    else
                    {
                        this.TextPrinter.Append($"{border}{value.PadLeft(columnLength[prop.Name], symbol)}");
                    }
                }
            }

            this.TextPrinter.Append($"{border}");
            Console.WriteLine(this.TextPrinter);

            static string GetFormat(Type type) => type.Equals(typeof(DateTime)) ? "{0:yyyy-MMM-dd}" : "{0:0}";
        }

        private Dictionary<string, int> MaxLengthOfFields(IEnumerable<T> records)
        {
            var propertyLengthPairs = new Dictionary<string, int>();

            if (!records.Any())
            {
                return propertyLengthPairs;
            }

            foreach (var propertie in this.RecordProperties)
            {
                int max = records.Max(x =>
                {
                    object value = propertie.GetValue(x);
                    return value is null ? 0 : value.ToString().Length + 1;
                });

                propertyLengthPairs.Add(propertie.Name, Math.Max(max, propertie.Name.Length));
            }

            return propertyLengthPairs;
        }
    }
}
