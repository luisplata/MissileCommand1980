using System;
using TMPro;
using UnityEngine;

namespace View
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private TankView tankView;
        [SerializeField] private TextMeshProUGUI points;
        private int pointsCount;
        private void Start()
        {
            tankView.OnEnemyDestroy += OnEnemyDestroy;
        }

        private void OnEnemyDestroy()
        {
            pointsCount++;
            points.text = $"{pointsCount}";
        }
    }
}
