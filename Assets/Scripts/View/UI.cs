using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private TankView tankView;
        [SerializeField] private TextMeshProUGUI points, lifeUi;
        [SerializeField] private GameObject panelGameOver;
        [SerializeField] private DraftController draft;
        public OnAumentoDeMissiles AumentoDeMissiles;
        public delegate void OnAumentoDeMissiles();
        private RunSession _runSession;

        private void Awake()
        {
            RunSession.Current = _runSession = new RunSession(1000f, 10);
        }

        private void Start()
        {
            tankView.OnEnemyDestroy += OnEnemyDestroy;
            UpdateUi();
            points.text = $"{_runSession.Score}";
        }

        private void OnEnemyDestroy()
        {
            _runSession.AddScore();
            _runSession.AddKill();
            if (_runSession.ShouldAddMissile())
            {
                AumentoDeMissiles?.Invoke();
            }
            points.text = $"{_runSession.Score}";
            draft?.OnEnemyKilled();
        }

        public void ApllyDamange(float damage)
        {
            _runSession.ApplyDamage(damage);
            UpdateUi();
        }

        private void UpdateUi()
        {
            lifeUi.text = $"{_runSession.Life}";
        }

        public void ShowGameOverAndOptions()
        {
            panelGameOver.SetActive(true);
        }
    }
}