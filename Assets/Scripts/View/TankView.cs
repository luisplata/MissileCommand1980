using System;
using Model;
using UnityEngine;

namespace View
{
    public class TankView : MonoBehaviour, ITankView
    {
        [SerializeField] private GameObject canion;
        [SerializeField] private GameObject pointToExit;
        [SerializeField] private float angleMore;
        [SerializeField] private float min;
        [SerializeField] private GameObject bullet;
        [SerializeField] private DebuggerCustom debuggerCustom;
        [SerializeField] private float cooldown;
        [SerializeField] private Animator _animator;
        private Tank _tank;

        private void Awake()
        {
            _tank = new Tank(this, canion.transform.position, cooldown, min);
            _canUseTank = true;
        }

        public delegate void OnPlayerDestroyEnemy();
        public OnPlayerDestroyEnemy OnEnemyDestroy;
        private bool _canUseTank;

        // Update is called once per frame
        void Update()
        {
            if (!_canUseTank) return;
            _tank.AddDelta(Time.deltaTime);
            if (_tank.CanRotate())
            {
                Rotate();
            }
        }

        private void Rotate()
        {
            var angle = _tank.GetAngle();
            var rotation = canion.transform.rotation;
            var diff = Quaternion.Slerp(rotation, Quaternion.Euler(0, 0, angle), angleMore * Time.deltaTime);
            var diffRotationZ = rotation.z - Quaternion.Euler(0, 0, angle).z;
            if (_tank.CanShoot(Mathf.Abs(diffRotationZ)))
            {
                //FireBullet();//para futuro refactor
                _animator.SetTrigger("fire");
            }
            canion.transform.rotation = diff;
        }

        private void FireBullet()
        {
            //Aqui convertir en una factoria
            //Ejecutar animacion de disparo para luego instanciar la bala
            var bulletInstantiate = Instantiate(bullet, pointToExit.transform.position, canion.transform.rotation);
            if(bulletInstantiate.TryGetComponent<Bullet>(out var bulletComponent))
            {
                //Aqui convertir en un builder
                bulletComponent.Configure(_tank.GetGoal(), _tank.GetDirectionNormalize(), pointToExit);
                bulletComponent.OnEnemyDestroy += () =>
                {
                    OnEnemyDestroy?.Invoke();
                };
            }

            _tank.StopRotating();
        }
        
        public void Fire(Vector2 vector2)
        {
            _tank.Rotate(vector2);
        }

        public float GetAngleUpFrom(Vector2 diff)
        {
            return Vector2.Angle(diff, Vector2.up);
        }

        public bool IsLeft(Vector2 position, Vector2 point)
        {
            return Vector3.Cross(position, point).z > 0;
        }

        public void StopAllMovements()
        {
            _canUseTank = false;
        }
    }
}
