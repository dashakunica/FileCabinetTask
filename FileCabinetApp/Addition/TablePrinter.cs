using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    public class TablePrinter<T>
    {
        private const char Plus = '+';
        private const char VerticalBorder = '|';
        private const char HorizontalBorder = '-';
        private const char WhiteSpace = ' ';

        private enum RowType
        {
            Header,
            HorizontalBorderLine,
            Values,
        }

        public StringBuilder StringBuilder { get; } = new StringBuilder();

        private PropertyInfo[] RecordProperties { get; } = typeof(T).GetProperties();

        public void ToTableFormat(IEnumerable<T> records)
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
                    this.ToRow(item, columnLength, RowType.HorizontalBorderLine);
                    this.ToRow(item, columnLength, RowType.Header);
                    this.ToRow(item, columnLength, RowType.HorizontalBorderLine);
                    createHeader = false;
                }

                this.ToRow(item, columnLength, RowType.Values);
                this.ToRow(item, columnLength, RowType.HorizontalBorderLine);
            }
        }

        private void ToRow(T record, Dictionary<string, int> columnLength, RowType type)
        {
            this.StringBuilder.Clear();
            char symbol = (type == RowType.Header || type == RowType.Values) ? WhiteSpace : HorizontalBorder;
            char border = (type == RowType.Header || type == RowType.Values) ? VerticalBorder : Plus;

            foreach (var prop in this.RecordProperties)
            {
                var mappingProp = this.RecordProperties.First(x => x.Name == prop.Name);

                string value = type == RowType.Header ? prop.Name :
                    (type == RowType.Values) ? prop.GetValue(record).ToString() :
                    new string(HorizontalBorder, columnLength[prop.Name]);

                if (prop.PropertyType.IsValueType)
                {
                    this.StringBuilder.Append($"{border}{value.PadRight(columnLength[prop.Name], symbol)}");
                }
                else
                {
                    this.StringBuilder.Append($"{border}{value.PadLeft(columnLength[prop.Name], symbol)}");
                }
            }

            this.StringBuilder.Append($"{border}");
            Console.WriteLine(this.StringBuilder);
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
