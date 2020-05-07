using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Composite validator.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">All validators properties.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = new List<IRecordValidator>(validators ?? throw new ArgumentNullException(nameof(validators)));
        }

        /// <inheritdoc/>
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
    }
}
