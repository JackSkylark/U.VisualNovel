using Cysharp.Threading.Tasks;
using Dialogue.Abstractions;
using Dialogue.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Dialogue
{
    public abstract class DialogueCommands : MonoBehaviour
    {
        public abstract UniTask WriteLine(
            string text);

        public abstract UniTask<TIdentifier> PresentChoices<TIdentifier>(
            IEnumerable<Choice<TIdentifier>> choices);

        public abstract UniTask UpdateBackground();
        public abstract UniTask UpdatePortrait();
    }


    public class DialogueManager : DialogueCommands
    {
        [SerializeField]
        private DialogueWriter _dialogueWriter;

        [SerializeField]
        private ChoicePresenter _textChoicePresenter;

        [SerializeField]
        private ChoicePresenter _buttonChoicePresenter;

        public override UniTask<TIdentifier> PresentChoices<TIdentifier>(
            IEnumerable<Choice<TIdentifier>> choices)
        {
            return _textChoicePresenter.Present(
                choices, 
                CancellationToken.None);
        }

        public override UniTask UpdateBackground()
        {
            throw new System.NotImplementedException();
        }

        public override UniTask UpdatePortrait()
        {
            throw new System.NotImplementedException();
        }

        public override UniTask WriteLine(string text)
        {
            return _dialogueWriter.WriteLine(text);
        }

        private void Start()
        {
            RunSequenceWithEscape();
        }

        private async UniTask RunSequenceWithEscape()
        {
            var cancellationSource = new CancellationTokenSource();

            var exitRequestedTask =
                UniTask.WaitUntil(
                    () => Input.GetKeyDown(KeyCode.Escape),
                    PlayerLoopTiming.Update);

            var sequenceTask =
                RunSequence(cancellationSource.Token);

            var completedTaskIndex = 
                await UniTask.WhenAny(
                    exitRequestedTask, 
                    sequenceTask);

            if (completedTaskIndex == 0)
            {
                cancellationSource.Cancel();
            }
        }

        private async UniTask RunSequence(
            CancellationToken cancellationToken)
        {
            var shouldExit = false;
            var loopCount = 0;
            while (!shouldExit)
            {
                loopCount++;

                var selectedChoice =
                await _buttonChoicePresenter.Present(
                    new List<Choice<int>>
                    {
                        new()
                        {
                            Id = 0,
                            Text = loopCount <= 1 
                                ? "Talk"
                                : "Talk Again"
                        },
                        new()
                        {
                            Id = 1,
                            Text = "Walk Away"
                        }
                    },
                    cancellationToken);

                if (selectedChoice == 0)
                {
                    await WriteLines(loopCount, cancellationToken);
                }
                else
                {
                    await _dialogueWriter.WriteLine("Well, so long then", cancellationToken);
                    shouldExit = true;
                }
            }

            Application.Quit();
        }

        private async UniTask WriteLines(
            int visitCount,
            CancellationToken token)
        {
            var greeting =
                visitCount <= 1
                ? "Hi there! My name is Pobi. I am glad to meet you."
                : "Hello again!";

            await _dialogueWriter.WriteLine(greeting, token);

            await _dialogueWriter.WriteLine("This is a test of the dialogue system.", token);
            await _dialogueWriter.WriteLine("Are you listening?", token);

            var selectedChoice = 0;
            while (selectedChoice != 2)
            {
                selectedChoice = 
                    await _textChoicePresenter.Present(
                        new List<Choice<int>>
                        {
                            new Choice<int>
                            {
                                Id = 1,
                                Text = "Say What?!"
                            },
                            new Choice<int>
                            {
                                Id = 2,
                                Text = "The Dialogue System? What is that?"
                            }
                        },
                        token);

                if (selectedChoice == 1)
                {
                    await _dialogueWriter.WriteLine("EVERYBODY SAY WHAT!?!?!", token);
                }
            }

            await _dialogueWriter.WriteLine("I'm glad you asked. It's a system which handles the following domains:", token);
            await _dialogueWriter.WriteLine("1. Rendering Dialogue Text", token);
            await _dialogueWriter.WriteLine("2. Rendering and Handling Dialogue Decisions", token);

            await _dialogueWriter.WriteLine("We can do this like this:", token);

            await _textChoicePresenter.Present(
                new List<Choice<int>>
                {
                    new Choice<int>
                    {
                        Id = 0,
                        Text = "With responses writen in lines of text?"
                    },
                    new Choice<int>
                    {
                        Id = 1,
                        Text = "I have no idea what is going on."
                    },
                    new Choice<int>
                    {
                        Id = 2,
                        Text = "..."
                    }
                }, token);

            await _dialogueWriter.WriteLine("Or like this:", token);

            await _buttonChoicePresenter.Present(
                new List<Choice<int>>
                {
                    new Choice<int>
                    {
                        Id = 0,
                        Text = "Ooo Buttons!"
                    },
                    new Choice<int>
                    {
                        Id = 1,
                        Text = "I get it now"
                    }
                }, token);

            await _dialogueWriter.WriteLine("Thanks for listening to me. I'm always happy to chat.", token);
            await _dialogueWriter.WriteLine("Goodbye!", token);
        }
    }
}
