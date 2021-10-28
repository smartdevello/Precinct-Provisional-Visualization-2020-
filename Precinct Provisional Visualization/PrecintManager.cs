using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Precinct_Provisional_Visualization
{

    public class PrecintManager
    {
        private string inputfile;
        private string exception_msg;

        public PrecintManager(string inputfile)
        {
            this.inputfile = inputfile;
        }
        public void setInputfile(string inputfile)
        {
            this.inputfile = inputfile;
        }
        public string getLastException()
        {
            return this.exception_msg;
        }
        public List<PrecintData> readData()
        {
            List<PrecintData> data = new List<PrecintData>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                //PrepareHeaderForMatch = header_args => header_args.Header.ToLower(),
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };
            try
            {
                using (var reader = new StreamReader(this.inputfile))
                using (var csv = new CsvReader(reader, config))
                {
                    data = csv.GetRecords<PrecintData>().ToList();
                }

            }
            catch (Exception e)
            {
                exception_msg = e.GetType().FullName;
                exception_msg = e.Message;
                return null;
            }
            return data;
            
        }
    }
}
