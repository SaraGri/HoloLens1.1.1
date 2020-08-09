// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.UI.Keyboard
{
    [RequireComponent(typeof(Button))]
    public class KeyboardKeyFunc : MonoBehaviour
    {
        public enum Function
        {
            // Commands
            Enter,
            Tab,
            ABC,
            Symbol,
            Previous,
            Next,
            Close,
            Dictate,

            // Editing
            Shift,
            CapsLock,
            Space,
            Backspace,

            UNDEFINED,
        }

        public Function m_ButtonFunction = Function.UNDEFINED;

        private Button m_Button = null;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        private void Start()
        {
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(new UnityEngine.Events.UnityAction(FireFunctionKey));
        }

        private void FireFunctionKey()
        {
            Keyboard.Instance.FunctionKey(this);
        }
    }
}

