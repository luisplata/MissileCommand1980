using UnityEngine;

namespace View.Cards
{
    [CreateAssetMenu(menuName = "Cards/Propulsores", fileName = "Propulsores")]
    public class PropulsoresCardSO : CardSO
    {
        [Tooltip("Propulsores: multiplicador de impulso del interceptor")]
        [SerializeField] private float propulsoresImpulseMultiplier = 1.2f;
        [Tooltip("Propulsores: multiplicador de velocidad de apuntado")]
        [SerializeField] private float propulsoresAimMultiplier = 1.2f;

        public override string GetDescription()
        {
            return $"Interceptor +{(propulsoresImpulseMultiplier - 1) * 100:0}% vel. y apuntado más rápido";
        }

        public override void ApplyEffect(CardContext ctx)
        {
            ctx.bulletPrefab.GetComponent<Bullet>().MultiplyImpulseForce(propulsoresImpulseMultiplier);
            ctx.baseManager.ApplyToAll(b => b.MultiplyAimSpeed(propulsoresAimMultiplier));
        }
    }
}