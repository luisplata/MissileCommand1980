using UnityEngine;

namespace View
{
    public class HouseController : ObjectDestroyer
    {
        public FloorController.OnCollisionEnterFromBullet OnCollisionFromBullet;

        public override void GetImpact(float damage)
        {
            OnCollisionFromBullet?.Invoke(damage);
        }
    }
}