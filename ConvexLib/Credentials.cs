namespace ConvexLib
{
    public class Credentials
    {
        public Address address { get;}
        public AccountKey accountKey { get; set; }
        public string secretKey { get; set; }

        public Credentials(Address addr, AccountKey ak = null, string sk = null)
        {
            address = addr;
            accountKey = ak;
            secretKey = sk;
        }
    }
}