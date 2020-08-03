using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class InveractiveCube : MonoBehaviour, IFocusable, IInputClickHandler 
{
    bool isInFocus;
    // Use this for initialization
    void Start () {
        isInFocus = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // A little trick to achive a OnInputClicked-Event on this GameObject since 
    // the event does not provide the reference of the selected object.
    // Using HoloLens first Generation, the user hase to gaze at the object he 
    // wants to interact with, so the object has to be on focus. During beeing 
    // on focus, the object can simple be clicked by the HoloLens input clicked event
    // and we also can determine which object it is because each interactable GameObject
    // within the scene has its own script for this situation :) -> specified by "this." 
    // in that case. 
    public void OnFocusEnter()
    {
        isInFocus = true;
        Debug.Log("Focus on InteractionCube");
        //throw new System.NotImplementedException();
    }

    public void OnFocusExit()
    {
        isInFocus = false;
        Debug.Log("NO Focus on InteractionCube");
        //throw new System.NotImplementedException();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (isInFocus)
        {
            Debug.Log("Picked " + this.gameObject.name);
            //Application.Quit(); // Does not work
        }

        //Debug.Log(eventData.selectedObject.gameObject.name);  // nullRef :(
        //throw new System.NotImplementedException();
    }

}
