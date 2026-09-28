using System;
using System.Collections;
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
        [SerializeField] private int bulletsPerShot = 1;
        private Tank _tank;
        private SpriteRenderer _spriteRenderer;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _tank = new Tank(this, canion.transform.position, cooldown, min);
            _canUseTank = true;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public delegate void OnPlayerDestroyEnemy(int points);
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

        public void FireBullet()
        {
            //Aqui convertir en una factoria
            //Ejecutar animacion de disparo para luego instanciar la bala
            for (var i = 0; i < bulletsPerShot; i++)
            {
                var bulletInstantiate = Instantiate(bullet, pointToExit.transform.position, canion.transform.rotation);
                if (bulletInstantiate.TryGetComponent<Bullet>(out var bulletComponent))
                {
                    //Aqui convertir en un builder
                    bulletComponent.Configure(_tank.GetGoal(), _tank.GetDirectionNormalize(), pointToExit);
                    bulletComponent.OnEnemyDestroy += points =>
                    {
                        OnEnemyDestroy?.Invoke(points);
                    };
                }
            }

            _tank.StopRotating();
        }

        public void SetBulletsPerShot(int count)
        {
            bulletsPerShot = count;
        }

        public void MultiplyCooldown(float factor)
        {
            _tank.CooldownMultiplier *= factor;
        }

        public void MultiplyAimSpeed(float factor)
        {
            angleMore *= factor;
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

        // Reload feedback (bases-arrival-fix): BaseManager.RefillAll (on SectorStarted) calls
        // this on every base so the player SEES that ammo was reloaded. Flashes the tank
        // sprite 3 times over ~0.6s. Code-only — the Tank prefab root already has a
        // SpriteRenderer; no new assets, no prefab edits.
        public void FlashReload()
        {
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            if (_spriteRenderer == null) yield break;
            var original = _spriteRenderer.color;
            var flash = new Color(1f, 0.85f, 0.35f); // warm amber tint, visible over the white sprite
            for (var i = 0; i < 3; i++)
            {
                _spriteRenderer.color = flash;
                yield return new WaitForSeconds(0.1f);
                _spriteRenderer.color = original;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
