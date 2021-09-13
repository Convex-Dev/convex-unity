using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConvexLib
{
    public class Game
    {
        private static readonly HttpClient client = new HttpClient();

        // Create account
        public async Task<Account> CreateAccount()
        {
            try
            {
                KeyPair keyPair = new KeyPair().GenerateKeyPair();
                AccountKey accountKey = new AccountKey {accountKey = keyPair.publicKey};
                HttpResponseMessage response =
                    await client.PostAsJsonAsync("https://convex.world/api/v1/createAccount", accountKey);
                response.EnsureSuccessStatusCode();

                Account account = await response.Content.ReadFromJsonAsync<Account>();
                return account;
            }  
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Account errorAccount = new Account {error = e.Message};
                return errorAccount;
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Account errorAccount = new Account {error = e.Message};
                return errorAccount;
            }
            catch (JsonException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Account errorAccount = new Account {error = e.Message};
                return errorAccount;
            } 
        }

        // Get account details
        public async Task<AccountDetails> GetAccountDetails(int address)
        {
            try
            {
                return await client.GetFromJsonAsync<AccountDetails>(
                    String.Format("https://convex.world/api/v1/accounts/{0}", address));
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (JsonException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return null;
        }

        // Faucet
        public async Task<Faucet> Faucet(int address, int amount)
        {
            try
            {
                Faucet faucet = new Faucet();
                faucet.address = address;
                faucet.amount = amount;

                HttpResponseMessage response =
                    await client.PostAsJsonAsync<Faucet>("https://convex.world/api/v1/faucet", faucet);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Faucet responseFaucet = JsonSerializer.Deserialize<Faucet>(responseBody);

                return responseFaucet;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (JsonException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return null;
        }

        // Query
        public async Task<QueryResponse> Query(int address, string source)
        {
            try
            {
                QueryRequest query = new QueryRequest();
                query.address = address;
                query.source = source;

                HttpResponseMessage response =
                    await client.PostAsJsonAsync<QueryRequest>("https://convex.world/api/v1/query", query);
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
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                QueryResponse response = new QueryResponse {errorCode = "HttpRequestException", source = e.Message};
                return response;
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                QueryResponse response = new QueryResponse {errorCode = "NotSupportedException", source = e.Message};
                return response;
            }
            catch (JsonException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                QueryResponse response = new QueryResponse {errorCode = "JsonException", source = e.Message};
                return response;
            }
        }
        // Prepare transaction
    }
}