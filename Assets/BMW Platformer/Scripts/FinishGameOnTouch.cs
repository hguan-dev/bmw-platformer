using UnityEngine;

namespace Platformer
{
    public class FinishGameOnTouch : MonoBehaviour
    {
        private GameManager gameManager;

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>(); // Find the GameManager script in the scene
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player") // Ensure the player GameObject has the "Player" tag
            {
                Debug.Log("Player touched the car. Game Over!");
                gameManager.EndGame(); // Call the EndGame method in the GameManager script
            }
        }
    }
}
