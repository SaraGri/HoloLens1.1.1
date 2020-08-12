using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class Voreinstellungen : MonoBehaviour {
	

	private SpatialMappingObserver surfaceObserver;
	private SpatialMappingManager spatialMappingManager;
	private ObjectSurfaceObserver objectSurfaceObserver;

	private SpatialUnderstanding spatialUnderstanding;
	private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh;

	public InputField[] vEingabe = new InputField[9];

	// Use this for initialization
	void Start () {
		surfaceObserver = gameObject.GetComponent<SpatialMappingObserver>();
		spatialMappingManager = gameObject.GetComponent<SpatialMappingManager>();
		objectSurfaceObserver = gameObject.GetComponent<ObjectSurfaceObserver>();

		spatialUnderstanding = gameObject.GetComponent<SpatialUnderstanding>();
		spatialUnderstandingCustomMesh = gameObject.GetComponent<SpatialUnderstandingCustomMesh>();

        for (int i = 0; i < 9; i++)
        {
			//vEingabe[i] = gameObject.GetComponent<InputField>();
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/// <summary>
	/// /////////////Voreinstellungen SpatialMappingObserver
	/// </summary>
	/// <param name="anzahl"></param>
	public void setTrianglesPerCubicMeter(float anzahl) {
		surfaceObserver.TrianglesPerCubicMeter = anzahl;
	}

	public void setTimeBetweenUpdates(float zeit)
	{
		surfaceObserver.TimeBetweenUpdates = zeit;
	}

    
	public void setObserverVolumeType(ObserverVolumeTypes type)
	{
		surfaceObserver.ObserverVolumeType = type;
	}

	public void setObservationVolume(Vector3 v)
	{
		surfaceObserver.Extents = v;
	}

	public void setOriginOfObservationVolume(Vector3 v)
	{
		surfaceObserver.Origin = v;
	}

	public void setOrientation(Quaternion i)
	{
		surfaceObserver.Orientation = i;
	}

	////////////////////////////////////
	/////////////////////////////////// Voreinstellungen SpatialMappingManager
	public void setPhysicsLayer(int layer)
	{
		spatialMappingManager.PhysicsLayer = layer;
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
	public void setSimulatedUpdatePeriodInSeconds(float b)
	{
		objectSurfaceObserver.SimulatedUpdatePeriodInSeconds = b;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//SpatialUnderstanding Voreinstellungen

	public void setAutoBeginScanning(bool b)
	{
		spatialUnderstanding.AutoBeginScanning = b;
	}

	public void setUpdatePeriod_DuringScanning(float b)
	{
		spatialUnderstanding.UpdatePeriod_DuringScanning = b;
	}

	public void setUpdatePeriod_AfterScanning(float b)
	{
		spatialUnderstanding.UpdatePeriod_AfterScanning = b;
	}
	/// <summary>
	/// /////////////////////////////// SpatialUnderstandingCostumMesh
	/// </summary>
	public void setImportMeshPeriod(float b)
	{
		spatialUnderstandingCustomMesh.ImportMeshPeriod = b;
	}

	public void setMaxFrameTime(float b)
	{
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
