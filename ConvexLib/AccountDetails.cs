using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexLib
{
    [Serializable]
    public class AccountDetails
    {
        public int address { get; set; }
        public bool isLibrary { get; set; }
        public bool isActor { get; set; }
        public int memorySize { get; set; }
        public int allowance { get; set; }
        public string type { get; set; }
        public int balance { get; set; }
        public int sequence { get; set; }

    }
}
