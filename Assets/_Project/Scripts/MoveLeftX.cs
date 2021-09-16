using UnityEngine;

namespace _Project.Scripts
{
    public class MoveLeftX : MonoBehaviour
    {
        private PlayerControllerX playerControllerScript;
        private float leftBound = -10;
        public float speed;

        // Start is called before the first frame update
        void Start()
        {
            playerControllerScript = GameObject.Find("Player").GetComponent<PlayerControllerX>();
        }

        // Update is called once per frame
        void Update()
        {
            // If game is not over, move to the left
            if (!playerControllerScript.gameOver)
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
            }

            // If object goes off screen that is NOT the background, destroy it
            if (transform.position.x < leftBound && !gameObject.CompareTag("Background"))
            {
                if (gameObject.CompareTag("Bomb"))
                {
                    playerControllerScript.UpdateScore(1);
                }
                Destroy(gameObject);
            }

        }
    }
}
