using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private BaseManager baseManager;
        [SerializeField] private TextMeshProUGUI points, lifeUi;
        [SerializeField] private GameObject panelGameOver;
        [SerializeField] private TextMeshProUGUI gameOverTitle;
        private RunSession _runSession;

        private void Awake()
        {
            RunSession.Current = _runSession = new RunSession();
        }

        private void Start()
        {
            baseManager.OnEnemyDestroy += OnEnemyDestroy;
            _runSession.CityDied += UpdateUi;
            UpdateUi();
            points.text = $"{_runSession.Score}";
        }

        private void OnEnemyDestroy(int gainedPoints)
        {
            _runSession.AddScore(gainedPoints);
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

        public void ShowVictoryAndOptions()
        {
            if (gameOverTitle != null)
            {
                gameOverTitle.text = "¡Victoria!";
            }
            panelGameOver.SetActive(true);
        }
    }
}