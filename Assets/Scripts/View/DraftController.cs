using System.Collections.Generic;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Cards;

namespace View
{
    public class DraftController : MonoBehaviour
    {
        [SerializeField] private BaseManager baseManager;
        [SerializeField] private MissilesEnemies missilesEnemies;
        [SerializeField] private InputController inputController;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject panelDraft;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button[] cardButtons = new Button[3];
        [SerializeField] private TextMeshProUGUI[] cardLabels = new TextMeshProUGUI[3];
        [SerializeField] private List<CardSO> pool;

        private readonly List<CardSO> _currentCards = new List<CardSO>(3);
        private readonly CardContext _ctx = new CardContext();
        private bool _draftOpen;

        // El draft se abre entre sectores: RunSession emite SectorStarted al
        // completar la oleada activa (S1-S4). No hay trigger temporal.
        private void Start()
        {
            RunSession.Current.SectorStarted += ShowDraft;
            _ctx.baseManager = baseManager;
            _ctx.missilesEnemies = missilesEnemies;
            _ctx.bulletPrefab = bulletPrefab;
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
                cardLabels[i].text = $"{_currentCards[i].CardName}\n<size=55%>{_currentCards[i].GetDescription()}</size>";
            }
            panelDraft.SetActive(true);
            Time.timeScale = 0f;
            inputController.SetUsage(false);
        }

        public void ChooseCard(int index)
        {
            if (!_draftOpen) return;
            _currentCards[index].ApplyEffect(_ctx);
            _draftOpen = false;
            panelDraft.SetActive(false);
            Time.timeScale = 1f;
            inputController.SetUsage(true);
            missilesEnemies.StartSector(RunSession.Current.Sector);
        }

        private void DealCards()
        {
            _currentCards.Clear();
            var temp = new List<CardSO>(pool);
            while (_currentCards.Count < 3 && temp.Count > 0)
            {
                var pick = temp[UnityEngine.Random.Range(0, temp.Count)];
                temp.Remove(pick);
                _currentCards.Add(pick);
            }
        }
    }
}