using UnityEngine;

namespace ServiceLocatorPath
{
    public class Installer : MonoBehaviour
    {
        private void Awake()
        {
            if (GameObject.FindObjectsOfType<Installer>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            var playFab = new PlayFabCustom();
            ServiceLocator.Instance.RegisterService<IPlayFabCustom>(playFab);
            DontDestroyOnLoad(gameObject);
        }
    }
}