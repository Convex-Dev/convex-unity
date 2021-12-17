# Unity game library

This folder contains code for the Convex Unity game library.

The library has 3 main classes which interact with the convex network;
1. Convex class
2. FungibleLibrary class
3. NonFungibleLibrary class

The `convex` class contains the code to interact with the convex test network directly. 
The supported API calls in `convex` class are;
1. Create account
```c#
public readonly ConvexLib.Convex Cvx;

// Create private and public key
KeyPair keyPair = new KeyPair();
string publicKey = keyPair.publicKey;

AccountKey accountKey = new AccountKey(publicKey);
Address address = await Cvx.CreateAccount(accountKey);
Credentials newCred = new Credentials(address, accountKey, keyPair);

// Save Credentials so that we can avoid passing it to all functions 
Cvx.SetCredentials(credentials)
```
2. Fetch account details
```c#
AccountDetails accountDetails = await Cvx.GetAccountDetails(address);
Account account = accountDetails.ToAccount();
```
3. Request coins
```c#
int amount 50;
Faucet faucet = await Cvx.Faucet(amount);
```
4. Query 
```c#
QueryResponse queryResponse = await Cvx.Query(source);
Result result = queryResponse.value;
```
5. Prepare transaction
```c#
string hash = await PrepareTransaction(source);
```
6. Submit transaction
```c#
string hash = await PrepareTransaction(source);
byte[] hash1 = KeyPair.StringToByteArray(hash);

byte[] pk = KeyPair.StringToByteArray(Creds.keyPair.privateKey);
string sign = KeyPair.Sign(hash1, pk);
            
Result result = await Cvx.SubmitTransaction(hash, sign);
```
Prepare transaction and Submit transaction can be done with one call more neatly as;
```c#
Result result = await Cvx.Transact(source);
```

Also the library includes functionality to handle fungible tokens. 
The code can be found in the `FungibleLibrary` class and has support for
1. Creation of fungible tokens
```c#
FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
Result balance = await fungibleLibrary.Transfer(10);
```
2. Transfer of fungible tokens
```c#
Address sender = new Address(senderAddress);
Address receiver = new Address(receiverAdress);
int amount = 10;

FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
Result balance = await fungibleLibrary.Transfer(sender, receiver, amount);
```
3. Minting of fungible tokens
```c#
FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
Result balance = await fungibleLibrary.MintToken(10);
```
4. Checking balance of fungible tokens
```c#
FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
Result result = await fungibleLibrary.CheckBalance();
```
5. Burning fungible tokens
```c#
FungibleLibrary fungibleLibrary = new FungibleLibrary(Cvx);
Result balance = await fungibleLibrary.BurnToken(10);
```

Support for non fungible tokens is still in development with the following supported endpoints;
1. Creation of non fungible tokens

Code for handling non fungible tokens can be found at `NonFungibleLibrary`

Copyright (c) 2021 [Superstruct Ltd](https://superstruct.nz/)
