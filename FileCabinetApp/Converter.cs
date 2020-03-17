using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public static class Converter
    {
        public static Tuple<bool, string, string> StringConverter(string source)
        {
            return new Tuple<bool, string, string>(true, source, source);
        }

        public static Tuple<bool, string, DateTime> DateOfBirthConverter(string source)
        {
            DateTime result;
            if (DateTime.TryParse(source, out result))
            {
                return new Tuple<bool, string, DateTime>(true, source, result);
            }

            return new Tuple<bool, string, DateTime>(false, source, result);
        }

        public static Tuple<bool, string, char> SexConverter(string source)
        {
            char result;
            if (char.TryParse(source, out result))
            {
                return new Tuple<bool, string, char>(true, source, result);
            }

            return new Tuple<bool, string, char>(false, source, result);
        }

        public static Tuple<bool, string, decimal> SalaryConverter(string source)
        {
            decimal result;
            if (decimal.TryParse(source, out result))
            {
                return new Tuple<bool, string, decimal>(true, source, result);
            }

            return new Tuple<bool, string, decimal>(false, source, result);
        }

        public static Tuple<bool, string, short> Bonuses(string source)
        {
            short result;
            if (short.TryParse(source, out result))
            {
                return new Tuple<bool, string, short>(true, source, result);
            }

            return new Tuple<bool, string, short>(false, source, result);
        }
    }
}
