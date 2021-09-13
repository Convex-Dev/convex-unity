using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace ConvexLib
{
    public class KeyPair
    {
        public string privateKey { get; set; }
        public string publicKey { get; set; }

        //Generates a private and public Ed25519 keys
        public KeyPair GenerateKeyPair()
        {
            Ed25519KeyPairGenerator keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();

            Ed25519PrivateKeyParameters pairPrivate = (Ed25519PrivateKeyParameters) keyPair.Private;
            string prv = Convert.ToBase64String(pairPrivate.GetEncoded());

            Ed25519PublicKeyParameters pairPublic = (Ed25519PublicKeyParameters) keyPair.Public;
            string pub = Convert.ToBase64String(pairPublic.GetEncoded());

            return new KeyPair {privateKey = prv, publicKey = pub};
        }
    }
}