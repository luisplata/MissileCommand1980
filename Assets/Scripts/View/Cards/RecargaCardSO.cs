using UnityEngine;

namespace View.Cards
{
    [CreateAssetMenu(menuName = "Cards/Recarga", fileName = "Recarga")]
    public class RecargaCardSO : CardSO
    {
        [Tooltip("Recarga: multiplicador de cooldown del cañón")]
        [SerializeField] private float recargaCooldownMultiplier = 0.8f;

        public override string GetDescription()
        {
            return $"Cooldown del cañón -{(1 - recargaCooldownMultiplier) * 100:0}%";
        }

        public override void ApplyEffect(CardContext ctx)
        {
            ctx.baseManager.ApplyToAll(b => b.MultiplyCooldown(recargaCooldownMultiplier));
        }
    }
}