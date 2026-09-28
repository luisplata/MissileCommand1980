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
        [SerializeField] private float spawnIntervalSeconds = 1.5f;

        private bool _canCreateMissile;
        private bool _sectorActive;
        private int _spawned;
        private int _totalCount;
        private Coroutine _spawnCoroutine;
        private Coroutine _slowCoroutine;
        private readonly List<Bullet> _activeEnemies = new();

        private void Start()
        {
            _canCreateMissile = true;
            StartSector(RunSession.Current.Sector);
        }

        public void StartSector(int sector)
        {
            if (!_canCreateMissile) return;
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            _sectorActive = true;
            _spawned = 0;
            var config = SectorConfigs.All[sector - 1];
            _totalCount = config.TotalCount;
            Debug.Log($"Sector {config.Sector}: {config.TotalCount} misiles");
            _spawnCoroutine = StartCoroutine(SpawnSectorCoroutine(config));
        }

        // M1a-IT2 (dificultad): los misiles del sector se spawnean ESCALONADOS,
        // uno cada intervalo, en vez de todos juntos. El total del sector es el
        // MISMO (SectorConfigs.TotalCount); solo cambia la cadencia. El
        // intervalo aprieta levemente por sector: baseline spawnIntervalSeconds
        // - sector*0.1s (mín 0.8s) → S1=1.4s ... S5=1.0s.
        private IEnumerator SpawnSectorCoroutine(SectorConfig config)
        {
            var interval = Mathf.Max(spawnIntervalSeconds - config.Sector * 0.1f, 0.8f);
            while (_spawned < config.TotalCount)
            {
                SpawnOneMissile(config);
                _spawned++;
                if (_spawned < config.TotalCount)
                {
                    yield return new WaitForSeconds(interval);
                }
            }
        }

        private void SpawnOneMissile(SectorConfig config)
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
            // El sector completa solo cuando TODOS los misiles ya se spawnearon
            // Y no queda ninguno activo (no completar con spawns pendientes).
            if (_sectorActive && _spawned >= _totalCount && _activeEnemies.Count == 0)
            {
                _sectorActive = false;
                RunSession.Current.CompleteSector();
            }
        }

        public void StopCreatingMissile()
        {
            _canCreateMissile = false;
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        public void SlowEnemies(float factor, float duration)
        {
            if (_slowCoroutine != null)
            {
                StopCoroutine(_slowCoroutine);
            }
            _slowCoroutine = StartCoroutine(SlowEnemiesCoroutine(factor, duration));
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