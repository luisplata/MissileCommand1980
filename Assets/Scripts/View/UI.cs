using System;
using TMPro;
using UnityEngine;

namespace View
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Tank tank;
        [SerializeField] private TextMeshProUGUI points;
        private int pointsCount;
        private void Start()
        {
            tank.OnEnemyDestroy += OnEnemyDestroy;
        }

        private void OnEnemyDestroy()
        {
            pointsCount++;
            points.text = $"{pointsCount}";
        }
    }
}
