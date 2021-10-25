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

        private void Start()
        {
            light.transform.localScale = Vector3.zero;
        }

        public void Configuration()
        {
            _startCount = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_startCount) return;
            light.transform.localScale += Vector3.one * (explosionIncreasing * Time.deltaTime);
            _deltaTimeLocal += Time.deltaTime;
            if (_deltaTimeLocal > explosionDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}
