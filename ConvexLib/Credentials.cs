using System;

namespace ConvexLib
{
    [Serializable]
    public class Credentials
    {
        public Address address { get; set; }
        public AccountKey accountKey { get; set; }
        public KeyPair keyPair { get; set; }

        public Credentials(Address addr, AccountKey ak = null, KeyPair kp = null)
        {
            address = addr;
            accountKey = ak;
            keyPair = kp;
        }
    }
}