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
    public class Convex
    {
        private static readonly HttpClient Client = new HttpClient();
        public Credentials Creds;

        public void SetCredentials(Credentials cred)
        {
            Creds = cred;
        }

        public async Task<string> PrepareTransaction(string source, Address address = null, int? sequence = null)
        {
            if (address == null && Creds == null) throw new Exception("Missing credentials");

            QueryRequest query = new QueryRequest();
            query.address = address ?? Creds.address;
            query.source = source;

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

        public async Task<Result> SubmitTransaction(string hash, string sig)
        {
            if (Creds == null) throw new Exception("Missing credentials");

            TransactionRequest query = new TransactionRequest
            {
                hash = hash,
                sig = sig
            };

            HttpResponseMessage response =
                await Client.PostAsJsonAsync<TransactionRequest>("https://convex.world/api/v1/transaction/prepare",
                    query);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            if (responseBody != null)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                string val = jsonDocument.RootElement.GetProperty("value").ToString();
                Result result = new Result();

                if (val != null)
                {
                    result.value = val;
                }
                else
                {
                    result.value = jsonDocument.RootElement.GetProperty("errorCode").ToString();
                }

                return result;
            }

            return null;
        }

        public async Task<Result> Transact(string source)
        {
            string hash = await PrepareTransaction("(import convex.core :as core) (core/* 3 4)");

            //todo sign hash with credentials.secretKey
            string sign = "";

            Result result = await SubmitTransaction(hash, sign);
            return result;
        }

        // Create account
        public async Task<Account> CreateAccount(AccountKey accountKey = null)
        {
            if (accountKey == null && Creds.accountKey == null)
            {
                throw new Exception("Missing credentials");
            }

            HttpResponseMessage response =
                await Client.PostAsJsonAsync("https://convex.world/api/v1/createAccount",
                    accountKey.value ?? Creds.accountKey);
            response.EnsureSuccessStatusCode();

            Account account = await response.Content.ReadFromJsonAsync<Account>();
            return account;
        }

        // Get account details
        public async Task<AccountDetails> GetAccountDetails(Address address = null)
        {
            if (address == null && Creds == null)
            {
                throw new Exception("Missing credentials");
            }

            return await Client.GetFromJsonAsync<AccountDetails>(
                String.Format("https://convex.world/api/v1/accounts/{0}", address ?? Creds.address));
        }

        // Faucet
        public async Task<Faucet> Faucet(int amount, Address address = null)
        {
            if (address == null && Creds == null)
            {
                throw new Exception("Missing credentials");
            }

            Faucet faucet = new Faucet();
            faucet.address = address ?? Creds.address;
            faucet.amount = amount;

            HttpResponseMessage response =
                await Client.PostAsJsonAsync<Faucet>("https://convex.world/api/v1/faucet", faucet);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Faucet responseFaucet = JsonSerializer.Deserialize<Faucet>(responseBody);

            return responseFaucet;
        }

        // Query
        public async Task<QueryResponse> Query(string source, Address address = null)
        {
            if (address == null && Creds == null)
            {
                throw new Exception("Missing credentials");
            }

            QueryRequest query = new QueryRequest();
            query.address = address ?? Creds.address;
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

    public class FungibleLibrary
    {
        public Convex convex { get; }

        public FungibleLibrary()
        {
            convex = new Convex();
        }

        public async Task<Result> Transfer(Address token, Int16 holderSecretKey, Address holder = null,
            AccountKey holderAccountKey = null, Address receiver = null, int? amount = null)
        {
            return await convex.Transact($"(import convex.fungible :as fungible) " +
                                         $"(fungible/transfer {token} {receiver} {amount})");
        }

        public async Task<Result> CreateToken(int supply)
        {
            string source = "{" + $":supply {supply}" + "}";
            return await convex.Transact(("(import convex.fungible :as fungible) " +
                                          $"(deploy [(fungible/build-token {source}) " + "(fungible/add-mint {})])"));
        }
    }

    public class NonFungibleLibrary
    {
        public Convex convex { get; }

        public NonFungibleLibrary()
        {
            convex = new Convex();
        }

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