using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConvexLib
{
    // Main class for interacting with the convex test network
    public class Convex
    {
        private static readonly HttpClient Client = new HttpClient();
        public Credentials Creds;
        
        public void SetCredentials(Credentials cred)
        {
            Creds = cred;
        }

        // Prepare transaction
        // Makes an API call to https://convex.world/tools/rest-api/prepare-transaction
        public async Task<string> PrepareTransaction(string source, Address address = null, int? sequence = null)
        {
            if (address == null && Creds == null) throw new Exception("Missing credentials");

            QueryRequest query = new QueryRequest
            {
                address = address?.address ?? Creds.address.address,
                source = source
            };

            HttpResponseMessage response =
                await Client.PostAsJsonAsync<QueryRequest>("https://convex.world/api/v1/transaction/prepare", query);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            if (responseBody != null)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                return jsonDocument.RootElement.GetProperty("hash").ToString();
            }

            return null;
        }

        // Submit transaction
        // Makes an API call to https://convex.world/tools/rest-api/submit-transaction
        public async Task<Result> SubmitTransaction(string hash, string sig)
        {
            if (Creds == null) throw new Exception("Missing credentials");

            TransactionRequest query = new TransactionRequest
            {
                accountKey = Creds.accountKey.accountKey,
                address = Creds.address.address,
                hash = hash,
                sig = sig
            };

            HttpResponseMessage response =
                await Client.PostAsJsonAsync<TransactionRequest>("https://convex.world/api/v1/transaction/submit",
                    query);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            if (responseBody != null)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                string val = jsonDocument.RootElement.GetProperty("value").ToString();
                Result result = new Result
                {
                    value = val ?? jsonDocument.RootElement.GetProperty("errorCode").ToString()
                };
                return result;
            }

            return null;
        }

        // Transact
        // This function prepares the transaction and then submits it.
        public async Task<Result> Transact(string source)
        {
            string hash = await PrepareTransaction(source);
            byte[] hash1 = KeyPair.StringToByteArray(hash);

            byte[] pk = KeyPair.StringToByteArray(Creds.keyPair.privateKey);
            string sign = KeyPair.Sign(hash1, pk);

            Result result = await SubmitTransaction(hash, sign);
            return result;
        }

        // Create account
        // This makes an API call to https://convex.world/tools/rest-api/create-an-account
        // Returns a convex address
        public async Task<Address> CreateAccount(AccountKey accountKey = null)
        {
            if (accountKey == null && Creds.accountKey == null)
            {
                throw new Exception("Missing credentials");
            }

            HttpResponseMessage response =
                await Client.PostAsJsonAsync("https://convex.world/api/v1/createAccount",
                    accountKey ?? Creds.accountKey);
            response.EnsureSuccessStatusCode();

            Address address = await response.Content.ReadFromJsonAsync<Address>();
            return address;
        }

        // Fetch account details
        // This makes and API call to https://convex.world/tools/rest-api/account-details
        // Returns the account details
        public async Task<AccountDetails> GetAccountDetails(Address address = null)
        {
            if (address == null && Creds == null)
            {
                throw new Exception("Missing credentials");
            }

            return await Client.GetFromJsonAsync<AccountDetails>(
                String.Format("https://convex.world/api/v1/accounts/{0}", address ?? Creds.address));
        }

        // Request coins
        // This makes an API call to https://convex.world/tools/rest-api/request-coins
        public async Task<Faucet> Faucet(int amount, Address address = null)
        {
            if (address == null && Creds == null)
            {
                throw new Exception("Missing credentials");
            }

            Faucet faucet = new Faucet
            {
                address = address?.address ?? Creds.address.address,
                amount = amount
            };

            HttpResponseMessage response =
                await Client.PostAsJsonAsync<Faucet>("https://convex.world/api/v1/faucet", faucet);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Faucet responseFaucet = JsonSerializer.Deserialize<Faucet>(responseBody);

            return responseFaucet;
        }

        // Query the test network
        // Makes an API call to https://convex.world/tools/rest-api/query
        public async Task<QueryResponse> Query(string source, Address address = null)
        {
            if (address == null && Creds == null)
            {
                throw new Exception("Missing credentials");
            }

            QueryRequest query = new QueryRequest();
            query.address = address?.address ?? Creds.address.address;
            query.source = source;

            HttpResponseMessage response =
                await Client.PostAsJsonAsync<QueryRequest>("https://convex.world/api/v1/query", query);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            QueryResponse res = new QueryResponse();

            if (responseBody != null)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                res.value = jsonDocument.RootElement.GetProperty("value").ToString();
            }
            else
            {
                res.source = "could not fetch the response";
            }

            return res;
        }
    }

    // This class is for interacting with fungible tokens
    public class FungibleLibrary
    {
        public Convex convex { get; }

        public FungibleLibrary(Convex cvx)
        {
            convex = cvx;
        }

        // Transfer tokens
        public async Task<Result> Transfer(Address token, Int16 holderSecretKey, Address holder = null,
            AccountKey holderAccountKey = null, Address receiver = null, int? amount = null)
        {
            return await convex.Transact($"(import convex.fungible :as fungible) " +
                                         $"(fungible/transfer {token} {receiver} {amount})");
        }

        // Create fungible tokens
        public async Task<FungibleToken> CreateToken(int supply, string name = null, string description = null)
        {
            if (convex.Creds == null) throw new Exception("Missing credentials.");
            string source = "{" + $":supply {supply} " + "}";
            Result result = await convex.Transact(("(import convex.fungible :as fungible) " +
                                                   $"(def my-coin (deploy [(fungible/build-token {source}) " +
                                                   "(fungible/add-mint {})]))"));
            Address address = Address.FromString(result.value as string);
            FungibleTokenMetadata fungibleTokenMetadata = new FungibleTokenMetadata
            {
                name = name ?? "Game coins",
                description = description ?? "These coins will be consumed on every play and can be top up."
            };
            FungibleToken fungibleToken = new FungibleToken(address, fungibleTokenMetadata);
            return fungibleToken;
        }

        // Consume fungible tokens
        public async Task<Result> MintToken(int supply)
        {
            if (convex.Creds == null) throw new Exception("Missing credentials.");
            return await convex.Transact(("(import convex.fungible :as fungible) " +
                                          $"(fungible/mint my-coin {supply})"));
        }

        // Check the balance of fungible tokens
        public async Task<Result> CheckBalance()
        {
            if (convex.Creds == null) throw new Exception("Missing credentials.");
            return await convex.Transact(("(import convex.fungible :as fungible) " +
                                          "(fungible/balance my-coin *address*)"));
        }

        // Destroy fungible tokens
        public async Task<Result> BurnToken(int supply)
        {
            if (convex.Creds == null) throw new Exception("Missing credentials.");
            return await convex.Transact(("(import convex.fungible :as fungible) " +
                                          $"(fungible/burn my-coin {supply})"));
        }
    }

    // This class is for interacting with non fungible tokens
    public class NonFungibleLibrary
    {
        public Convex convex { get; }

        public NonFungibleLibrary(Convex cvx)
        {
            convex = cvx;
        }

        // Create non fungible tokens
        public async Task<Result> CreateToken(Address caller, AccountKey callerAccountKey, UInt16 callerSecretKey,
            Dictionary<string, string> attributes = null)
        {
            var attrs = attributes ?? new Dictionary<string, string>();

            string name = attrs["name"] as string ?? "No name";
            string uri = attrs["uri"] as string;

            string data = "{" +
                          $":name \"{name},\" " +
                          $":uri \"{uri}\" " +
                          ":extra {} " + "}";

            string source = "(import convex.nft-tokens :as nft)" +
                            $"(deploy (nft/create-token {data} nil) )";

            return await convex.Transact(source);
        }
    }
}