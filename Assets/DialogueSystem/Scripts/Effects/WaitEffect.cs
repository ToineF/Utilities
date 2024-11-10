using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    public class WaitEffect : DialogueEffect
    {
        public WaitEffect(string type, string value, int index) : base(type, value, index)
        {
        }

        public override IEnumerator Update(DialogueManager manager)
        {
            if (float.TryParse(Value, out float speed) == false)
            {
                Debug.LogError($"{Value} in {this.GetType().Name} is not a number.");
                yield break;
            }
            
            yield return new WaitForSeconds(speed);
        }
    }
}