using UnityEngine;

namespace View.Cards
{
    [CreateAssetMenu(menuName = "Cards/Lenta", fileName = "Lenta")]
    public class LentaCardSO : CardSO
    {
        [Tooltip("Lenta: factor de velocidad enemiga (0.5 = -50%)")]
        [SerializeField] private float lentaSpeedFactor = 0.5f;
        [Tooltip("Lenta: duración del enlentecimiento (s)")]
        [SerializeField] private float lentaDurationSeconds = 3f;

        public override string GetDescription()
        {
            return $"Misiles enemigos -{lentaSpeedFactor * 100:0}% por {lentaDurationSeconds:0}s";
        }

        public override void ApplyEffect(CardContext ctx)
        {
            ctx.missilesEnemies.SlowEnemies(lentaSpeedFactor, lentaDurationSeconds);
        }
    }
}