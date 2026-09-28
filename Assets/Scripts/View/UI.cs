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
        private RunSession _runSession;

        private void Awake()
        {
            RunSession.Current = _runSession = new RunSession();
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
            points.text = $"{_runSession.Score}";
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