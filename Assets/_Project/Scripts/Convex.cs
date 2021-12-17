using System.Threading.Tasks;
using ConvexLib;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Scripts
{
    public class ConvexAPI
    {
        public readonly ConvexLib.Convex Cvx;
        private string credsKey = "credsKey";
        private string tokenKey = "tokenKey";
        private string balanceKey = "balanceKey";

        public ConvexAPI()
        {
            Cvx = new ConvexLib.Convex();
        }

        private void UpdateStateBalance(int score)
        {
            PlayerPrefs.SetInt(balanceKey, score);
            PlayerPrefs.Save();
            Debug.Log($"Updated available pesos balance {score}");
        }

        private void UpdateStateToken(string token)
        {
            PlayerPrefs.SetString(tokenKey, token);
            PlayerPrefs.Save();
            Debug.Log($"Updated token {token}");
        }

        private void UpdateStateCredentials(string credentials)
        {
            PlayerPrefs.SetString(credsKey, credentials);
            PlayerPrefs.Save();
            Debug.Log($"Updated credentials {credentials}");
        }

        private void SaveCredentials(Credentials credentials)
        {
            string json = JsonConvert.SerializeObject(credentials);
            UpdateStateCredentials(json);
        }

        public async Task<Address> GameCreateAccount()
        {
            Credentials credentials;
            string cache = PlayerPrefs.GetString(credsKey, "");

            if (cache.Length > 0)
            {
                Credentials cred = JsonConvert.DeserializeObject<Credentials>(cache);
                credentials = cred;
                Debug.Log("Deserialized " + credentials);
            }
            else
            {
                KeyPair keyPair = new KeyPair();
                string publicKey = keyPair.publicKey;
                AccountKey accountKey = new AccountKey(publicKey);

                Address address = await Cvx.CreateAccount(accountKey);
                Debug.Log(address.address);

                Credentials newCred = new Credentials(address, accountKey, keyPair);
                credentials = newCred;

                SaveCredentials(newCred);
            }

            Cvx.SetCredentials(credentials);

            return credentials.address;
        }

        public async Task<Account> FetchAccount(Address address)
        {
            AccountDetails accountDetails = await Cvx.GetAccountDetails(address);
            Account account = accountDetails.ToAccount();

            return account;
        }

        public async Task<Account> InitAccount()
        {
            Address address = await GameCreateAccount();
            return await FetchAccount(address);
        }

        public async void InitConvex()
        {
            Account accountDetails = await InitAccount();

            if (accountDetails.Balance < 1000000)
            {
                Faucet faucet = await Cvx.Faucet(99999999, accountDetails.Address);
            }

            CreatePesos();
        }

        // Pesos is in game currency
        // Create 10 pesos when the user signs up
        public async void CreatePesos(int? amount = null)
        {
            string cache = PlayerPrefs.GetString(tokenKey, "");
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);

            if (cache.Length > 0)
            {
                FungibleToken token = JsonConvert.DeserializeObject<FungibleToken>(cache);
                Debug.Log($"Fetched token {token.address.address}");

                Result result = await fungibleLibrary.CheckBalance();
                if (result.value is string val && val != "")
                {
                    int bal = int.Parse(val);
                    UpdateStateBalance(bal);
                }
                else
                {
                    Debug.Log("Could not check balance.");
                }

                BurnPesos();
            }
            else
            {
                FungibleToken token = await fungibleLibrary.CreateToken(amount ?? 10);
                string json = JsonConvert.SerializeObject(token);
                UpdateStateToken(json);
            }
        }

        // Consume 1 coins every time the game is played
        public async void BurnPesos(int amount = 1)
        {
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
            int cache = PlayerPrefs.GetInt(balanceKey);
            if (amount <= cache)
            {
                Result balance = await fungibleLibrary.BurnToken(amount);
                if (balance.value is string val && val != "")
                {
                    int bal = int.Parse(val);
                    UpdateStateBalance(bal);
                }
            }
            else
            {
                Debug.Log($"Could not consume {amount} tokens from {cache}");
            }
        }

        // Award 2 coins every time the user hits their highest score
        public async void HitHighScore()
        {
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
            Result balance = await fungibleLibrary.MintToken(10);
            if (!(balance.value is string))
            {
                Debug.Log("Could not award tokens.");
            }
        }

        public async void EndGame()
        {
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
            Result result = await fungibleLibrary.CheckBalance();
            if (result.value is string val && val != "")
            {
                Debug.Log($"Val \n {val}");
                int bal = int.Parse(val);
                UpdateStateBalance(bal);
            }
            else
            {
                Debug.Log("Could not check balance.");
            }
        }
    }
}