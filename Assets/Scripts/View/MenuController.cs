using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace View
{
    public class MenuController : MonoBehaviour
    {
        public const string MainMenuScene = "MainMenu";
        public const string GameScene = "Game";

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);   
        }

        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
