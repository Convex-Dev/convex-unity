using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexLib
{
    [Serializable]
    public class Faucet
    {
        public int address { get; set; }
        public int amount { get; set; }
        public int value { get; set; }
    }
}
