using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexLib
{
    [Serializable]
    public class QueryResponse
    {
        public string value { get; set; }
        public string source { get; set; }
        public string errorCode { get; set; }
    }

    [Serializable]
    public class TransactResponse
    {
        public int address { get; set; }
        public string hash { get; set; }
        public int sequence { get; set; }
        public int source { get; set; }
    }

    public class Result
    {
        public dynamic value { get; set; }
        public dynamic errorCode { get; set; }
    }

    public class RedundantValue
    {
        public dynamic value { get; set; }
    }
}