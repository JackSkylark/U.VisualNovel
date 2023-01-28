using System;
using UniRx;

namespace Dialogue.Abstractions
{
    public interface IChoice
    {
        IObservable<Unit> OnSelect { get; }
        public void SetText(string text);
    }
}


