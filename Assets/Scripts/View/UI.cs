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
        [SerializeField] private TextMeshProUGUI ammoText;
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
            baseManager.AmmoChanged += UpdateAmmo;
            _runSession.CityDied += UpdateUi;
            UpdateUi();
            UpdateAmmo();
            points.text = $"{_runSession.Score}";
        }

        private void UpdateAmmo()
        {
            // Per-base ammo HUD: "A:10 D:10 O:10" (single line, space-separated).
            var parts = new string[baseManager.BaseCount];
            for (var i = 0; i < baseManager.BaseCount; i++)
            {
                parts[i] = $"{BaseManager.DisplayName(i)}:{baseManager.AmmoFor(i)}";
            }
            ammoText.text = string.Join(" ", parts);
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