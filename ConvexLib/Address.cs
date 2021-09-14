using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace ConvexLib
{
    [Serializable]
    public class Address
    {
        public int address { get; set; }

        public static Address FromString(string addr)
        {
            Address address = new Address
            {
                address = int.Parse(addr.Replace("#", ""))
            };
            return address;
        }

        public static Address FromJson(JsonDocument jsonDocument)
        {
            Address address = new Address
            {
                address = int.Parse(jsonDocument.RootElement.GetProperty("value").ToString())
            };
            return address;
        }

        public override string ToString()
        {
            return String.Join("#", address);
        }
    }
}