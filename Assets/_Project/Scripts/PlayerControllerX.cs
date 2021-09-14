﻿using ConvexLib;
using UnityEngine;

namespace _Project.Scripts
{
    public class PlayerControllerX : MonoBehaviour
    {
        public bool gameOver;

        public float floatForce;
        private float gravityModifier = 1.5f;
        private Rigidbody playerRb;

        public ParticleSystem explosionParticle;
        public ParticleSystem fireworksParticle;

        private AudioSource playerAudio;
        public AudioClip moneySound;
        public AudioClip explodeSound;
        private float yAxisLimit = 15;

        private AccountKey _accountKey;
        private Account _account;


        // Start is called before the first frame update
        void Start()
        {
            Physics.gravity *= gravityModifier;
            playerAudio = GetComponent<AudioSource>();
            playerRb = GetComponent<Rigidbody>();

            StartConvex();
        }

        private async void StartConvex()
        {
            KeyPair keyPair = new KeyPair();
            string publicKey = keyPair.GenerateKeyPair().publicKey;
            AccountKey accountKey = new AccountKey
            {
                value = publicKey
            };

            ConvexLib.Convex convex = new ConvexLib.Convex();
            Account account = await convex.CreateAccount(accountKey);
            Debug.Log(account);

            Credentials credentials = new Credentials();
            credentials.address = Address.FromString("#4022");

            convex.SetCredentials(credentials);

            //Works
            //AccountDetails accountDetails = await game.GetAccountDetails(account.address);
            // AccountDetails accountDetails = await convex.GetAccountDetails(4422);
            // Debug.Log(accountDetails);
            //
            // //Works
            // Faucet faucet = await convex.Faucet(251, 1000);
            // Debug.Log(faucet);
            //
            //Works
            // QueryResponse loginResponse = await convex.Query("(map inc [1 2 3])");
            // Debug.Log(loginResponse);

            // string resp = await convex.Transact();
            // Debug.Log("Response " + resp);
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y > yAxisLimit)
            {
                transform.position = new Vector3(transform.position.x, yAxisLimit, transform.position.z);
            }

            // While space is pressed and player is low enough, float up
            if (Input.GetKey(KeyCode.Space) && !gameOver)
            {
                playerRb.AddForce(Vector3.up * floatForce);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // Apply a small upward force at the start of the game
                playerRb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            }

            // if player collides with bomb, explode and set gameOver to true
            if (other.gameObject.CompareTag("Bomb"))
            {
                explosionParticle.Play();
                playerAudio.PlayOneShot(explodeSound, 1.0f);
                gameOver = true;
                Debug.Log("Game Over!");
                Destroy(other.gameObject);
            }

            // if player collides with money, fireworks
            else if (other.gameObject.CompareTag("Money"))
            {
                fireworksParticle.Play();
                playerAudio.PlayOneShot(moneySound, 1.0f);
                Destroy(other.gameObject);
            }
        }
    }
}