using System;
using UnityEngine;

namespace View
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private GameObject light;
        [SerializeField] private float explosionDuration;
        [SerializeField] private float explosionIncreasing;

        private float _deltaTimeLocal;
        private bool _startCount;
        private GameObject _originI;

        public Tank.OnPlayerDestroyEnemy OnEnemyDestroy;
        
        private void Start()
        {
            light.transform.localScale = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_startCount) return;
            light.transform.localScale += Vector3.one * (explosionIncreasing * Time.deltaTime);
            _deltaTimeLocal += Time.deltaTime;
            if (_deltaTimeLocal > explosionDuration)
            {
                Destroy(_originI);
                Destroy(gameObject.transform.parent.transform.parent.gameObject);
            }
        }

        public void Configuration(GameObject originI)
        {
            _startCount = true;
            _originI = originI;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Respawn"))
            {
                OnEnemyDestroy?.Invoke();
            }
        }
    }
}
