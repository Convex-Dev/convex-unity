using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConvexLib
{
    [Serializable]
    public class FungibleTokenMetadata
    {
        public string name { get; set; }
        public string description { get; set; }

        public static FungibleTokenMetadata FromJson(JsonDocument jsonDocument)
        {
            FungibleTokenMetadata fungibleTokenMetadata = new FungibleTokenMetadata
            {
                name = jsonDocument.RootElement.GetProperty("name").ToString(),
                description = jsonDocument.RootElement.GetProperty("description").ToString()
            };
            return fungibleTokenMetadata;
        }

        public string ToJson(FungibleTokenMetadata tokenMetadata)
        {
            return new JavaScriptSerializer().Serialize(tokenMetadata);
        }

        public override string ToString()
        {
            return ToJson(this);
        }
    }

    [Serializable]
    public class FungibleToken : Asset
    {
        // Shouldn't change after creation
        public Address address { get; set; }
        public FungibleTokenMetadata metadata { get; set; }

        public FungibleToken(Address addr, FungibleTokenMetadata met)
        {
            address = addr;
            metadata = met;
        }

        public FungibleToken FromJson(JsonDocument jsonContent)
        {
            Address addr = Address.FromJson(
                JsonDocument.Parse(jsonContent.RootElement.GetProperty("address").ToString()));

            FungibleTokenMetadata tokenMetadata = FungibleTokenMetadata.FromJson(
                JsonDocument.Parse(jsonContent.RootElement.GetProperty("metadata").ToString()));

            FungibleToken fungibleToken = new FungibleToken(addr, tokenMetadata);
            return fungibleToken;
        }

        public override bool Equals(object obj)
        {
            return obj is FungibleToken token && address == token.address;
        }

        public override int GetHashCode()
        {
            return address.GetHashCode();
        }

        public static string ToJson(FungibleToken fungibleToken)
        {
            return new JavaScriptSerializer().Serialize(fungibleToken);
        }

        public override string ToString()
        {
            return ToJson(this);
        }
    }

    public class NonFungibleTokenMetadata : Asset
    {
        public string name { get; set; }
        public string description { get; set; }

        public static NonFungibleTokenMetadata FromJson(JsonDocument jsonDocument)
        {
            NonFungibleTokenMetadata nonFungibleTokenMetadata = new NonFungibleTokenMetadata
            {
                name = jsonDocument.RootElement.GetProperty("name").ToString(),
                description = jsonDocument.RootElement.GetProperty("description").ToString()
            };
            return nonFungibleTokenMetadata;
        }

        public string ToJson(NonFungibleTokenMetadata tokenMetadata)
        {
            return new JavaScriptSerializer().Serialize(tokenMetadata);
        }

        public override string ToString()
        {
            return ToJson(this);
        }
    }

    public class NonFungibleToken : Asset
    {
        public Address address { get; set; }
        public NonFungibleTokenMetadata metadata { get; }

        public NonFungibleToken(Address addr, NonFungibleTokenMetadata tokenMetadata)
        {
            address = addr;
            metadata = tokenMetadata;
        }

        public NonFungibleToken FromJson(JsonDocument jsonDocument)
        {
            Address addr = Address.FromJson(
                JsonDocument.Parse(jsonDocument.RootElement.GetProperty("address").ToString()));

            NonFungibleTokenMetadata tokenMetadata = NonFungibleTokenMetadata.FromJson(
                JsonDocument.Parse(jsonDocument.RootElement.GetProperty("metadata").ToString()));

            NonFungibleToken token = new NonFungibleToken(addr, tokenMetadata);
            return token;
        }

        public override bool Equals(object obj)
        {
            return obj is NonFungibleToken token && address == token.address;
        }

        public override int GetHashCode()
        {
            return address.GetHashCode();
        }

        public static string ToJson(NonFungibleToken token)
        {
            return new JavaScriptSerializer().Serialize(token);
        }

        public override string ToString()
        {
            return ToJson(this);
        }
    }
}