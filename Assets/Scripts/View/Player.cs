using Model;
using UnityEngine;

namespace View
{
    public class Player : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Respawn"))
            {
                Debug.Log($"Una vida menos. Vida: {RunSession.Current.Life}");
            }
        }
    }
}
