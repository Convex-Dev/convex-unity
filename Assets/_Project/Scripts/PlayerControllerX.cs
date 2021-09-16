using ConvexLib;
using UnityEngine;
using TMPro;

namespace _Project.Scripts
{
    public class PlayerControllerX : MonoBehaviour
    {
        public bool gameOver;
        public TextMeshProUGUI scoreText;
        private int score;

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

            score = 0;
            UpdateScore(0);

            ConvexAPI convexAPI = new ConvexAPI();
            convexAPI.InitConvex();
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

        public void UpdateScore(int scoreToAdd, bool? reset = null)
        {
            if (reset == true)
            {
                scoreText.text = "Score: " + scoreToAdd;
            }
            else
            {
                score += scoreToAdd;
                scoreText.text = "Score: " + score;
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
                UpdateScore(0, true);
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
                UpdateScore(5);
                playerAudio.PlayOneShot(moneySound, 1.0f);
                Destroy(other.gameObject);
            }
        }
    }
}