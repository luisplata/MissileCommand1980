using UnityEngine;

namespace View.Cards
{
    [CreateAssetMenu(menuName = "Cards/Doble", fileName = "Doble")]
    public class DobleCardSO : CardSO
    {
        [Tooltip("Doble: misiles por disparo")]
        [SerializeField] private int dobleBulletsPerShot = 2;

        public override string GetDescription()
        {
            return $"Doble: {dobleBulletsPerShot} misiles por disparo";
        }

        public override void ApplyEffect(CardContext ctx)
        {
            ctx.baseManager.ApplyToAll(b => b.SetBulletsPerShot(dobleBulletsPerShot));
        }
    }
}