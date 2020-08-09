using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inputtext : MonoBehaviour
{
	UnityEngine.TouchScreenKeyboard keyboard;
	public static string keyboardText = "";
	public GameObject ob;

	void start() {
		ob.GetComponent<InputField>();
		tastatur();


	}
	void update()
	{
		texteingabe();
	}

	public void tastatur()
	{
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
	}
	public void texteingabe()
	{
		if (TouchScreenKeyboard.visible == false && keyboard != null)
		{
			if (keyboard.done == true)
			{
				keyboardText = keyboard.text;
				keyboard = null;
			}
		}
	}

	public string Keyboardeingabe()
	{
		return keyboardText;
	}
}