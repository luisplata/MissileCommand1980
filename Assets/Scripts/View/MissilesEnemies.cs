using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace View
{
    public class MissilesEnemies : MonoBehaviour
    {
        [SerializeField] private GameObject limitLeft, limitRight;
        [SerializeField] private GameObject targetLeft, targetRight;
        [SerializeField] private GameObject missile;

        private bool _canCreateMissile;
        private bool _sectorActive;
        private readonly List<Bullet> _activeEnemies = new();

        private void Start()
        {
            _canCreateMissile = true;
            StartSector(RunSession.Current.Sector);
        }

        public void StartSector(int sector)
        {
            if (!_canCreateMissile) return;
            _sectorActive = true;
            var config = SectorConfigs.All[sector - 1];
            Debug.Log($"Sector {config.Sector}: {config.TotalCount} misiles");
            for (var i = 0; i < config.TotalCount; i++)
            {
                var missileLocal = Instantiate(missile) as GameObject;
                var enemyMissile = missileLocal.GetComponent<Bullet>();
                var position = new Vector2(Random.Range(limitLeft.transform.position.x, limitRight.transform.position.x), limitLeft.transform.position.y);
                var target = PickCityTarget();
                missileLocal.transform.position = position;
                enemyMissile.MultiplyImpulseForce(config.SpeedMultiplier);
                enemyMissile.Configure(target, (target - position).normalized, missileLocal);
                _activeEnemies.Add(enemyMissile);
            }
        }

        private Vector2 PickCityTarget()
        {
            var houses = FindObjectsByType<HouseController>(FindObjectsSortMode.None);
            if (houses.Length > 0)
            {
                var alive = new List<HouseController>(houses.Length);
                foreach (var house in houses)
                {
                    if (RunSession.Current.CityAlive(house.CityId))
                    {
                        alive.Add(house);
                    }
                }
                if (alive.Count > 0)
                {
                    return alive[Random.Range(0, alive.Count)].transform.position;
                }
            }
            return new Vector2(Random.Range(targetLeft.transform.position.x, targetRight.transform.position.x), targetLeft.transform.position.y);
        }

        private void Update()
        {
            _activeEnemies.RemoveAll(m => m == null);
            if (_sectorActive && _activeEnemies.Count == 0)
            {
                _sectorActive = false;
                RunSession.Current.CompleteSector();
            }
        }

        public void StopCreatingMissile()
        {
            _canCreateMissile = false;
        }

        public void SlowEnemies(float factor, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(SlowEnemiesCoroutine(factor, duration));
        }

        private IEnumerator SlowEnemiesCoroutine(float factor, float duration)
        {
            _activeEnemies.RemoveAll(m => m == null);
            var original = new Dictionary<Bullet, Vector2>();
            foreach (var enemy in _activeEnemies)
            {
                var rigidbody = enemy.GetComponent<Rigidbody2D>();
                original[enemy] = rigidbody.linearVelocity;
                rigidbody.linearVelocity *= factor;
            }

            yield return new WaitForSecondsRealtime(duration);

            foreach (var pair in original)
            {
                if (pair.Key == null) continue;
                var rigidbody = pair.Key.GetComponent<Rigidbody2D>();
                if (rigidbody != null)
                {
                    rigidbody.linearVelocity = pair.Value;
                }
            }
        }
    }
}