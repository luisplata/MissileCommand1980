using UnityEngine;

namespace View
{
    public abstract class ObjectDestroyer : MonoBehaviour
    {
        public abstract void GetImpact(float damage);
    }
}