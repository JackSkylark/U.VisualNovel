using Cysharp.Threading.Tasks;
using Dialogue;
using Dialogue.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class InkStoryRunner : MonoBehaviour
    {
        [SerializeField] 
        DialogueCommands _dialogueManager;

        private async UniTask WriteLines()
        {
            await _dialogueManager.WriteLine("This is a test. Do not worry!");
            await _dialogueManager.WriteLine("This is only a test.");
            await _dialogueManager.WriteLine("Are you listening?");
        }
    }
}
