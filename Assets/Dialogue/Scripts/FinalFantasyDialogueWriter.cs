using System;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.Linq;
using Dialogue.Abstractions;
using Dialogue.Constants;
using System.Threading;

namespace Dialogue
{
    public class FinalFantasyDialogueWriter : DialogueWriter
    {
        private const string TextGameObjectName = "ui.dialogue.text.message";
        private const string ContinueArrowGameObjectName = "ui.dialogue.text.continue";

        [SerializeField]
        private GameObject _uiPrefab;

        [SerializeField]
        private PrintSettings _settings;

        private GameObject _container;
        private TextMeshProUGUI _text;
        private GameObject _continueIcon;

        [SerializeField]
        private AudioSource _voiceSource;

        [SerializeField]
        private AudioClip[] _voices;

        [SerializeField]
        private AudioClip _punctuation;

        private void Awake()
        {
            InitializeDialogueWriter();
            ResetDialogueWriter();
        }

        private void InitializeDialogueWriter()
        {
            if (_container != null)
            {
                Destroy(_container);
            }

            _container = Instantiate(_uiPrefab, transform);
            _text = _container.GetComponentInChildren<TextMeshProUGUI>();

            var textGo = _container.transform.Find(TextGameObjectName);
            _text = textGo.GetComponent<TextMeshProUGUI>();

            _continueIcon = _container.transform.Find(ContinueArrowGameObjectName).gameObject;
        }

        private void ResetDialogueWriter()
        {
            _text.text = string.Empty;
            _continueIcon.SetActive(false);
            _container.SetActive(false);
        }

        public override async UniTask WriteLine(
            string text,
            CancellationToken cancellationToken = default)
        {
            _container.SetActive(true);

            try
            {
                await UniTask.WhenAny(
                WaitUntilNextButtonIsPressed(cancellationToken),
                TypeTextCharByChar(text, cancellationToken));

                _text.maxVisibleCharacters = text.Length;
                _continueIcon.SetActive(true);

                await WaitUntilNextButtonIsPressed(cancellationToken);
            }
            catch (Exception ex) 
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                ResetDialogueWriter();
            }            
        }

        private async UniTask WaitUntilNextButtonIsPressed(
            CancellationToken token)
        {
            await UniTask.WaitUntil(
                () => Input.GetKeyDown(KeyCode.Return),
                PlayerLoopTiming.Update,
                token);
        }

        private async UniTask TypeTextCharByChar(
            string line,
            CancellationToken token)
        {
            _text.text = line;
            _text.maxVisibleCharacters = 0;

            bool isAddingRichText = false;
            foreach (var character in line.ToCharArray())
            {
                if (token.IsCancellationRequested) 
                { 
                    return; 
                }

                if (character == '<' || isAddingRichText)
                {
                    isAddingRichText = true;
                    if (character == '>')
                    {
                        isAddingRichText = false;
                    }
                }
                else
                {
                    _voiceSource.Stop();
                    _voiceSource.clip = _voices[UnityEngine.Random.Range(0, _voices.Length)];
                    _voiceSource.Play();


                    // play sound
                    _text.maxVisibleCharacters++;

                    var delay = GetCharacterPrintDelaySeconds(character);
                    await UniTask.Delay(delay, cancellationToken: token);
                }
            }
        }

        private TimeSpan GetCharacterPrintDelaySeconds(
            char character)
        {
            var delay = _settings.CharPrintDelaySeconds;

            if (DialogueConstants.PunctuationCharacters.Contains(character))
            {
                delay *= _settings.PunctuationPrintDelayModifier;
            }

            return TimeSpan.FromSeconds(delay);
        }


        // TODO: make this configurable for each line
        [Serializable]
        public class PrintSettings
        {
            [SerializeField]
            [Range(0.01f, 0.2f)]
            private float _charPrintDelaySeconds = 0.04f;

            [SerializeField]
            private float _punctuationPrintDelayModifier = 1f;

            public float CharPrintDelaySeconds => _charPrintDelaySeconds;
            public float PunctuationPrintDelayModifier => _punctuationPrintDelayModifier;
        }
    }
}
