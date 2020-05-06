using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Full name serialization model.
    /// </summary>
    [Serializable]
    public class Name
    {
        /// <summary>
        /// Gets or sets first name.
        /// </summary>
        /// <value>
        /// First name.
        /// </value>
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name.
        /// </summary>
        /// <value>
        /// First name.
        /// </value>
        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
