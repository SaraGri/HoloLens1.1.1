using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILogicForMenu : MonoBehaviour {
	public Button[] uiButtons = new Button[7];
	public InputField fileName;
	public Dropdown slicerDropdown;
	public Toggle slicerToggle;
	private bool activate;
	private List<Button> activeUIButtons;
	//public List<Button> uiButtons;
	//public Button b;
	// Use this for initialization
	void Start () {
		fileName = fileName.GetComponent<InputField>();
		slicerDropdown = slicerDropdown.GetComponent<Dropdown>();
		slicerToggle = slicerToggle.GetComponent<Toggle>();
		activate = false;
		activeUIButtons = new List<Button>();
		//b = b.GetComponent<Button>();
		//fileName.gameObject.SetActive(true);
		//slicerDropdown.gameObject.SetActive(true);
		//slicerToggle.gameObject.SetActive(true);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void checkUIObjectsActive()
    {

		for (int i = 0; i < uiButtons.Length; i++)
		{
			//https://answers.unity.com/questions/790671/how-to-check-if-object-is-active.html
			if (uiButtons[i].gameObject.activeSelf)
			{
				activeUIButtons.Add(uiButtons[i]);
				uiButtons[i].gameObject.SetActive(false);
			}
		}
		if (fileName.gameObject.activeSelf || slicerDropdown.gameObject.activeSelf || slicerToggle.gameObject.activeSelf) {
			activate = true;
			fileName.gameObject.SetActive(false);
			slicerDropdown.gameObject.SetActive(false); 
			slicerToggle.gameObject.SetActive(false);
		}
	}

	public void reactivateClosedUIElements() {
		foreach (var button in activeUIButtons) {
			button.gameObject.SetActive(true);
		}
		fileName.gameObject.SetActive(activate);
		slicerDropdown.gameObject.SetActive(activate);
		slicerToggle.gameObject.SetActive(activate);
		activeUIButtons = new List<Button>();
	}
}
