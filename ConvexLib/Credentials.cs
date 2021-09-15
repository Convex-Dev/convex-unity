namespace ConvexLib
{
    public class Credentials
    {
        public Address address { get; }
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