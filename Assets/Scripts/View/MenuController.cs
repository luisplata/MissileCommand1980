using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace View
{
    public class MenuController : MonoBehaviour
    {
        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);   
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
