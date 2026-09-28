using UnityEngine;

namespace View.Cards
{
    public abstract class CardSO : ScriptableObject
    {
        [SerializeField] private string cardName;
        public string CardName => cardName;

        public abstract string GetDescription();

        public abstract void ApplyEffect(CardContext ctx);
    }
}