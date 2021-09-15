using System;
using System.Buffers.Text;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace ConvexLib
{
    public class KeyPair
    {
        public string privateKey { get; set; }
        public string publicKey { get; set; }

        //Generates a private and public Ed25519 keys
        public KeyPair()
        {
            Ed25519KeyPairGenerator keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();

            Ed25519PrivateKeyParameters pairPrivate = (Ed25519PrivateKeyParameters) keyPair.Private;
            string prv = ByteArrayToString(pairPrivate.GetEncoded());

            Ed25519PublicKeyParameters pairPublic = (Ed25519PublicKeyParameters) keyPair.Public;
            string pub = ByteArrayToString(pairPublic.GetEncoded());

            privateKey = prv;
            publicKey = pub;
        }

        public static string Sign(byte[] msg, byte[] secretKey)
        {
            var signer = SignerUtilities.GetSigner("Ed25519");
            signer.Init(true, new Ed25519PrivateKeyParameters(secretKey, 0));
            signer.BlockUpdate(msg, 0, msg.Length);

            byte[] sign = signer.GenerateSignature();
            return ByteArrayToString(sign);
        }

        // The following methods come with .NET 5 but we are using 4.7
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);

            for (int i = 0; i < ba.Length; i++)
                hex.Append(ba[i].ToString("X2"));

            return hex.ToString();
        }

        public static byte[] StringToByteArray(String hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}