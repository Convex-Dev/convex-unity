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

        public Account ToAccount()
        {
            Account account = new Account();
            account.Address = address;
            account.Balance = balance;
            account.Sequence = sequence;
            account.Type = ConvertType(type);
            account.MemoryAllowance = allowance;
            account.MemorySize = memorySize;
            return account;
        }

        public AccountType ConvertType(string accType)
        {
            if (Enum.TryParse<AccountType>(accType, out var result))
            {
                switch (result)
                {
                    case AccountType.Actor:
                        return AccountType.Actor;
                    case AccountType.Library:
                        return AccountType.Library;
                    case AccountType.User:
                        return AccountType.User;
                    default:
                        throw new Exception("Unknown account type");
                }
            }

            throw new Exception("Unknown account type");
        }
    }
}