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

    public class TransactionRequest
    {
        public int address { get; set; }
        public string accountKey { get; set; }
        public string hash { get; set; }
        public string sig { get; set; }
    }
}
