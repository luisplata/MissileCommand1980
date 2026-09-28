using System;
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

        [SerializeField] private float minTime, maxTime;
        [SerializeField] private float detaTimeLocal, time;
        [SerializeField] private List<string> cantidadDeMisiles;
        
        private bool _canCreateMissile;
        private readonly List<Bullet> _activeEnemies = new();

        private void Start()
        {
            time = Random.Range(minTime, maxTime);
            _canCreateMissile = true;
            Debug.Log($"Sector {RunSession.Current.Sector}");
        }

        public void CreateMissile()
        {
            foreach (var misile in cantidadDeMisiles)
            {
                var missileLocal = Instantiate(missile) as GameObject;
                var enemyMissile = missileLocal.GetComponent<Bullet>();
                var position = new Vector2(Random.Range(limitLeft.transform.position.x, limitRight.transform.position.x), limitLeft.transform.position.y);
                var target = PickCityTarget();
                Debug.Log($"position {position} target {target}");
                missileLocal.transform.position = position;
                enemyMissile.Configure(target, (target - position).normalized, missileLocal);
                _activeEnemies.Add(enemyMissile);
            }
        }

        private Vector2 PickCityTarget()
        {
            var houses = FindObjectsByType<HouseController>(FindObjectsSortMode.None);
            if (houses.Length > 0)
            {
                var house = houses[Random.Range(0, houses.Length)];
                return house.transform.position;
            }
            return new Vector2(Random.Range(targetLeft.transform.position.x, targetRight.transform.position.x), targetLeft.transform.position.y);
        }

        private void Update()
        {
            if (!_canCreateMissile) return;
            detaTimeLocal += Time.deltaTime;
            if (!(detaTimeLocal > time)) return;
            detaTimeLocal = 0;
            time = Random.Range(minTime, maxTime);
            CreateMissile();
        }

        public void StopCreatingMissile()
        {
            _canCreateMissile = false;
        }

        public void AddOneMoreMissile()
        {
            cantidadDeMisiles.Add("OtroMisile");
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