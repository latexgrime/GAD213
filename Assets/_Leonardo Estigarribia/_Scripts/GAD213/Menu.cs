using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Leonardo_Estigarribia._Scripts
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        
        [SerializeField] private GameObject pauseText;
        [SerializeField] private GameObject youWonText;
        [SerializeField] private GameObject youLostText;

        [SerializeField] private GameObject continueButton;

        [SerializeField] private GameObject instructionsMenu;

        private void Start()
        {
            pauseMenu.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }

        public void CloseInstructionsMenu()
        {
            instructionsMenu.SetActive(false);
        }

        private void PauseGame()
        {
            pauseMenu.SetActive(true);
            pauseText.SetActive(true);
            continueButton.SetActive(true);
            youWonText.SetActive(false);
            youLostText.SetActive(false);
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            pauseText.SetActive(false);
            continueButton.SetActive(false);
            youWonText.SetActive(false);
            youLostText.SetActive(false);
            Time.timeScale = 1;
        }

        public void GameLostMenu()
        {
            pauseMenu.SetActive(true);
            pauseText.SetActive(false);
            continueButton.SetActive(false);
            youWonText.SetActive(false);
            youLostText.SetActive(true);
            Time.timeScale = 0;
        }

        public void GameWonMenu()
        {
            pauseMenu.SetActive(true);
            pauseText.SetActive(false);
            continueButton.SetActive(false);
            youWonText.SetActive(true);
            youLostText.SetActive(false);
            Time.timeScale = 0;        
        }
        
        public void RestartScene()
        {
            Time.timeScale = 1;
            var currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
