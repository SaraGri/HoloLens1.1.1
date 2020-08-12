using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class Voreinstellungen : MonoBehaviour {

	// Ref to spatial mapping and spatial understanding
	public GameObject spatialMapping;
	public GameObject spatialUnderstanding;

	private SpatialMappingManager spatialMappingManager;
	private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh;
	private SpatialUnderstanding spa;
	private SpatialMappingObserver surfaceObserver;
	//public SpatialMappingManager spatialMappingManager;
	private ObjectSurfaceObserver objectSurfaceObserver;



	public Dropdown meshColliders;
	public InputField[] vEingabe = new InputField[9];
	private bool b;
	private int dValue;

	// Use this for initialization
	void Start () {
		surfaceObserver = spatialMapping.GetComponent<SpatialMappingObserver>();
		spatialMappingManager = spatialMapping.GetComponent<SpatialMappingManager>();
		objectSurfaceObserver = spatialMapping.GetComponent<ObjectSurfaceObserver>();
		spatialUnderstandingCustomMesh = spatialUnderstanding.GetComponent<SpatialUnderstandingCustomMesh>();
		spa = spatialUnderstanding.GetComponent<SpatialUnderstanding>();

	}
	
	// Update is called once per frame
	void Update () {
	//	b = bool.Parse(meshColliders.value.ToString);
		for (int i = 0; i < vEingabe.Length; i++)
		{

			vEingabe[i] = vEingabe[i].GetComponent<InputField>();
		}

		dValue = meshColliders.GetComponent<Dropdown>().value;
	}
	// https://answers.unity.com/questions/1167834/how-do-you-access-the-text-value-of-the-dropdown-u.html
	public void dropdownEingabe() {
		if (dValue == 1) {
			b = true;
			Debug.Log("wert " + b);
		}
		else if (dValue == 0) {
			b = false;
			Debug.Log("wert " + b);
		}

	}
	/// <summary>
	/// /////////////Voreinstellungen SpatialMappingObserver
	/// </summary>
	/// <param name="anzahl"></param>
	public void setTrianglesPerCubicMeter() {
		
		float a = float.Parse(vEingabe[0].text);
		surfaceObserver.TrianglesPerCubicMeter = a;
		Debug.Log("zahl " + a);
	}

	public void setTimeBetweenUpdates()
	{
		
		float zeit = float.Parse(vEingabe[9].text);
		
		surfaceObserver.TimeBetweenUpdates = zeit;
	}

    //kein Inputfeld
	public void setObserverVolumeType(ObserverVolumeTypes type)
	{
		
		surfaceObserver.ObserverVolumeType = type;
	}


	/// von https://answers.unity.com/questions/1134997/string-to-vector3.html
	public static Vector3 StringToVector3(string sVector)
	{
		// Remove the parentheses
		if (sVector.StartsWith("(") && sVector.EndsWith(")"))
		{
			sVector = sVector.Substring(1, sVector.Length - 2);
		}

		// split the items
		string[] sArray = sVector.Split(',');

		// store as a Vector3
		Vector3 result = new Vector3(
			float.Parse(sArray[0]),
			float.Parse(sArray[1]),
			float.Parse(sArray[2]));

		return result;
	}

	/// 

	public void setObservationVolume()
	{
		Vector3 v = StringToVector3(vEingabe[1].text);
		surfaceObserver.Extents = v;
		Debug.Log(v);
	}

	public void setOriginOfObservationVolume()
	{
		Vector3 v = StringToVector3(vEingabe[2].text);
		surfaceObserver.Origin = v;
		Debug.Log(v.x + v.y + v.z);
	}
	 //kein inputfeld
	public void setOrientation(Quaternion i)
	{
		surfaceObserver.Orientation = i;
	}

	////////////////////////////////////
	/////////////////////////////////// Voreinstellungen SpatialMappingManager
	//kein Inputfeld
	public void setPhysicsLayer(int a)
	{
		spatialMappingManager.PhysicsLayer = a;
	}

	public void setDrawVisualMeshes(bool b)
	{
		spatialMappingManager.DrawVisualMeshes = b;
	}

	public void setCastShadows(bool b)
	{
		spatialMappingManager.CastShadows = b;
	}

	////////////////////////////////////
	/////////////////////////////////// Voreinstellungen ObjectSurfaceObserver
	///
	public void setSimulatedUpdatePeriodInSeconds()
	{

		float b = float.Parse(vEingabe[4].text);
		
		objectSurfaceObserver.SimulatedUpdatePeriodInSeconds = b;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//SpatialUnderstanding Voreinstellungen

	public void setAutoBeginScanning(bool b)
	{
		spa.AutoBeginScanning = b;
	}

	public void setUpdatePeriod_DuringScanning()
	{
		float b = float.Parse(vEingabe[5].text);
		
		spa.UpdatePeriod_DuringScanning = b;
	}

	public void setUpdatePeriod_AfterScanning()
	{
		float b = float.Parse(vEingabe[8].text);
		
		spa.UpdatePeriod_AfterScanning = b;
	}
	/// <summary>
	/// /////////////////////////////// SpatialUnderstandingCostumMesh
	/// </summary>
	public void setImportMeshPeriod()
	{

		float b = float.Parse(vEingabe[6].text);
		
		spatialUnderstandingCustomMesh.ImportMeshPeriod = b;
	}

	public void setMaxFrameTime()
	{
		float b = float.Parse(vEingabe[7].text);
		
		spatialUnderstandingCustomMesh.MaxFrameTime = b;
	}

	public void setDrawProcessedMesh(bool b)
	{
		spatialUnderstandingCustomMesh.DrawProcessedMesh = b;
	}

	public void setCreateMeshColliders(bool b)
	{
		spatialUnderstandingCustomMesh.CreateMeshColliders = b;
	}

}
