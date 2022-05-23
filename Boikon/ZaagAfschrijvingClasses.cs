using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Boikon
{
    class CSV
    {
        [Name("Product")]
        public string Product { get; set; }
        [Name("ProductCode")]
        public string ProductCode { get; set; }
        
        public CSV() { }
    }
}
