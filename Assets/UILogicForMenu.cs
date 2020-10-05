
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogicForMenu : MonoBehaviour {
	private bool activElement;
	private List<Button> activeUIButtons;
	public Button[] uiButtons = new Button[8];
	public InputField fileName;
	public Dropdown slicerDropdown;
	public Toggle slicerToggle;
	
	// Use this for initialization
	void Start () {

		fileName = fileName.GetComponent<InputField>();
		slicerDropdown = slicerDropdown.GetComponent<Dropdown>();
		slicerToggle = slicerToggle.GetComponent<Toggle>();
		activeUIButtons = new List<Button>();
		activElement = false;
}

// Update is called once per frame
void Update () {
		
	}

	public void checkUIObjectsActivity() {

		for (int i = 0; i < uiButtons.Length; i++)
		{ 
			if (uiButtons[i].gameObject.activeSelf) {
				activeUIButtons.Add(uiButtons[i]);
				uiButtons[i].gameObject.SetActive(false);
			}
		}
		if (fileName.gameObject.activeSelf || slicerDropdown.gameObject.activeSelf || slicerToggle.gameObject.activeSelf) {
			fileName.gameObject.SetActive(false);
			slicerDropdown.gameObject.SetActive(false); 
			slicerToggle.gameObject.SetActive(false);
			activElement = true;
		}
	}

	public void reactivateClosedUIElements() {
		foreach (var button in activeUIButtons) {
			button.gameObject.SetActive(true);
		}
		if (activElement) {
			fileName.gameObject.SetActive(true);
			slicerDropdown.gameObject.SetActive(true);
			slicerToggle.gameObject.SetActive(true);
			activElement = false;
		}
		activeUIButtons = new List<Button>();
	}
}
