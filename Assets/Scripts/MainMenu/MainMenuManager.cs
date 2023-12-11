using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Milan.GrassBubble.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup optionsCanvas;
        [SerializeField]
        CanvasGroup mainMenuCanvas;
        [SerializeField]
        Canvas canvas;
        // Start is called before the first frame update
        void Start()
        {
            canvas.sortingOrder = -1;
            Global.debugSettings.isEnabled = false;
            Cursor.visible = true;
            Global.currentLevel = 0;
            optionsCanvas.gameObject.SetActive(false);
            mainMenuCanvas.gameObject.SetActive(true);
            FadeController.Fade(FadeController.FadeColor.White, FadeController.FadeColor.Clear, FadeController.FadeType.EaseInOutQuad, 3, AllowClick);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                SceneManager.LoadScene(Global.Scenes.Debug);
            }
        }
        void AllowClick()
        {
            canvas.sortingOrder = 0;
        }
        public void OnStartClick()
        {
            canvas.sortingOrder = -1;
            FadeController.Fade(FadeController.FadeColor.Clear, FadeController.FadeColor.White, FadeController.FadeType.EaseOutQuart, 5, StartGame);
        }
        public void OnQuitClick()
        {
            canvas.sortingOrder = -1;
            StartCoroutine(QuitGame(0.75f));
        }
        public void OnOptionClick()
        {
            mainMenuCanvas.gameObject.SetActive(false);
            optionsCanvas.gameObject.SetActive(true);
        }
        public void OnDebugClick()
        {
            // SceneManager.LoadScene("");
        }
        public void OnReturnToMainMenuClick()
        {
            optionsCanvas.gameObject.SetActive(false);
            mainMenuCanvas.gameObject.SetActive(true);
        }
        void StartGame()
        {
            SceneManager.LoadScene(Global.Scenes.Intro);
        }
        IEnumerator QuitGame(float secondsToWait)
        {
            FadeController.Fade(FadeController.FadeColor.Clear, FadeController.FadeColor.Black, FadeController.FadeType.EaseOutQuart, secondsToWait);
            yield return new WaitForSeconds(secondsToWait + 0.5f);
            Application.Quit();
        }
    }
}