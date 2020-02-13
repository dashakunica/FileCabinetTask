using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Immutable class for parameters.
    /// </summary>
    public class IntroduceParameter
    {
        /// <summary>
        /// Gets Id of record.
        /// </summary>
        /// <value>
        /// Id.
        /// </value>
        public int Id { get; }

        /// <summary>
        /// Gets FirstName of record.
        /// </summary>
        /// <value>
        /// FirstName.
        /// </value>
        public string FirstName { get; }

        /// <summary>
        /// Gets LastName of record.
        /// </summary>
        /// <value>
        /// LastName.
        /// </value>
        public string LastName { get; }

        /// <summary>
        /// Gets DateOfBirth of record.
        /// </summary>
        /// <value>
        /// DateOfBirth.
        /// </value>
        public DateTime DateOfBirth { get; }
    }
}
