using System.Threading.Tasks;
using ConvexLib;
using UnityEngine;

namespace _Project.Scripts
{
    public class ConvexAPI
    {
        public readonly ConvexLib.Convex Cvx;

        public ConvexAPI()
        {
            Cvx = new ConvexLib.Convex();
        }

        public async Task<Address> GameCreateAccount()
        {
            KeyPair keyPair = new KeyPair();
            string publicKey = keyPair.publicKey;
            AccountKey accountKey = new AccountKey(publicKey);

            Address address = await Cvx.CreateAccount(accountKey);
            Debug.Log(address.address);

            Credentials credentials = new Credentials(address, accountKey, keyPair);
            Cvx.SetCredentials(credentials);

            return address;
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
            Debug.Log(accountDetails.Balance);

            // Request addition to balance
            Faucet faucet = await Cvx.Faucet(99999999, accountDetails.Address);

            Account account = await FetchAccount(accountDetails.Address);
            Debug.Log(account.Balance);

            CreatePesos();
        }

        // Pesos is in game currency
        // Create 10 pesos when the user signs up
        public async void CreatePesos(int? amount = null)
        {
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
            FungibleToken token = await fungibleLibrary.CreateToken(amount ?? 10);
            Debug.Log(token.address);
            
        }

        // Consume 1 coins every time the game is played
        public async void BurnPesos(int? amount = null)
        {
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
            Result balance = await fungibleLibrary.BurnToken(amount ?? 1);
            Debug.Log(balance.value);   
        }

        // Award 2 coins every time the user hits their highest score
        public async void HitHighScore()
        {
            FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
            Result balance = await fungibleLibrary.MintToken(10);
            Debug.Log(balance.value);
        }
    }
}