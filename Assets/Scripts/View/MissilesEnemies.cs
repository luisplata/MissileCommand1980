using System;
using System.Collections.Generic;
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

        private void Start()
        {
            time = Random.Range(minTime, maxTime);
            _canCreateMissile = true;
        }

        public void CreateMissile()
        {
            foreach (var misile in cantidadDeMisiles)
            {
                var missileLocal = Instantiate(missile) as GameObject;
                var enemyMissile = missileLocal.GetComponent<Bullet>();
                var position = new Vector2(Random.Range(limitLeft.transform.position.x, limitRight.transform.position.x), limitLeft.transform.position.y);
                var target = new Vector2(Random.Range(targetLeft.transform.position.x, targetRight.transform.position.x), targetLeft.transform.position.y);
                Debug.Log($"position {position} target {target}");
                missileLocal.transform.position = position;
                enemyMissile.Configure(target, (target - (Vector2)transform.position).normalized, missileLocal);
            }
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
    }
}