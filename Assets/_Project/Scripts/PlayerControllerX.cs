using System;
using ConvexLib;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class PlayerControllerX : MonoBehaviour
    {
        public ConvexAPI ConvexAPI;
        public bool gameOver;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI gameOverText;
        private int score;
        public Button restartButton;
        public Button requestTokens;

        public float floatForce;
        private float gravityModifier = 1.5f;
        private bool modifyGravity;
        private Rigidbody playerRb;

        public ParticleSystem explosionParticle;
        public ParticleSystem fireworksParticle;

        private AudioSource playerAudio;
        public AudioClip moneySound;
        public AudioClip explodeSound;
        private float yAxisLimit = 15;

        private AccountKey _accountKey;
        private Account _account;

        private string highscoreKey = "hskey";
        public int highscore;

        private string balanceKey = "balanceKey";


        // Start is called before the first frame update
        void Start()
        {
            if (modifyGravity == false)
            {
                Physics.gravity *= gravityModifier;
                modifyGravity = false;
            }

            playerAudio = GetComponent<AudioSource>();
            playerRb = GetComponent<Rigidbody>();

            int cache = PlayerPrefs.GetInt(balanceKey);
            
            ConvexAPI = new ConvexAPI();
            ConvexAPI.InitConvex();

            score = 0;
            UpdateScore(0);

            highscore = PlayerPrefs.GetInt(highscoreKey);

            if (cache == 0)
            {
                GameOver();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.y > yAxisLimit)
            {
                transform.position = new Vector3(transform.position.x, yAxisLimit, transform.position.z);
            }

            // While space is pressed and player is low enough, float up
            if (Input.GetKey(KeyCode.Space) && !gameOver && transform.position.y < yAxisLimit)
            {
                playerRb.AddForce(Vector3.up * floatForce);
            }
        }

        public void UpdateScore(int scoreToAdd, bool? reset = null)
        {
            if (reset == true)
            {
                score = 0;
                scoreText.text = "Score: " + scoreToAdd;
            }
            else
            {
                score += scoreToAdd;
                scoreText.text = "Score: " + score;
            }
        }

        public void GameOver()
        {
            if (score > highscore)
            {
                ConvexAPI.HitHighScore();

                PlayerPrefs.SetInt(highscoreKey, score);
                PlayerPrefs.Save();
                Debug.Log("Hit highscore, congratulations.");
            }
            else
            {
                int cache = PlayerPrefs.GetInt(balanceKey);
                if (cache == 0)
                {
                    requestTokens.gameObject.SetActive(true);
                }
            }

            ConvexAPI?.EndGame();

            gameOverText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            gameOver = true;
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gameOver = false;
        }

        public void RequestTokens()
        {
            ConvexAPI.HitHighScore();
        }

        public void RequestAndRestart()
        {
            RequestTokens();
            RestartGame();
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
                GameOver();
                Destroy(other.gameObject);
            }

            // if player collides with money, fireworks
            else if (other.gameObject.CompareTag("Money"))
            {
                fireworksParticle.Play();
                UpdateScore(5);
                playerAudio.PlayOneShot(moneySound, 1.0f);
                Destroy(other.gameObject);
            }
        }
    }
}