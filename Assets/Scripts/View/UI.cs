using System;
using TMPro;
using UnityEngine;

namespace View
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private TankView tankView;
        [SerializeField] private TextMeshProUGUI points, lifeUi;
        [SerializeField] private float life;
        [SerializeField] private float cadaCuantoAumentanLosMisiles;
        [SerializeField] private GameObject panelGameOver;
        public OnGameOver GameOver;
        public OnAumentoDeMissiles AumentoDeMissiles;
        private int pointsCount;
        public delegate void OnGameOver();
        public delegate void OnAumentoDeMissiles();

        private void Start()
        {
            tankView.OnEnemyDestroy += OnEnemyDestroy;
            UpdateUi();
            points.text = $"{pointsCount}";
        }

        private void OnEnemyDestroy()
        {
            pointsCount++;
            if (pointsCount % cadaCuantoAumentanLosMisiles == 0)
            {
                AumentoDeMissiles?.Invoke();   
            }
            points.text = $"{pointsCount}";
        }

        public void ApllyDamange(float damage)
        {
            if (life <= 0) return;
            life -= damage;
            UpdateUi();
        }

        private void UpdateUi()
        {
            lifeUi.text = $"{life}";
            if (life <= 0)
            {
                GameOver?.Invoke();
            }
        }

        public void ShowGameOverAndOptions()
        {
            panelGameOver.SetActive(true);
        }
    }
}
