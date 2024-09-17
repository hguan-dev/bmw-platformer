using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer{

    public class Car : MonoBehaviour
    {
        private GameManager gameManager;

        void Start()
        {
            // Find the GameManager in the scene
            gameManager = GameObject.FindObjectOfType<GameManager>();

            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene.");
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Send a message to the GameManager to handle victory
                gameManager.WinGame();
            }
        }
    }  
}