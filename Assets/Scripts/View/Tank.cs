using UnityEngine;

namespace View
{
    public class Tank : MonoBehaviour
    {
        [SerializeField] private GameObject canion;
        [SerializeField] private GameObject pointToExit;
        [SerializeField] private float angleMore;
        [SerializeField] private float min;
        [SerializeField] private GameObject bullet;

        private bool _rotating;
        private Vector2 point;

        private Quaternion concurrentAngle;
        // Update is called once per frame
        void Update()
        {
            if (_rotating)
            {
                var angle = GetAngle();
                var rotation = canion.transform.rotation;
                var diff = Quaternion.Slerp(rotation, Quaternion.Euler(0, 0, angle), angleMore * Time.deltaTime);
                var diffRotationZ = rotation.z - Quaternion.Euler(0, 0, angle).z;
                if (Mathf.Abs(diffRotationZ) < min)
                {
                    FireBullet();
                }
                canion.transform.rotation = diff;
            }
        }

        private void FireBullet()
        {
            var bulletInstantiate = Instantiate(bullet, pointToExit.transform.position, canion.transform.rotation);
            if(bulletInstantiate.TryGetComponent<Bullet>(out var bulletComponent))
            {
                var diff = point - (Vector2)canion.transform.position;
                bulletComponent.Configure(point, diff.normalized);
            }
            _rotating = false;
        }

        private float GetAngle()
        {
            var position = canion.transform.position;
            var diff = point - (Vector2)position;
            var angle = Vector2.Angle(diff,Vector2.up);
            var cross = Vector3.Cross(position, point);
            if (cross.z > 0)
            {
                angle *= -1;
            }
            return angle;
        }

        private void RotateTo(Vector2 toPoint)
        {
            _rotating = true;
            point = toPoint;
        }

        public void Fire(Vector2 vector2)
        {
            RotateTo(vector2);
        }
    }
}
