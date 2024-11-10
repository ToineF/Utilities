using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class TMP_Effect : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        [Header("Text Effects")]
        [SerializeField] private TextEffectData _hiddenEffectData;
        [SerializeField] private TextEffectData _baseEffectData;
        [SerializeField] private TextEffectDatasList _textEffectsToCheck;
        
        private float[] _characterApparitionTimers;
        private TextEffectData[] _textEffects;
        private DialogueManager _dialogueManager;

        public override string ToString()
        {
            return "TFX";
        }

        public void Initialize(string newText, DialogueEffect[] dialogueEffects, DialogueManager dialogueManager)
        {
            _characterApparitionTimers = new float[newText.Length];
            _dialogueManager = dialogueManager;
            
            IntitializeTextEffects(newText, dialogueEffects);
        }
        
        private void IntitializeTextEffects(string newText, DialogueEffect[] dialogueEffects)
        {
            _textEffects = new TextEffectData[newText.Length];
            for (int i = 0; i < _textEffects.Length; i++)
            {
                _textEffects[i] = _baseEffectData;
            }
            
            // Apply effect at each character
            foreach (TextEffectData data in _textEffectsToCheck.Datas)
            {
                for (int i = 0; i < dialogueEffects.Length; i++)
                {
                    if (ToString() == dialogueEffects[i].Type && data.Key == dialogueEffects[i].Value)
                    {
                        for (int j = dialogueEffects[i].Index; j < (i < dialogueEffects.Length - 1 ? dialogueEffects[i+1].Index : newText.Length); j++)
                        {
                            _textEffects[j] = data;
                        }
                    }
                }
            }
        }
        
        private void LateUpdate()
        {
            PlayTextAnimation();
        }

        private void PlayTextAnimation()
        {
            _text.ForceMeshUpdate();
            TMP_TextInfo textInfo = _text.textInfo;
            bool[] visitedCharacters = new bool[textInfo.characterCount];

            if (_textEffects == null) return;

            for (int i = 0; i < _textEffects.Length; i++)
            {
                bool isCharacterHidden =
                    _dialogueManager.CurrentCharIndex < i && _dialogueManager.CurrentCharIndex != -1;
                var data = isCharacterHidden ? _hiddenEffectData : _textEffects[i];

                if (data == null) continue;

                if (isCharacterHidden == false) _characterApparitionTimers[i] += Time.deltaTime;
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];


                if (!charInfo.isVisible) continue;
                if (visitedCharacters[i]) continue;
                visitedCharacters[i] = true;

                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                Vector3 upCenterPoint =
                    new Vector2(
                        (meshInfo.vertices[charInfo.vertexIndex].x + meshInfo.vertices[charInfo.vertexIndex + 2].x) / 2,
                        meshInfo.vertices[charInfo.vertexIndex].y);
                Vector3 middleCenterPoint =
                    new Vector2(
                        (meshInfo.vertices[charInfo.vertexIndex].x + meshInfo.vertices[charInfo.vertexIndex + 2].x) / 2,
                        (meshInfo.vertices[charInfo.vertexIndex].y + meshInfo.vertices[charInfo.vertexIndex].y + 1) /
                        2);
                Vector3 charData = data.CharMathDisplacement.GetTotalFunction(middleCenterPoint);
                Vector3 characterScale = Vector3.zero;
                if (isCharacterHidden == false)
                    characterScale = data.TextEffect.GetCurrentScale(_characterApparitionTimers[i]);

                for (int j = 0; j < 4; j++)
                {
                    int index = charInfo.vertexIndex + j;
                    Vector3 origin = meshInfo.vertices[index];
                    meshInfo.vertices[index] = origin + data.VertexMathDisplacement.GetTotalFunction(origin) + charData;
                    meshInfo.colors32[index] = data.TextEffect.Colors[j];
                    if (isCharacterHidden == false)
                        meshInfo.vertices[index] += data.TextEffect.ReturnAddedScaledPosition(meshInfo.vertices[index],
                            characterScale, middleCenterPoint);
                }
            }
            
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                _text.UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }
}