using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Date of birth json model.
    /// </summary>
    public class DateOfBirthJson
    {
        /// <summary>
        /// Gets or sets from valid value.
        /// </summary>
        /// <value>
        /// From valid value.
        /// </value>
        public DateTime Min { get; set; }

        /// <summary>
        /// Gets or sets to valid value.
        /// </summary>
        /// <value>
        /// To valid value.
        /// </value>
        public DateTime Max { get; set; }
    }
}
