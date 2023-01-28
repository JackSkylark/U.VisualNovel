using Cysharp.Threading.Tasks;
using Dialogue.Models;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Dialogue.Abstractions
{
    public abstract class ChoicePresenter : MonoBehaviour
    {
        public abstract UniTask<TIdentifier> Present<TIdentifier>(
            IEnumerable<Choice<TIdentifier>> choices,
            CancellationToken token);
    }
}
