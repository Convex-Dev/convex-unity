using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexLib
{
    [Serializable]
    public class QueryRequest
    {
        public int address { get; set; }
        public string source { get; set; }
    }
}
