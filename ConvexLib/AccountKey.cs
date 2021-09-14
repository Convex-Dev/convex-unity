using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexLib
{
    [Serializable]
    public class AccountKey
    {
        public string accountKey { get; set; }

        public AccountKey(string val)
        {
            accountKey = val;
        }
    }
}