using UnityEngine;
using TMPro;
using System.Collections;
using System;
using AntoineFoucault.Utilities;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

//using BlownAway.Character.Feedbacks;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public Action OnDialogueEnd;


        public int CurrentTextIndex
        {
            get => _currentTextIndex;
            set
            {
                if (_currentDialogue == null) return;

                if (HasCurrentTextEnded)
                {
                    _currentTextIndex = Mathf.Clamp(value, 0, _currentDialogue.Lines.Length - 1);
                    //AudioManager.Instance?.PlayClip(_dialogueContinueSound);
                }

                if ((value < 0 || value > _currentDialogue.Lines.Length - 1) && HasCurrentTextEnded)
                    EndDialogue();
                else
                    StartNewText();
            }
        }

        [field: SerializeField] public CanvasGroup DialogueUI { get; private set; }

        public bool HasCurrentTextEnded
        {
            get => _hasCurrentTextEnded;
            set
            {
                _hasCurrentTextEnded = value;
                UpdateDialogueEndArrow();
            }
        }

        [SerializeField] private TMP_Text _dialogueTextboxText;

        //[SerializeField] private CharacterFeedbacksManager _feedbackManager;
        [SerializeField] private Image _dialogueTextbox;
        [SerializeField] private TMP_Text _talkingCharacterNameText;
        [SerializeField] private Image _talkingCharacterSprite;
        [SerializeField] private Animator _isDialoguePlayingArrow;
        [SerializeField] private string _isDialoguePlayingArrowCondition;

        [Header("Text Effects")] [SerializeField]
        private TextEffectData _hiddenEffectData;
        [SerializeField] private TextEffectData _baseEffectData;
        [SerializeField] private TextEffectDatasList _textEffectsToCheck;
        
        private float[] _characterApparitionTimers;
        private Dialogue _currentDialogue;
        private int _currentTextIndex;
        private int _currentCharIndex;
        private float _currentCharWaitTime;
        private bool _hasCurrentTextEnded = true;
        private Coroutine _writingCharactersCoroutine;
        private TextEffectData[] _textEffects;
        private DialogueEffect[] _dialogueEffects;

        public void SetNewDialogue(Dialogue newDialogue)
        {
            HasCurrentTextEnded = true;
            _currentDialogue = newDialogue;
            CurrentTextIndex = 0;
            _dialogueTextbox.color = newDialogue.CharacterData.DialogueBoxColor;
            _dialogueTextboxText.color = newDialogue.CharacterData.DialogueTextColor;
            _talkingCharacterNameText.text = newDialogue.CharacterData.Name;
            _talkingCharacterSprite.sprite = newDialogue.CharacterData.Sprite;
        }

        private void GoToTextAt(int index)
        {
            CurrentTextIndex = index;
        }

        public void GoToNextText()
        {
            GoToTextAt(CurrentTextIndex + 1);
        }

        private void StartNewText()
        {
            string currentText = _currentDialogue.Lines[CurrentTextIndex].Text;

            if (!HasCurrentTextEnded)
            {
                FullyWriteText();
            }
            else
            {
                string finalText = GetTextEffectsInString(currentText, out DialogueEffect[] dialogueEffects);
                IntitializeTextEffects(finalText, dialogueEffects);
                _dialogueEffects = dialogueEffects;
                _writingCharactersCoroutine = StartCoroutine(WriteEachCharacter(_dialogueTextboxText, finalText, _currentDialogue.CharacterData));
            }
        }

        private IEnumerator WriteEachCharacter(TMP_Text dialogueTextbox, string finalText,
            DialogueCharacterData characterData)
        {
            _talkingCharacterSprite.sprite = _currentDialogue.Lines[CurrentTextIndex].SpriteOverride;
            if (_currentDialogue.Lines[CurrentTextIndex].SpriteOverride == null)
                _talkingCharacterSprite.sprite = _currentDialogue.CharacterData.Sprite;
            //Debug.Log((_currentDialogue?.Lines[CurrentTextIndex].SpriteOverride) ?? (_currentDialogue.CharacterData.Sprite));

            HasCurrentTextEnded = false;
            dialogueTextbox.text = finalText;
            _currentCharIndex = 0;
            _characterApparitionTimers = new float[finalText.Length];

            for (int i = 0; i < finalText.Length; i++)
            {
                _currentCharIndex = i;
                char c = finalText[i];
                _currentCharWaitTime = _baseEffectData.TextEffect.CharacterApparitionTime;

                if (characterData.SoundsTalk.Length > 0)
                {
                    if (i % Math.Max(1, characterData.TalkFrequency) == 0 && char.IsLetterOrDigit(c))
                    {
                        var sound = characterData.SoundsTalk.GetRandomItem();
                        //_feedbackManager?.AudioManager?.PlayClip(sound);
                    }
                }

                UpdateEffects(i, _dialogueEffects);

                yield return new WaitForSeconds(_textEffects[_currentCharIndex]?.TextEffect.CharacterApparitionTime ?? _currentCharWaitTime);
            }

            HasCurrentTextEnded = true;
            _currentCharIndex = -1;
        }

        private void FullyWriteText()
        {
            if (_writingCharactersCoroutine != null) StopCoroutine(_writingCharactersCoroutine);
            _currentCharIndex = -1;
            HasCurrentTextEnded = true;
        }

        private void EndDialogue()
        {
            OnDialogueEnd?.Invoke();
        }

        private void UpdateDialogueEndArrow()
        {
            //_isDialoguePlayingArrow.SetBool(_isDialoguePlayingArrowCondition, HasCurrentTextEnded);
        }

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

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                meshInfo.mesh.colors32 = meshInfo.colors32;
                _dialogueTextboxText.UpdateGeometry(meshInfo.mesh, i);
            }
        }

        private string GetTextEffectsInString(string text, out DialogueEffect[] dialogueEffects)
        {
            string newText = text;
            
            // Split Text and get effects
            string[] effects = Regex.Matches(newText, @"<.*?=.*?>").Select(m => m.Value).ToArray();
            dialogueEffects = new DialogueEffect[effects.Length];
            
            for (int i = 0; i < effects.Length; i++)
            {
                var effect = effects[i];
                var input = effect.Substring(1, effect.Length - 2).Split('=');
                
                var type = input[0];
                var value = input[1];
                var index = newText.IndexOf(effect, StringComparison.Ordinal);
                
                dialogueEffects[i] = new DialogueEffect(type, value, index);
                
                newText = newText.ReplaceFirst(effect, String.Empty);
            }
            
            return newText;
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
                    if (data.Key == dialogueEffects[i].Value)
                    {
                        for (int j = dialogueEffects[i].Index; j < (i < dialogueEffects.Length - 1 ? dialogueEffects[i+1].Index : newText.Length); j++)
                        {
                            _textEffects[j] = data;
                        }
                    }
                }
            }
        }
        
        private void UpdateEffects(int index, DialogueEffect[] dialogueEffects)
        {
            foreach (var effect in dialogueEffects)
            {
                if (effect.Index == index) effect.Update();
            }
        }
    }
}