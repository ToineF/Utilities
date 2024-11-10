using System;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public DialogueLine[] Lines => _lines;
        public DialogueCharacterData CharacterData => characterData;

        [SerializeField] private DialogueLine[] _lines;
        [SerializeField] private DialogueCharacterData characterData;
    }

    [Serializable]
    public struct DialogueLine
    {
        [TextArea]
        public string Text;
        public Sprite SpriteOverride;
    }
}