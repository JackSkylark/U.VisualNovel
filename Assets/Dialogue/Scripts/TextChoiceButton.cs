using Dialogue.Abstractions;
using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dialogue
{
    public class TextChoiceButton : MonoBehaviour, IChoice, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private Color _defaultColor;

        [SerializeField]
        private Color _hoverColor;

        private bool _isActive;

        private Subject<Unit> _onSelect = new();
        public IObservable<Unit> OnSelect => _onSelect.AsObservable();

        public void SetIsActive(
            bool isActive)
        {
            _isActive = isActive;
        }

        private void Update()
        {
            _text.color = _isActive ? _hoverColor : _defaultColor;
            _text.fontStyle = _isActive ? FontStyles.Bold : FontStyles.Normal;
        }

        public void OnPointerEnter(
            PointerEventData eventData)
        {
            SetIsActive(true);
        }

        public void OnPointerExit(
            PointerEventData eventData)
        {
            SetIsActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onSelect.OnNext(Unit.Default);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}


