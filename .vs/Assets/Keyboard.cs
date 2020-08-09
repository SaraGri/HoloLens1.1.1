// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.UI.Keyboard
{
    public class Keyboard : Singleton<Keyboard>, IDictationHandler
    {
        UnityEngine.TouchScreenKeyboard keyboard;
        
        public enum LayoutType
        {
            Alpha,
            Symbol,
            URL,
            Email,
        }

        #region Callbacks

        public event EventHandler OnTextSubmitted = delegate { };

        public event Action<string> OnTextUpdated = delegate { };

        public event EventHandler OnClosed = delegate { };

        public event EventHandler OnPrevious = delegate { };

        public event EventHandler OnNext = delegate { };

        public event EventHandler OnPlacement = delegate { };

        #endregion Callbacks

        public InputField InputField = null;

        public AxisSlider InputFieldSlide = null;

        public bool SliderEnabled = true;

        public bool SubmitOnEnter = true;

        public Image AlphaKeyboard = null;

        public Image SymbolKeyboard = null;

        public Image AlphaSubKeys = null;

        public Image AlphaWebKeys = null;

        public Image AlphaMailKeys = null;

        private LayoutType m_LastKeyboardLayout = LayoutType.Alpha;

        [Header("Positioning")]
        [SerializeField]
        private float m_MaxScale = 1.0f;

        [SerializeField]
        private float m_MinScale = 1.0f;

        [SerializeField]
        private float m_MaxDistance = 3.5f;

        [SerializeField]
        private float m_MinDistance = 0.25f;

        public bool CloseOnInactivity = true;

        public float CloseOnInactivityTime = 15;

        private float _closingTime;

        public event Action<bool> OnKeyboardShifted = delegate { };

        private bool m_IsShifted = false;

        private bool m_IsCapslocked = false;

        public bool IsShifted
        {
            get { return m_IsShifted; }
        }

        public bool IsCapsLocked
        {
            get { return m_IsCapslocked; }
        }

        private int m_CaretPosition = 0;

        private Vector3 m_StartingScale = Vector3.one;

        private Vector3 m_ObjectBounds;

        private Color _defaultColor;

        private Image _recordImage;

        private AudioSource _audioSource;

        protected override void Awake()
        {
            base.Awake();

            m_StartingScale = transform.localScale;
            Bounds canvasBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);

            RectTransform rect = GetComponent<RectTransform>();
            m_ObjectBounds = new Vector3(canvasBounds.size.x * rect.localScale.x, canvasBounds.size.y * rect.localScale.y, canvasBounds.size.z * rect.localScale.z);

            // Actually find microphone key in the keyboard
            var dictationButton = HoloToolkit.Unity.Utils.GetChildRecursive(gameObject.transform, "Dictation");
            if (dictationButton != null)
            {
                var dictationIcon = dictationButton.Find("keyboard_closeIcon");
                if (dictationIcon != null)
                {
                    _recordImage = dictationIcon.GetComponentInChildren<Image>();
                    var material = new Material(_recordImage.material);
                    _defaultColor = material.color;
                    _recordImage.material = material;
                }
            }

            // Setting the keyboardType to an undefined TouchScreenKeyboardType,
            // which prevents the MRTK keyboard from triggering the system keyboard itself.
            InputField.keyboardType = (TouchScreenKeyboardType)(int.MaxValue);
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            // Keep keyboard deactivated until needed
            gameObject.SetActive(false);
        }


        private void Start()
        {
            // Delegate Subscription
            InputField.onValueChanged.AddListener(DoTextUpdated);
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        }

        private void DoTextUpdated(string value)
        {
            if (OnTextUpdated != null)
            {
                OnTextUpdated(value);
            }
        }

        private void LateUpdate()
        {
            // Axis Slider
            if (SliderEnabled)
            {
                Vector3 nearPoint = Vector3.ProjectOnPlane(CameraCache.Main.transform.forward, transform.forward);
                Vector3 relPos = transform.InverseTransformPoint(nearPoint);
                InputFieldSlide.TargetPoint = relPos;
            }
            CheckForCloseOnInactivityTimeExpired();
        }

        private void UpdateCaretPosition(int newPos)
        {
            InputField.caretPosition = newPos;
        }

        private void OnDisable()
        {
            m_LastKeyboardLayout = LayoutType.Alpha;
            Clear();
        }


        public void OnDictationHypothesis(DictationEventData eventData)
        {
        }

        public void OnDictationResult(DictationEventData eventData)
        {
            if (eventData.used)
            {
                return;
            }
            var text = eventData.DictationResult;
            ResetClosingTime();
            if (text != null)
            {
                m_CaretPosition = InputField.caretPosition;

                InputField.text = InputField.text.Insert(m_CaretPosition, text);
                m_CaretPosition += text.Length;

                UpdateCaretPosition(m_CaretPosition);
                eventData.Use();
            }
        }

        public void OnDictationComplete(DictationEventData eventData)
        {
            ResetClosingTime();
           // SetMicrophoneDefault();
        }

        public void OnDictationError(DictationEventData eventData)
        {
        }

        protected override void OnDestroy()
        {
           /* if (IsMicrophoneActive())
            {
                StartCoroutine(DictationInputManager.StopRecording());
            }*/
            base.OnDestroy();
        }

        #region Present Functions

        public void PresentKeyboard()
        {
            ResetClosingTime();
            gameObject.SetActive(true);
            ActivateSpecificKeyboard(LayoutType.Alpha);

            OnPlacement(this, EventArgs.Empty);

            // todo: if the app is built for xaml, our prefab and the system keyboard may be displayed.
            InputField.ActivateInputField();

          //  SetMicrophoneDefault();
        }


        public void PresentKeyboard(string startText)
        {
            PresentKeyboard();
            Clear();
            InputField.text = startText;
        }

        public void PresentKeyboard(LayoutType keyboardType)
        {
            PresentKeyboard();
            ActivateSpecificKeyboard(keyboardType);
        }

        public void PresentKeyboard(string startText, LayoutType keyboardType)
        {
            PresentKeyboard(startText);
            ActivateSpecificKeyboard(keyboardType);
        }

        #endregion Present Functions
        public void RepositionKeyboard(Vector3 kbPos, float verticalOffset = 0.0f)
        {
            transform.position = kbPos;
            ScaleToSize();
            LookAtTargetOrigin();
        }

        public void RepositionKeyboard(Transform objectTransform, BoxCollider aCollider = null, float verticalOffset = 0.0f)
        {
            transform.position = objectTransform.position;

            if (aCollider != null)
            {
                float yTranslation = -((aCollider.bounds.size.y * 0.5f) + verticalOffset);
                transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }
            else
            {
                float yTranslation = -((m_ObjectBounds.y * 0.5f) + verticalOffset);
                transform.Translate(0.0f, yTranslation, -0.6f, objectTransform);
            }

            ScaleToSize();
            LookAtTargetOrigin();
        }

        private void ScaleToSize()
        {
            float distance = (transform.position - CameraCache.Main.transform.position).magnitude;
            float distancePercent = (distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
            float scale = m_MinScale + (m_MaxScale - m_MinScale) * distancePercent;

            scale = Mathf.Clamp(scale, m_MinScale, m_MaxScale);
            transform.localScale = m_StartingScale * scale;

            Debug.LogFormat("Setting scale: {0} for distance: {1}", scale, distance);
        }

        private void LookAtTargetOrigin()
        {
            transform.LookAt(CameraCache.Main.transform.position);
            transform.Rotate(Vector3.up, 180.0f);
        }

        private void ActivateSpecificKeyboard(LayoutType keyboardType)
        {
            DisableAllKeyboards();
            ResetKeyboardState();

            switch (keyboardType)
            {
                case LayoutType.URL:
                    {
                        ShowAlphaKeyboard();
                        TryToShowURLSubkeys();
                        break;
                    }

                case LayoutType.Email:
                    {
                        ShowAlphaKeyboard();
                        TryToShowEmailSubkeys();
                        break;
                    }

                case LayoutType.Symbol:
                    {
                        ShowSymbolKeyboard();
                        break;
                    }

                case LayoutType.Alpha:
                default:
                    {
                        ShowAlphaKeyboard();
                        TryToShowAlphaSubkeys();
                        break;
                    }
            }
        }

        #region Keyboard Functions

       // #region Dictation
        /*
        private void BeginDictation()
        {
            ResetClosingTime();
            //StartCoroutine(DictationInputManager.StartRecording(gameObject));
            SetMicrophoneRecording();
        }

        private bool IsMicrophoneActive()
        {
            var result = _recordImage.color != _defaultColor;
            return result;
        }

        private void SetMicrophoneDefault()
        {
            _recordImage.color = _defaultColor;
        }

        private void SetMicrophoneRecording()
        {
            _recordImage.color = Color.red;
        }

        public void EndDictation()
        {
            StartCoroutine(DictationInputManager.StopRecording());
            SetMicrophoneDefault();
        }

        #endregion Dictation */

        public void AppendValue(KeyboardValueKey valueKey)
        {
            IndicateActivity();
            string value = "";

            // Shift value should only be applied if a shift value is present.
            if (m_IsShifted && !string.IsNullOrEmpty(valueKey.ShiftValue))
            {
                value = valueKey.ShiftValue;
            }
            else
            {
                value = valueKey.Value;
            }

            if (!m_IsCapslocked)
            {
                Shift(false);
            }

            m_CaretPosition = InputField.caretPosition;

            InputField.text = InputField.text.Insert(m_CaretPosition, value);
            m_CaretPosition += value.Length;

            UpdateCaretPosition(m_CaretPosition);
        }
        
        public void FunctionKey(KeyboardKeyFunc functionKey)
        {
            IndicateActivity();
            switch (functionKey.m_ButtonFunction)
            {
                case KeyboardKeyFunc.Function.Enter:
                    {
                        Enter();
                        break;
                    }

                case KeyboardKeyFunc.Function.Tab:
                    {
                        Tab();
                        break;
                    }

                case KeyboardKeyFunc.Function.ABC:
                    {
                        ActivateSpecificKeyboard(m_LastKeyboardLayout);
                        break;
                    }

                case KeyboardKeyFunc.Function.Symbol:
                    {
                        ActivateSpecificKeyboard(LayoutType.Symbol);
                        break;
                    }

                case KeyboardKeyFunc.Function.Previous:
                    {
                        MoveCaretLeft();
                        break;
                    }

                case KeyboardKeyFunc.Function.Next:
                    {
                        MoveCaretRight();
                        break;
                    }

                case KeyboardKeyFunc.Function.Close:
                    {
                        Close();
                        break;
                    }

               /* case KeyboardKeyFunc.Function.Dictate:
                    {
                        if (IsMicrophoneActive())
                        {
                            EndDictation();
                        }
                        else
                        {
                            BeginDictation();
                        }
                        break;
                    }*/

                case KeyboardKeyFunc.Function.Shift:
                    {
                        Shift(!m_IsShifted);
                        break;
                    }

                case KeyboardKeyFunc.Function.CapsLock:
                    {
                        CapsLock(!m_IsCapslocked);
                        break;
                    }

                case KeyboardKeyFunc.Function.Space:
                    {
                        Space();
                        break;
                    }

                case KeyboardKeyFunc.Function.Backspace:
                    {
                        Backspace();
                        break;
                    }

                case KeyboardKeyFunc.Function.UNDEFINED:
                    {
                        Debug.LogErrorFormat("The {0} key on this keyboard hasn't been assigned a function.", functionKey.name);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Backspace()
        {
            // check if text is selected
            if (InputField.selectionFocusPosition != InputField.caretPosition || InputField.selectionAnchorPosition != InputField.caretPosition)
            {
                if (InputField.selectionAnchorPosition > InputField.selectionFocusPosition) // right to left
                {
                    InputField.text = InputField.text.Substring(0, InputField.selectionFocusPosition) + InputField.text.Substring(InputField.selectionAnchorPosition);
                    InputField.caretPosition = InputField.selectionFocusPosition;
                }
                else // left to right
                {
                    InputField.text = InputField.text.Substring(0, InputField.selectionAnchorPosition) + InputField.text.Substring(InputField.selectionFocusPosition);
                    InputField.caretPosition = InputField.selectionAnchorPosition;
                }

                m_CaretPosition = InputField.caretPosition;
                InputField.selectionAnchorPosition = m_CaretPosition;
                InputField.selectionFocusPosition = m_CaretPosition;
            }
            else
            {
                m_CaretPosition = InputField.caretPosition;

                if (m_CaretPosition > 0)
                {
                    --m_CaretPosition;
                    InputField.text = InputField.text.Remove(m_CaretPosition, 1);
                    UpdateCaretPosition(m_CaretPosition);
                }
            }
        }

        public void Previous()
        {
            OnPrevious(this, EventArgs.Empty);
        }

        public void Next()
        {
            OnNext(this, EventArgs.Empty);
        }

        public void Enter()
        {
            if (SubmitOnEnter)
            {
                // Send text entered event and close the keyboard
                if (OnTextSubmitted != null)
                {
                    OnTextSubmitted(this, EventArgs.Empty);
                }

                Close();
            }
            else
            {
                string enterString = "\n";

                m_CaretPosition = InputField.caretPosition;

                InputField.text = InputField.text.Insert(m_CaretPosition, enterString);
                m_CaretPosition += enterString.Length;

                UpdateCaretPosition(m_CaretPosition);
            }

        }

        public void Shift(bool newShiftState)
        {
            m_IsShifted = newShiftState;
            OnKeyboardShifted(m_IsShifted);

            if (m_IsCapslocked && !newShiftState)
            {
                m_IsCapslocked = false;
            }
        }

        public void CapsLock(bool newCapsLockState)
        {
            m_IsCapslocked = newCapsLockState;
            Shift(newCapsLockState);
        }

        public void Space()
        {
            m_CaretPosition = InputField.caretPosition;
            InputField.text = InputField.text.Insert(m_CaretPosition++, " ");

            UpdateCaretPosition(m_CaretPosition);
        }

        public void Tab()
        {
            string tabString = "\t";

            m_CaretPosition = InputField.caretPosition;

            InputField.text = InputField.text.Insert(m_CaretPosition, tabString);
            m_CaretPosition += tabString.Length;

            UpdateCaretPosition(m_CaretPosition);
        }

        public void MoveCaretLeft()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition > 0)
            {
                --m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        public void MoveCaretRight()
        {
            m_CaretPosition = InputField.caretPosition;

            if (m_CaretPosition < InputField.text.Length)
            {
                ++m_CaretPosition;
                UpdateCaretPosition(m_CaretPosition);
            }
        }

        public void Close()
        {
            /*if (IsMicrophoneActive())
            {
                StartCoroutine(DictationInputManager.StopRecording());
            }
            SetMicrophoneDefault();*/
            OnClosed(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            ResetKeyboardState();
            InputField.MoveTextStart(false);
            InputField.text = "";
            m_CaretPosition = InputField.caretPosition;
        }

        #endregion

        public void SetScaleSizeValues(float minScale, float maxScale, float minDistance, float maxDistance)
        {
            m_MinScale = minScale;
            m_MaxScale = maxScale;
            m_MinDistance = minDistance;
            m_MaxDistance = maxDistance;
        }

        #region Keyboard Layout Modes

        public void ShowAlphaKeyboard()
        {
            AlphaKeyboard.gameObject.SetActive(true);
            m_LastKeyboardLayout = LayoutType.Alpha;
        }

        private bool TryToShowAlphaSubkeys()
        {
            if (AlphaKeyboard.IsActive())
            {
                AlphaSubKeys.gameObject.SetActive(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryToShowEmailSubkeys()
        {
            if (AlphaKeyboard.IsActive())
            {
                AlphaMailKeys.gameObject.SetActive(true);
                m_LastKeyboardLayout = LayoutType.Email;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryToShowURLSubkeys()
        {
            if (AlphaKeyboard.IsActive())
            {
                AlphaWebKeys.gameObject.SetActive(true);
                m_LastKeyboardLayout = LayoutType.URL;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ShowSymbolKeyboard()
        {
            SymbolKeyboard.gameObject.SetActive(true);
        }

        private void DisableAllKeyboards()
        {
            AlphaKeyboard.gameObject.SetActive(false);
            SymbolKeyboard.gameObject.SetActive(false);

            AlphaWebKeys.gameObject.SetActive(false);
            AlphaMailKeys.gameObject.SetActive(false);
            AlphaSubKeys.gameObject.SetActive(false);
        }

        private void ResetKeyboardState()
        {
            CapsLock(false);
        }

        #endregion Keyboard Layout Modes

        private void IndicateActivity()
        {
            ResetClosingTime();
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
            if (_audioSource != null)
            {
                _audioSource.Play();
            }
        }

        private void ResetClosingTime()
        {
            if (CloseOnInactivity)
            {
                _closingTime = Time.time + CloseOnInactivityTime;
            }
        }

        private void CheckForCloseOnInactivityTimeExpired()
        {
            if (Time.time > _closingTime && CloseOnInactivity)
            {
                Close();
            }
        }
    }

}
