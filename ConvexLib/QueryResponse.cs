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
    public class Value
    {
        public string value { get; set; }
    }

    public class IntValue
    {
        public int value { get; set; }
    }

}
