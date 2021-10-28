using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precinct_Provisional_Visualization
{
    public class CodeDescription
    {
        [Name("PRECINCT")]
        public string precinct { get; set; }

        [Name("Reason Code")]
        public string reason_code { get; set; }

        [Name("Description")]
        public string description { get; set; }

        [Name("Count")]
        public int count { get; set; }
    }
}
