using System;
using System.Collections;

namespace DialogueSystem
{
    [Serializable]
    public class DialogueEffect
    {
        public string Type { get; private set; }
        public string Value { get; private set; }
        public int Index { get; private set; }

        public DialogueEffect(string type, string value, int index)
        {
            Type = type;
            Value = value;
            Index = index;
        }

        public virtual IEnumerator Update(DialogueManager manager)
        {
            yield break;
        }
    }
}