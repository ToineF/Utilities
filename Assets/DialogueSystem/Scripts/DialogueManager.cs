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
        public int CurrentCharIndex => _currentCharIndex;
        [field:SerializeField] public float CurrentPlaySpeed { get; set; }

        [SerializeField] private TMP_Text _dialogueTextboxText;

        //[SerializeField] private CharacterFeedbacksManager _feedbackManager;
        [SerializeField] private Image _dialogueTextbox;
        [SerializeField] private TMP_Text _talkingCharacterNameText;
        [SerializeField] private Image _talkingCharacterSprite;
        [SerializeField] private Animator _isDialoguePlayingArrow;
        [SerializeField] private string _isDialoguePlayingArrowCondition;

        [Header("Text Effects")]
        [SerializeField] private TMP_Effect _tmpEffect;
        
        private Dialogue _currentDialogue;
        private int _currentTextIndex;
        private int _currentCharIndex;
        private float _currentCharWaitTime;
        private bool _hasCurrentTextEnded = true;
        private Coroutine _writingCharactersCoroutine;
        private DialogueEffect[] _dialogueEffects;
        private DialogueEffectsFactory _effectsFactory = new DialogueEffectsFactory();

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
                InitEffects(finalText, dialogueEffects);
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

            for (int i = 0; i < finalText.Length; i++)
            {
                yield return UpdateEffects(i, _dialogueEffects);
                
                _currentCharIndex = i;
                char c = finalText[i];

                if (characterData.SoundsTalk.Length > 0)
                {
                    if (i % Math.Max(1, characterData.TalkFrequency) == 0 && char.IsLetterOrDigit(c))
                    {
                        var sound = characterData.SoundsTalk.GetRandomItem();
                        //_feedbackManager?.AudioManager?.PlayClip(sound);
                    }
                }
                yield return new WaitForSeconds(CurrentPlaySpeed);
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

        private void InitEffects(string text, DialogueEffect[] dialogueEffects)
        {
            _tmpEffect.Initialize(text, dialogueEffects, this);
            _dialogueEffects = new DialogueEffect[dialogueEffects.Length];
            for (int i = 0; i < dialogueEffects.Length; i++)
            {
                _dialogueEffects[i] = _effectsFactory.CreateDialogueEffect(dialogueEffects[i].Type, dialogueEffects[i].Value, dialogueEffects[i].Index);
            }
        }

        private IEnumerator UpdateEffects(int index, DialogueEffect[] dialogueEffects)
        {
            foreach (var effect in dialogueEffects)
            {
                if (effect.Index == index)  yield return effect.Update(this);
            }
        }
    }
}