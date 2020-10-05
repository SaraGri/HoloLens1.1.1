
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class UserSettings : MonoBehaviour {

	private SpatialMappingObserver surfaceObserver;
	private SpatialMappingManager spatialMappingManager;
	private ObjectSurfaceObserver objectSurfaceObserver;

	private SpatialUnderstanding spatialUnderstanding;
	private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh;
	private TouchScreenKeyboard keyboard;
	private string [] keyboardEntry = new string [11];
	//public static string  = "";
	int index = 0;
	public GameObject mapping, understanding;
	public InputField[] userValue = new InputField[10];
	public Dropdown oType;
	public Toggle draw;
	public Text textForUserInformation;

	// Use this for initialization
	void Start()
	{
		surfaceObserver = mapping.gameObject.GetComponent<SpatialMappingObserver>();
		spatialMappingManager = mapping.gameObject.GetComponent<SpatialMappingManager>();
		objectSurfaceObserver = mapping.GetComponent<ObjectSurfaceObserver>();

		spatialUnderstanding = understanding.gameObject.GetComponent<SpatialUnderstanding>();
		spatialUnderstandingCustomMesh = understanding.gameObject.GetComponent<SpatialUnderstandingCustomMesh>();

		textForUserInformation = textForUserInformation.gameObject.GetComponent<Text>();

		for (int i = 0; i < userValue.Length; i++)
		{
			userValue[i] = userValue[i].gameObject.GetComponent<InputField>();
		}

		oType = oType.gameObject.GetComponent<Dropdown>();
		draw = draw.gameObject.GetComponent<Toggle>();

		for (int i = 0; i < keyboardEntry.Length; i++) {
			keyboardEntry[i] = null;
		}
	}

	// Update is called once per frame
	void Update()
	{
		this.keyboardEntry[0] = "-1";
		if (keyboard != null)
		{
			keyboardEntry[index] = keyboard.text;
		}
	}

	public void OpenKeyboard()
	{
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
	}

	private Vector3 StringToVector(int i) {
		//
		//Quelle: https://answers.unity.com/questions/1134997/string-to-vector3.html
		string[] sInput = keyboardEntry[i].Split('.');
		Vector3 result = new Vector3(
		float.Parse(sInput[0]),
		float.Parse(sInput[1]),
		float.Parse(sInput[2]));
		return result;
		//
		//
	}

	public void getInputFieldIndex(int i)
	{
		index = i;
		Debug.Log(index);

	}
	public void userInput()
	{
		//Testen der Inputwerte lokal
		//this.keyboardEntry[index] = userValue[index].text;
		Debug.Log(keyboardEntry[index]);
		if (TouchScreenKeyboard.visible == false && keyboard != null)
		{

			if (keyboard.done == true)
			{
				//für testen Ausklammern
				keyboardEntry[index] = keyboard.text;
			    keyboard = null;
			}
			//Debug.Log(keyboardEntry[index].GetType());
			
		}

	}
	private void getObserverType() {
		
		switch (oType.value.ToString())
		{
			case "AxisAlignedBox":
				setObserverVolumeType(ObserverVolumeTypes.AxisAlignedBox);
				keyboardEntry[10] = "AxisAlignedBox";
				break;
			case "OrientedBox":
				setObserverVolumeType(ObserverVolumeTypes.OrientedBox);
				keyboardEntry[10] = "OrientedBox";
				break;
			case "Sphere":
				setObserverVolumeType(ObserverVolumeTypes.Sphere);
				keyboardEntry[10] = "Sphere";
				break;
			default:
				setObserverVolumeType(ObserverVolumeTypes.AxisAlignedBox);
				keyboardEntry[10] = "DefaultAxisAlignedBox";
				break;
		}
		//setObserverVolumeType(type);
	}

	private void drawToggle() {
		if (draw.isOn)
		{
			setCastShadows(true);
		}

		else 
		{
			setCastShadows(false);
		}
	}
		
	public void saveSettingsPreference() {

		getObserverType();
		drawToggle();
		if (keyboardEntry[0] != null && keyboardEntry[0] !="") 
		{
			//Debug.Log(keyboardEntry[0] + "hallo");
			float i = float.Parse(keyboardEntry[0]);
			Debug.Log(keyboardEntry[0] + i);
			setTrianglesPerCubicMeter(i);
		}
		if (keyboardEntry[1] != null && keyboardEntry[1] != "")
		{
			setTimeBetweenUpdates(float.Parse(keyboardEntry[1]));
		}
		
		if (keyboardEntry[2] != null && keyboardEntry[2] != "")
		{
			setObservationVolume(StringToVector(2));
		}

		if (keyboardEntry[3] != null && keyboardEntry[3] != "")
		{
			setOriginOfObservationVolume(StringToVector(3));
		}


		if (keyboardEntry[4] != null && keyboardEntry[4] != "")
		{
			setPhysicsLayer(int.Parse(keyboardEntry[4]));
		}

		if (keyboardEntry[5] != null && keyboardEntry[5] != "")
		{
			setSimulatedUpdatePeriodInSeconds(float.Parse(keyboardEntry[6]));
		}

		if (keyboardEntry[6] != null && keyboardEntry[6] != "")
		{
			setUpdatePeriod_DuringScanning(float.Parse(keyboardEntry[7]));
		}

		if (keyboardEntry[7] != null && keyboardEntry[7] != "")
		{
			setUpdatePeriod_AfterScanning(float.Parse(keyboardEntry[8]));
		}

		if (keyboardEntry[8] != null && keyboardEntry[8] != "")
		{
			setImportMeshPeriod(float.Parse(keyboardEntry[9]));
		}

		if (keyboardEntry[9] != null && keyboardEntry[9] != "")
		{
			setMaxFrameTime(float.Parse(keyboardEntry[10]));
		}
		checkInput();
		closeSettingElements(false);
	}

	public void closeSettingElements(bool b) {
		for (int i = 0; i < userValue.Length; i++)
		{
			userValue[i].gameObject.SetActive(b);
		}
		draw.gameObject.SetActive(b);
		oType.gameObject.SetActive(b);
	}

	public void cancelInput()
    {
		for (int i = 0; i < keyboardEntry.Length; i++)
		{
			keyboardEntry[i] = null;
		}
	}

	public void checkInput()
	{
		string testText = "";
		for (int i = 0; i < keyboardEntry.Length; i++) {
			testText = string.Concat (testText, keyboardEntry[index]);
		}
		string filePath = Path.Combine(Application.persistentDataPath, "testUserInput" );
		using (TextWriter writer = File.CreateText(filePath)) 
		{
			writer.Write(testText);
		}
	}

	private void userInformation(string textForField) {
		textForUserInformation.text += "Warning: invalid input in " + textForField + "\n";
	}


	/////////////////////////////////////////
	// Voreinstellungen SpatialMappingManager
	/////////////////////////////////////////

	public void setTrianglesPerCubicMeter(float triangles)
	{
		if (100f < triangles && triangles < 1500f) {
			surfaceObserver.TrianglesPerCubicMeter = triangles;
		}
		else {
			surfaceObserver.TrianglesPerCubicMeter = 500f;
			userInformation("setTrianglesPerCubicMeter");
		}
	}

	public void setTimeBetweenUpdates(float time)
	{
		if (1 < time && time < 15) {
			surfaceObserver.TimeBetweenUpdates = time;
		}
		else {
			surfaceObserver.TimeBetweenUpdates = 3.5f;
			userInformation("setTimeBetweenUpdates");
		}
	}


	public void setObserverVolumeType(ObserverVolumeTypes typem)
	{
		Debug.Log(typem);
		surfaceObserver.ObserverVolumeType = typem;
	}

	public void setObservationVolume(Vector3 volumeObservation)
	{
			surfaceObserver.Extents = volumeObservation;
	}

	public void setOriginOfObservationVolume(Vector3 observationVolume)
	{
		surfaceObserver.Origin = observationVolume;
	}

	public void setOrientation(Quaternion orientation)
	{
		surfaceObserver.Orientation = orientation;
	}


	/////////////////////////////////////////
	// Voreinstellungen SpatialMappingManager
	/////////////////////////////////////////
    
	public void setPhysicsLayer(int layer)
	{
		if (layer > 1 && layer < 100) {
			spatialMappingManager.PhysicsLayer = layer;
		}
		else {
			spatialMappingManager.PhysicsLayer = 31;
			userInformation("setPhysicsLayer");
		}
	}

	public void setDrawVisualMeshes(bool draw)
	{
		spatialMappingManager.DrawVisualMeshes = draw;
	}

	public void setCastShadows(bool shadows)
	{
		spatialMappingManager.CastShadows = shadows;
	}


	/////////////////////////////////////////
	// Voreinstellungen ObjectSurfaceObserver
	/////////////////////////////////////////
	public void setSimulatedUpdatePeriodInSeconds(float seconds)
	{
		if (seconds > - 3  && seconds < 5) {
			objectSurfaceObserver.SimulatedUpdatePeriodInSeconds = seconds;
		}
		else {
			objectSurfaceObserver.SimulatedUpdatePeriodInSeconds = -1;
			userInformation("setSimulatedUpdatePeriodInSeconds");
		}
	}


	////////////////////////////////////////
	// Voreinstellungen SpatialUnderstanding
	////////////////////////////////////////

	public void setAutoBeginScanning(bool auto)
	{
		spatialUnderstanding.AutoBeginScanning = auto;
	}

	public void setUpdatePeriod_DuringScanning(float period)
	{
		if (period > 1 && period < 5) {
			spatialUnderstanding.UpdatePeriod_DuringScanning = period;
		}
		else {
			spatialUnderstanding.UpdatePeriod_DuringScanning = 1.0f;
			userInformation("setUpdatePeriod_DuringScanning");
		}
	}

	public void setUpdatePeriod_AfterScanning(float period)
	{
		if (period > 1 && period < 10) {
			spatialUnderstanding.UpdatePeriod_AfterScanning = period;
		}
		else {
			spatialUnderstanding.UpdatePeriod_AfterScanning = 4.0f;
			userInformation("setUpdatePeriod_AfterScanning");
		}
	}


	///////////////////////////////////////////////////
	// Voreinstellungen SpatialUnderstandingCustomMesh
	///////////////////////////////////////////////////
    
	public void setImportMeshPeriod(float import)
	{
		if (import > -1 && import < 10) {
			spatialUnderstandingCustomMesh.ImportMeshPeriod = import;
		}
		else {
			spatialUnderstandingCustomMesh.ImportMeshPeriod = 1.0f;
			userInformation("setImportMeshPeriod");
		}
	}

	public void setMaxFrameTime(float maxFrameTime)
	{
		if (maxFrameTime > 0 && maxFrameTime < 10) {
			spatialUnderstandingCustomMesh.MaxFrameTime = maxFrameTime;
		}
		else {
			spatialUnderstandingCustomMesh.MaxFrameTime = 5.0f;
			userInformation("setMaxFrameTime");
		}
	}

	public void setDrawProcessedMesh(bool drawMesh)
	{
		spatialUnderstandingCustomMesh.DrawProcessedMesh = drawMesh;
	}

	public void setCreateMeshColliders(bool createMeshColliders)
	{
		spatialUnderstandingCustomMesh.CreateMeshColliders = createMeshColliders;
	}
}
