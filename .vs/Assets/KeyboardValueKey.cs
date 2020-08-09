// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.UI.Keyboard
{
    [RequireComponent(typeof(Button))]
    public class KeyboardValueKey : MonoBehaviour
    {
        public string Value;

        public string ShiftValue;

        private Text m_Text;

        private Button m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        private void Start()
        {
            m_Text = gameObject.GetComponentInChildren<Text>();
            m_Text.text = Value;

            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(FireAppendValue);

            Keyboard.Instance.OnKeyboardShifted += Shift;
        }

        private void FireAppendValue()
        {
            Keyboard.Instance.AppendValue(this);
        }

        public void Shift(bool isShifted)
        {
            // Shift value should only be applied if a shift value is present.
            if (isShifted && !string.IsNullOrEmpty(ShiftValue))
            {
                m_Text.text = ShiftValue;
            }
            else
            {
                m_Text.text = Value;
            }
        }
    }
}


