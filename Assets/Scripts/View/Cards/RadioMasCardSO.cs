using UnityEngine;

namespace View.Cards
{
    [CreateAssetMenu(menuName = "Cards/Radio+", fileName = "RadioMas")]
    public class RadioMasCardSO : CardSO
    {
        [Tooltip("Radio+: multiplicador de escala de explosión")]
        [SerializeField] private float radioMaxScaleMultiplier = 1.25f;

        public override string GetDescription()
        {
            return $"Nube de explosión +{(radioMaxScaleMultiplier - 1) * 100:0}%";
        }

        public override void ApplyEffect(CardContext ctx)
        {
            ctx.bulletPrefab.GetComponentInChildren<Explosion>().MultiplyMaxScale(radioMaxScaleMultiplier);
        }
    }
}