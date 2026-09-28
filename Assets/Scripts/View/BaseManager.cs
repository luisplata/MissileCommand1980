using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace View
{
    public class BaseManager : MonoBehaviour
    {
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private List<Transform> basePositions = new List<Transform>();
        [SerializeField] private int ammoPerBase = 10;
        [SerializeField] private float shotCooldownSeconds = 0.5f; // mirrors TankView.cooldown prefab value

        private readonly List<TankView> _bases = new List<TankView>();
        private int[] _ammo;
        private float[] _lastShotTime;

        public event Action<int> OnEnemyDestroy;      // BM-6 aggregated
        public event Action AmmoChanged;              // HUD hook (D4)

        public int TotalAmmo
        {
            get
            {
                var t = 0;
                foreach (var a in _ammo) t += a;
                return t;
            }
        }

        private void Awake()
        {
            foreach (var m in basePositions)
            {
                // BM-1: skip null slots and inactive markers (deactivating a marker removes a base)
                if (m == null || !m.gameObject.activeInHierarchy) continue;
                var go = Instantiate(tankPrefab, m.position, Quaternion.identity);
                go.transform.SetParent(transform, true); // worldPositionStays ONLY (R7/R8)
                var v = go.GetComponent<TankView>();
                v.OnEnemyDestroy += p => OnEnemyDestroy?.Invoke(p);
                _bases.Add(v);
            }
            _ammo = new int[_bases.Count];
            _lastShotTime = new float[_bases.Count];
            for (var i = 0; i < _bases.Count; i++)
            {
                _ammo[i] = ammoPerBase;
                _lastShotTime[i] = float.NegativeInfinity;
            }
        }

        private void Start()
        {
            // BM-3: non-null by any Start (UI.Awake creates RunSession.Current first)
            RunSession.Current.SectorStarted += RefillAll;
        }

        public void TryFireClosestTo(Vector2 worldPoint)
        {
            // BM-4/D3: nearest base with ammo>0 AND gate elapsed. Per-shot gate (NOT per-forward
            // decrement): InputController fires every frame while held; per-forward would drain 10
            // ammo in ~0.17s. Accepted edge: Recarga (×0.8 internal cooldown) won't accelerate the
            // hold cadence because this gate is a fixed 0.5s mirror — documented per design D3.
            var best = -1;
            var bestD = float.MaxValue;
            for (var i = 0; i < _bases.Count; i++)
            {
                if (_ammo[i] <= 0) continue; // "sin dardos = mirar"
                if (Time.time - _lastShotTime[i] < shotCooldownSeconds) continue; // D3 gate
                var d = (_bases[i].transform.position - (Vector3)worldPoint).sqrMagnitude;
                if (d < bestD)
                {
                    bestD = d;
                    best = i;
                }
            }
            if (best < 0) return;
            _ammo[best]--;
            _lastShotTime[best] = Time.time;
            AmmoChanged?.Invoke(); // BM-2 decrement before Fire
            _bases[best].Fire(worldPoint);
        }

        public void RefillAll()
        {
            for (var i = 0; i < _ammo.Length; i++)
            {
                _ammo[i] = ammoPerBase;
                _lastShotTime[i] = float.NegativeInfinity;
            }
            AmmoChanged?.Invoke();
        }

        public void ApplyToAll(Action<TankView> a)
        {
            foreach (var b in _bases) a(b);
        }

        public void StopAll() => ApplyToAll(b => b.StopAllMovements());
    }
}