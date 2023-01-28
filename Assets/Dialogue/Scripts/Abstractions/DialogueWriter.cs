using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Dialogue.Abstractions
{
    public abstract class DialogueWriter : MonoBehaviour
    {
        public abstract UniTask WriteLine(
            string text,
            CancellationToken cancellationToken = default);
    }
}
