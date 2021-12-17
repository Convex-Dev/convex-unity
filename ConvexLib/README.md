# Unity game library

This folder contains code for the Convex Unity game library.

The library has 3 main classes which interact with the convex network;
1. Convex class
2. FungibleLibrary class
3. NonFungibleLibrary class

The `convex` class contains the code to interact with the convex test network directly. 
The supported API calls in `convex` class are;
1. Create account
2. Fetch account details
3. Request coins
4. Query 
5. Prepare transaction
6. Submit transaction

Also the library includes functionality to handle fungible tokens. 
The code can be found in the `FungibleLibrary` class and has support for
1. Creation of fungible tokes
2. Transfer of fungible tokens
3. Minting of fungible tokens
4. Checking balance of fungible tokens
5. Burning fungible tokens

Support for non fungible tokens is still in development with the following supported endpoints;
1. Creation of non fungible tokens

Code for handling non fungible tokens can be found at `NonFungibleLibrary`

Copyright (c) 2021 [Superstruct Ltd](https://superstruct.nz/)
