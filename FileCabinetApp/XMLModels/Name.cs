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
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
