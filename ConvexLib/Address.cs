using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace ConvexLib
{
    public class Address
    {
        public int value { get; set; }

        public static Address FromString(string addr)
        {
            Address address = new Address
            {
                value = int.Parse(addr.Replace("#", ""))
            };
            return address;
        }

        public static Address FromJson(JsonDocument jsonDocument)
        {
            Address address = new Address
            {
                value = int.Parse(jsonDocument.RootElement.GetProperty("value").ToString())
            };
            return address;
        }

        public override string ToString()
        {
            return String.Join("#", value);
        }
    }
}