using UnityEngine;

namespace ServiceLocatorPath
{
    public class Installer : MonoBehaviour
    {
        private void Awake()
        {
            var playFab = new PlayFabCustom();
            ServiceLocator.Instance.RegisterService<IPlayFabCustom>(playFab);
            if (GameObject.FindObjectsOfType<Installer>().Length > 1)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}