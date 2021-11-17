using UnityEngine;

namespace View
{
    public class FloorController : ObjectDestroyer
    {
        public delegate void OnCollisionEnterFromBullet(float damage);

        public OnCollisionEnterFromBullet OnCollisionFromBullet;

        public override void GetImpact(float damage)
        {
            OnCollisionFromBullet?.Invoke(damage);
        }
    }
}
