using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class TextMeshPro_Effect : TMP_Text
    {
        private void LateUpdate()
        {
            PlayTextAnimation();
        }
        
        private void PlayTextAnimation()
        {
            _dialogueTextboxText.ForceMeshUpdate();
            TMP_TextInfo textInfo = _dialogueTextboxText.textInfo;
            bool[] visitedCharacters = new bool[textInfo.characterCount];

            if (_textEffects == null) return;

            for (int i = 0; i < _textEffects.Length; i++)
            {
                bool isCharacterHidden = _currentCharIndex < i && _currentCharIndex != -1; 
                var data = isCharacterHidden ? _hiddenEffectData : _textEffects[i];

                if (data == null) continue;
                
                if (isCharacterHidden == false) _characterApparitionTimers[i] += Time.deltaTime;
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];


                if (!charInfo.isVisible) continue;
                if (visitedCharacters[i]) continue;
                visitedCharacters[i] = true;

                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                Vector3 upCenterPoint = new Vector2((meshInfo.vertices[charInfo.vertexIndex].x + meshInfo.vertices[charInfo.vertexIndex + 2].x) / 2, meshInfo.vertices[charInfo.vertexIndex].y);
                Vector3 middleCenterPoint = new Vector2((meshInfo.vertices[charInfo.vertexIndex].x + meshInfo.vertices[charInfo.vertexIndex + 2].x) / 2, (meshInfo.vertices[charInfo.vertexIndex].y + meshInfo.vertices[charInfo.vertexIndex].y + 1) / 2);
                Vector3 charData = data.CharMathDisplacement.GetTotalFunction(middleCenterPoint);
                Vector3 characterScale = Vector3.zero;
                if (isCharacterHidden == false) characterScale = data.TextEffect.GetCurrentScale(_characterApparitionTimers[i]);

                for (int j = 0; j < 4; j++)
                {
                    int index = charInfo.vertexIndex + j;
                    Vector3 origin = meshInfo.vertices[index];
                    meshInfo.vertices[index] = origin + data.VertexMathDisplacement.GetTotalFunction(origin) + charData;
                    meshInfo.colors32[index] = data.TextEffect.Colors[j];
                    if (isCharacterHidden == false) meshInfo.vertices[index] += data.TextEffect.ReturnAddedScaledPosition(meshInfo.vertices[index], characterScale, middleCenterPoint);
                }
            }
    }
}