using System;
using UnityEngine;


namespace View
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private GameObject light;
        [SerializeField] private float explosionDuration;
        [SerializeField] private float explosionIncreasing;
        [SerializeField] private float explosionIncreasingLight;
        [SerializeField] private float maxScale;
        [SerializeField] private float fadeDelay;
        [SerializeField] private float fadeDuration;

        private float _deltaTimeLocal;
        private bool _startCount;
        private GameObject _originI;
        private UnityEngine.Rendering.Universal.Light2D light2D;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;

        public TankView.OnPlayerDestroyEnemy OnEnemyDestroy;
        
        private void Start()
        {
            _spriteRenderer = light.GetComponent<SpriteRenderer>();
            _originalColor = _spriteRenderer.color;
            light.transform.localScale = Vector3.zero;
            light2D = light.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            if (light2D.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Point)
            {
                light2D.pointLightOuterRadius = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_startCount) return;
            var nextScale = light.transform.localScale.x + (explosionIncreasing * Time.deltaTime);
            if (maxScale > 0 && nextScale > maxScale) nextScale = maxScale;
            light.transform.localScale = Vector3.one * nextScale;
            var nextRadius = light2D.pointLightOuterRadius + (explosionIncreasingLight * Time.deltaTime);
            if (maxScale > 0 && nextRadius > maxScale) nextRadius = maxScale;
            light2D.pointLightOuterRadius = nextRadius;
            ApplyPhaseFade();
            _deltaTimeLocal += Time.deltaTime;
            if (_deltaTimeLocal > explosionDuration)
            {
                Destroy(_originI);
                Destroy(gameObject.transform.parent.transform.parent.gameObject);
            }
        }

        private void ApplyPhaseFade()
        {
            if (fadeDuration <= 0 || _deltaTimeLocal < fadeDelay) return;
            var t = Mathf.InverseLerp(fadeDelay, fadeDelay + fadeDuration, _deltaTimeLocal);
            _spriteRenderer.color = Color.Lerp(_originalColor, new Color(0.5f, 0.5f, 0.5f, 0f), t);
            light2D.intensity = Mathf.Lerp(1f, 0f, t);
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
