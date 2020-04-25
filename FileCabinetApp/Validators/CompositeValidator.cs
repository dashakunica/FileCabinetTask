using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        public CompositeValidator()
            : this(Enumerable.Empty<IRecordValidator>())
        {
        }

        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = new List<IRecordValidator>(validators ?? throw new ArgumentNullException(nameof(validators)));
        }

        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(data);
            }
        }

        public void Add(IRecordValidator validator)
        {
            this.validators.Add(validator ?? throw new ArgumentNullException(nameof(validator)));
        }

        public bool Remove(IRecordValidator validator)
        {
            return this.validators.Remove(validator ?? throw new ArgumentNullException(nameof(validator)));
        }
    }
}
