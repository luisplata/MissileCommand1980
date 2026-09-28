using System;
using UnityEngine;


namespace View
{
    public class Explosion : MonoBehaviour
    {
        [Header("Referencia")]
        [SerializeField] private GameObject light;

        [Header("Fases (tiempos)")]
        [SerializeField] private float explosionDuration;
        [SerializeField] private float fadeDelay;
        [SerializeField] private float fadeDuration;

        [Header("Nube (escala)")]
        [SerializeField] private float explosionIncreasing;
        [SerializeField] private float maxScale;
        [SerializeField] private float startScale;

        [Header("Luz")]
        [SerializeField] private float explosionIncreasingLight;
        [SerializeField] private float lightStartRadius;
        [SerializeField] private float maxLightRadius;
        [SerializeField] private float lightStartIntensity;
        [SerializeField] private float lightEndIntensity;

        [Header("Color")]
        [SerializeField] private Color burstColor = Color.white;
        [SerializeField] private Color cloudColor = new Color(0.5f, 0.5f, 0.5f, 0f);

        private float _deltaTimeLocal;
        private bool _startCount;
        private GameObject _originI;
        private UnityEngine.Rendering.Universal.Light2D light2D;
        private SpriteRenderer _spriteRenderer;
        private float _ancestorScale = 1f;

        public TankView.OnPlayerDestroyEnemy OnEnemyDestroy;
        
        private void Start()
        {
            _spriteRenderer = light.GetComponent<SpriteRenderer>();
            _spriteRenderer.color = burstColor;
            _ancestorScale = transform.parent != null && transform.parent.lossyScale.x > 0f
                ? transform.parent.lossyScale.x
                : 1f;
            light.transform.localScale = Vector3.one * (startScale / _ancestorScale);
            light2D = light.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            if (light2D.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Point)
            {
                light2D.pointLightOuterRadius = lightStartRadius;
            }
            light2D.intensity = lightStartIntensity;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_startCount) return;
            var worldScale = (light.transform.localScale.x * _ancestorScale) + (explosionIncreasing * Time.deltaTime);
            if (maxScale > 0 && worldScale > maxScale) worldScale = maxScale;
            light.transform.localScale = Vector3.one * (worldScale / _ancestorScale);
            var nextRadius = light2D.pointLightOuterRadius + (explosionIncreasingLight * Time.deltaTime);
            var radiusCap = maxLightRadius > 0 ? maxLightRadius : maxScale;
            if (radiusCap > 0 && nextRadius > radiusCap) nextRadius = radiusCap;
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
            _spriteRenderer.color = Color.Lerp(burstColor, cloudColor, t);
            light2D.intensity = Mathf.Lerp(lightStartIntensity, lightEndIntensity, t);
        }

        public void Configuration(GameObject originI)
        {
            _startCount = true;
            _originI = originI;
        }

        public void MultiplyMaxScale(float factor)
        {
            maxScale *= factor;
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
