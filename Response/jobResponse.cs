using Consumo_Api_Lacuna_Genetics;
using Consumo_Api_Lacuna_Genetics.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumo_Api_Lacuna_Genetics.Response
{
    public class JobResponse
    {
        public Job Job { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}