using System.Collections.Generic;

namespace Dialogue.Constants
{ 
    public static class DialogueConstants
    {
        public static readonly IReadOnlyCollection<char> PunctuationCharacters =
            new List<char>
            {
                '.',
                ',',
                '!',
                '?'
            };
    }
}
