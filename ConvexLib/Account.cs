using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvexLib
{
    [Serializable]
    public class Account
    {
        public int Address;
        public int Sequence;
        public AccountType Type;
        public int Balance;
        public int MemorySize;
        public int MemoryAllowance;
        public string Error;
    }

    public enum AccountType
    {
        Actor,
        User,
        Library
    }
}