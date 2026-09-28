using System;
using System.Collections.Generic;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class DraftController : MonoBehaviour
    {
        public enum CardType
        {
            RadioMas,
            Recarga,
            Doble,
            Lenta,
            Propulsores
        }

        private static readonly Dictionary<CardType, string> CardCatalog = new Dictionary<CardType, string>
        {
            { CardType.RadioMas, "Radio+" },
            { CardType.Recarga, "Recarga" },
            { CardType.Doble, "Doble" },
            { CardType.Lenta, "Lenta" },
            { CardType.Propulsores, "Propulsores" }
        };

        private static readonly Dictionary<CardType, string> CardDescriptions = new Dictionary<CardType, string>
        {
            { CardType.RadioMas, "Nube de explosión +25%" },
            { CardType.Recarga, "Cooldown del cañón -20%" },
            { CardType.Doble, "2 misiles por disparo" },
            { CardType.Lenta, "Misiles enemigos -50% por 3s" },
            { CardType.Propulsores, "Interceptor +20% vel. y apuntado más rápido" }
        };

        [SerializeField] private BaseManager baseManager;
        [SerializeField] private MissilesEnemies missilesEnemies;
        [SerializeField] private InputController inputController;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject panelDraft;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button[] cardButtons = new Button[3];
        [SerializeField] private TextMeshProUGUI[] cardLabels = new TextMeshProUGUI[3];

        [Header("Mejoras de cartas (tunables)")]
        [Tooltip("Radio+: multiplicador de escala de explosión")]
        [SerializeField] private float radioMaxScaleMultiplier = 1.25f;
        [Tooltip("Recarga: multiplicador de cooldown del cañón")]
        [SerializeField] private float recargaCooldownMultiplier = 0.8f;
        [Tooltip("Doble: misiles por disparo")]
        [SerializeField] private int dobleBulletsPerShot = 2;
        [Tooltip("Lenta: factor de velocidad enemiga (0.5 = -50%)")]
        [SerializeField] private float lentaSpeedFactor = 0.5f;
        [Tooltip("Lenta: duración del enlentecimiento (s)")]
        [SerializeField] private float lentaDurationSeconds = 3f;
        [Tooltip("Propulsores: multiplicador de impulso del interceptor")]
        [SerializeField] private float propulsoresImpulseMultiplier = 1.2f;
        [Tooltip("Propulsores: multiplicador de velocidad de apuntado")]
        [SerializeField] private float propulsoresAimMultiplier = 1.2f;

        private readonly List<CardType> _currentCards = new List<CardType>(3);
        private bool _draftOpen;

        // El draft se abre entre sectores: RunSession emite SectorStarted al
        // completar la oleada activa (S1-S4). No hay trigger temporal.
        private void Start()
        {
            RunSession.Current.SectorStarted += ShowDraft;
        }

        private void ShowDraft()
        {
            if (_draftOpen) return;
            _draftOpen = true;
            DealCards();
            if (titleText != null)
            {
                titleText.text = "Elige una mejora";
            }
            for (var i = 0; i < cardButtons.Length; i++)
            {
                cardLabels[i].text = $"{CardCatalog[_currentCards[i]]}\n<size=55%>{CardDescriptions[_currentCards[i]]}</size>";
            }
            panelDraft.SetActive(true);
            Time.timeScale = 0f;
            inputController.SetUsage(false);
        }

        public void ChooseCard(int index)
        {
            if (!_draftOpen) return;
            ApplyEffect(_currentCards[index]);
            _draftOpen = false;
            panelDraft.SetActive(false);
            Time.timeScale = 1f;
            inputController.SetUsage(true);
            missilesEnemies.StartSector(RunSession.Current.Sector);
        }

        private void DealCards()
        {
            _currentCards.Clear();
            var pool = new List<CardType>(CardCatalog.Count);
            pool.AddRange(CardCatalog.Keys);
            while (_currentCards.Count < 3 && pool.Count > 0)
            {
                var pick = pool[UnityEngine.Random.Range(0, pool.Count)];
                pool.Remove(pick);
                _currentCards.Add(pick);
            }
        }

        private void ApplyEffect(CardType card)
        {
            switch (card)
            {
                case CardType.RadioMas:
                    bulletPrefab.GetComponentInChildren<Explosion>().MultiplyMaxScale(radioMaxScaleMultiplier);
                    break;
                case CardType.Recarga:
                    baseManager.ApplyToAll(b => b.MultiplyCooldown(recargaCooldownMultiplier));
                    break;
                case CardType.Doble:
                    baseManager.ApplyToAll(b => b.SetBulletsPerShot(dobleBulletsPerShot));
                    break;
                case CardType.Lenta:
                    missilesEnemies.SlowEnemies(lentaSpeedFactor, lentaDurationSeconds);
                    break;
                case CardType.Propulsores:
                    bulletPrefab.GetComponent<Bullet>().MultiplyImpulseForce(propulsoresImpulseMultiplier);
                    baseManager.ApplyToAll(b => b.MultiplyAimSpeed(propulsoresAimMultiplier));
                    break;
            }
        }
    }
}