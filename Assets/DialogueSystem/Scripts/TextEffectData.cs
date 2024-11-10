using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName ="New Text Effect", menuName ="Dialogue/Text Effect")]
    public class TextEffectData : ScriptableObject
    {
        [SerializeField] public string Key;
        [SerializeField] public TextEffect TextEffect;
        [SerializeField] public Vector3Displacement VertexMathDisplacement;
        [SerializeField] public Vector3Displacement CharMathDisplacement;
    }
}