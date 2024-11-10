using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    public class SpeedChangeEffect : DialogueEffect
    {
        public SpeedChangeEffect(string type, string value, int index) : base(type, value, index)
        {
        }

        public override IEnumerator Update(DialogueManager manager)
        {
            if (float.TryParse(Value, out float speed) == false)
            {
                Debug.LogError($"{Value} in {this.GetType().Name} is not a number.");
                yield break;
            }
            
            manager.CurrentPlaySpeed = speed;
            yield break;
        }
    }
}