using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumo_Api_Lacuna_Genetics.Class
{
    public class Job
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Strand { get; set; }
        public string StrandEncoded { get; set; }
        public string GeneEncoded { get; set; }
    }
}
