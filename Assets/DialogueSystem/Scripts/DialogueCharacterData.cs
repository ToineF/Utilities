using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/Character")]
    public class DialogueCharacterData : ScriptableObject
    {
        public string Name;
        public Sprite Sprite;
        public Color DialogueBoxColor;
        public Color DialogueTextColor;
        public AudioClip[] SoundsTalk;
        public int TalkFrequency;
    }
}