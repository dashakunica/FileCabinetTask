using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileCabinetApp
{
    public static class ServiceHelper
    {
        public static void AddRecordToDictionary<T>(T parameter, FileCabinetRecord record, Dictionary<T, List<FileCabinetRecord>> dictionary)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (!dictionary.ContainsKey(parameter))
            {
                dictionary.Add(parameter, new List<FileCabinetRecord>());
            }

            dictionary[parameter].Add(record);
        }

        public static void UpdateRecordInDictionary<T>(T resent, T current, FileCabinetRecord record, Dictionary<T, List<FileCabinetRecord>> dictionary)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            var item = dictionary[resent].First(x => x.Id == record.Id);
            dictionary[resent].Remove(item);
            AddRecordToDictionary(current, record, dictionary);
        }

        public static void DeleteRecordFromDictionary<T>(T parameter, FileCabinetRecord record, Dictionary<T, List<FileCabinetRecord>> dictionary)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.ContainsKey(parameter))
            {
                dictionary[parameter].Remove(record);
            }
        }

        public static ReadOnlyCollection<FileCabinetRecord> FindInDictionary<T>(T parameter, Predicate<FileCabinetRecord> predicate, ReadOnlyCollection<FileCabinetRecord> collection, Dictionary<T, List<FileCabinetRecord>> dictionary)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.ContainsKey(parameter))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(dictionary[parameter]);
            }

            var records = collection.Where(x => predicate(x)).ToArray();

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }
    }
}
