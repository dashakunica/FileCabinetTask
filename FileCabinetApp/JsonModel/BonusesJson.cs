using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Bonuses json model.
    /// </summary>
    public class BonusesJson
    {
        /// <summary>
        /// Gets or sets minimum vald value.
        /// </summary>
        /// <value>
        /// Minimum vald value.
        /// </value>
        public short Min { get; set; }

        /// <summary>
        /// Gets or sets maximum valid value.
        /// </summary>
        /// <value>
        /// Maximum valid value.
        /// </value>
        public short Max { get; set; }
    }
}
