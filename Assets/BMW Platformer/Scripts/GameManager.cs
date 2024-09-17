using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerGameObject;
        private PlayerController player;
        public GameObject deathPlayerPrefab;

        public Text timerText;
        public Text bestScoreText;
        public GameObject winModalInstance;
        public GameObject deathModalInstance;
        public AudioSource musicSource;
        public AudioSource winSound;
        public AudioSource gameOverSound;
        public AudioSource coinPickupSound;

        private Canvas canvas;
        private float timer;
        private static float bestScore = float.MaxValue;
        private bool gameActive;

        void Start()
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            gameActive = true;

            UpdateBestScoreText();

            canvas = FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                Debug.LogError("No Canvas found in the scene. Please add a Canvas to your scene.");
                return;
            }

            if (winModalInstance != null)
            {
                winModalInstance.SetActive(false);
            }
            if (deathModalInstance != null)
            {
                deathModalInstance.SetActive(false);
            }

            // Ensure the background music loops indefinitely
            if (musicSource != null)
            {
                musicSource.loop = true;
                RestartMusic();
            }
        }

        void Update()
        {
            if (gameActive)
            {
                timer += Time.deltaTime;
                timerText.text = "Time: " + timer.ToString("F2") + "s";
            }

            if (player.deathState == true)
            {
                EndGame();
            }
        }

        public void SubtractTime(float amount)
        {
            timer -= amount;
            if (timer < 0)
            {
                timer = 0;
            }
            timerText.text = "Time: " + timer.ToString("F2") + "s";
        }

        public void PlayCoinPickupSound()
        {
            if (coinPickupSound != null)
            {
                coinPickupSound.Play();
            }
        }

        public void EndGame()
        {
            gameActive = false;

            if (deathModalInstance != null)
            {
                deathModalInstance.SetActive(true);

                Text[] textFields = deathModalInstance.GetComponentsInChildren<Text>();
                foreach (Text textField in textFields)
                {
                    if (textField.name == "DeathText")
                    {
                        textField.text = "You fell victim to Mercedes-Benz!";
                    }
                    else if (textField.name == "CountdownText")
                    {
                        StartCoroutine(UpdateCountdownText(textField, 3, "Respawning"));
                    }
                }

                if (gameOverSound != null) {
                    gameOverSound.Play();
                }

                playerGameObject.SetActive(false);
            }

            GameObject deathPlayer = Instantiate(deathPlayerPrefab, playerGameObject.transform.position, playerGameObject.transform.rotation);
            deathPlayer.transform.localScale = new Vector3(playerGameObject.transform.localScale.x, playerGameObject.transform.localScale.y, playerGameObject.transform.localScale.z);
            player.deathState = false;
            
            StartCoroutine(FadeOutMusic(2f));
            Invoke("ReloadLevel", 3);
        }

        public void WinGame()
        {
            gameActive = false;
            if (timer < bestScore)
            {
                bestScore = timer;
            }

            if (winModalInstance != null)
            {
                winModalInstance.SetActive(true);

                Text[] textFields = winModalInstance.GetComponentsInChildren<Text>();
                foreach (Text textField in textFields)
                {
                    if (textField.name == "WinText")
                    {
                        textField.text = "You got in the BMW!!! You Win!";
                    }
                    else if (textField.name == "TimeText")
                    {
                        textField.text = "Time: " + timer.ToString("F2") + "s";
                    }
                    else if (textField.name == "FastestTimeText")
                    {
                        textField.text = "Best Time: " + bestScore.ToString("F2") + "s";
                    }
                    else if (textField.name == "CountdownText")
                    {
                        StartCoroutine(UpdateCountdownText(textField, 3, "Game resetting"));
                    }
                }
                if (winSound != null)
                {
                    winSound.Play();
                }
                
                playerGameObject.SetActive(false);
            }

            StartCoroutine(FadeOutMusic(2f));
            Invoke("ReloadLevel", 3);
        }

        private IEnumerator UpdateCountdownText(Text countdownText, int seconds, string actionText)
        {
            while (seconds > 0)
            {
                countdownText.text = actionText + " in " + seconds + " seconds...";
                yield return new WaitForSeconds(1f);
                seconds--;
            }
            countdownText.text = actionText + " now...";
        }

        private void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void UpdateBestScoreText()
        {
            Debug.Log("Best Score: " + bestScore);
            
            if (bestScore == float.MaxValue)
            {
                bestScoreText.text = "Best Time: --";
            }
            else
            {
                bestScoreText.text = "Best Time: " + bestScore.ToString("F2") + "s";
            }
        }

        private void RestartMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();  
                musicSource.Play(); 
            }
        }

        private IEnumerator FadeOutMusic(float fadeDuration)
        {
            if (musicSource != null)
            {
                float startVolume = musicSource.volume;

                while (musicSource.volume > 0)
                {
                    musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                    yield return null;
                }

                musicSource.Stop(); // Stop the music completely when the fade-out is done
                musicSource.volume = startVolume; // Reset the volume for the next play
            }
        }

    }
}
