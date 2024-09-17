using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Platformer{
    public class Coin : MonoBehaviour
    {
        public float timeToSubtract = 3f; 
        private GameManager gameManager;

        void Start()
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();

            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene.");
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                gameManager.PlayCoinPickupSound();
                gameManager.SubtractTime(timeToSubtract);
                Destroy(gameObject);
            }
        }
    }
}