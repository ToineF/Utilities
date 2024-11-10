using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName ="New Text Effect List", menuName ="Dialogue/Text Effect List")]
    public class TextEffectDatasList : ScriptableObject
    {
        [field:SerializeField] public TextEffectData[] Datas { get; private set; }
    }
}