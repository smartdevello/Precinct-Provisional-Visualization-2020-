using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;


namespace Precinct_Provisional_Visualization
{
    public class PrecintData
    {
        [Name("Precinct")]
        public string precinct { get; set; }

        [Name("Name")]
        public string name { get; set; }

        [Name("Reg-V")]
        public int reg_v { get; set; }

        [Name("Actual")]
        public int actual { get; set; }

        [Name("Prov")]
        public int prov { get; set; }

        [Name("Y-A1")]
        public int A1 { get; set; }

        [Name("Y-A2")]
        public int A2 { get; set; }

        [Name("Y-A3")]
        public int A3 { get; set; }

        [Name("Y-A4")]
        public int A4 { get; set; }

        [Name("Y-A5")]
        public int A5 { get; set; }

        [Name("Y-A6")]
        public int A6 { get; set; }

        [Name("Y-A7")]
        public int A7 { get; set; }

        [Name("Y-A8")]
        public int A8 { get; set; }

        [Name("N-B10")]
        public int B10 { get; set; }

        [Name("N-B11")]
        public int B11 { get; set; }

        [Name("N-B12")]
        public int B12 { get; set; }

        [Name("NB-13")]
        public int B13 { get; set; }

        [Name("N-B14")]
        public int B14 { get; set; }

        [Name("N-B17")]
        public int B17 { get; set; }

        [Name("2020")]
        public string r20;

        [Name("2016")]
        public string r16;

        [Name("2012")]
        public string r12;

        [Name("2008")]
        public string r08;

    }
}
