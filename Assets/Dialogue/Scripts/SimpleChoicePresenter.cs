using Cysharp.Threading.Tasks;
using Dialogue.Abstractions;
using Dialogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Dialogue
{
    public class SimpleChoicePresenter : ChoicePresenter
    {
        [SerializeField]
        private GameObject _choicePrefab;

        private void Awake()
        {
            ClearChoices();
        }

        public override async UniTask<TIdentifier> Present<TIdentifier>(
            IEnumerable<Choice<TIdentifier>> choices,
            CancellationToken token)
        {
            ClearChoices();

            try
            {
                var choiceObservables = new List<IObservable<TIdentifier>>();
                foreach (var choice in choices)
                {
                    var choiceGameObject = Instantiate(_choicePrefab, transform);
                    var choiceButton = choiceGameObject.GetComponent<IChoice>();

                    choiceButton.SetText(choice.Text);

                    var obs = choiceButton.OnSelect.Select(x => choice.Id);
                    choiceObservables.Add(obs);
                }

                return await Observable
                    .Merge(choiceObservables)
                    .ToUniTask(true, token);
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            finally
            {
                ClearChoices();
            }
        }

        private void ClearChoices()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}


