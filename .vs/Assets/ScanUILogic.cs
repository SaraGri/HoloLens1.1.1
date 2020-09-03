using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.IO; //für Stream
using UnityEngine.UI;
using System;

#if WINDOWS_UWP
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System;
#endif

public class ScanUILogic : MonoBehaviour {

	bool SMapping, SUnderStanding;
	private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh; //von andy
	private const string WavefrontFileExtension = ".obj"; //aus RoomMeshExporter
	private List<MeshFilter> meshfilters;
	private bool isMappingRunning, isUnderstandingRunning;
	private string fileName;
	// https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_SystemKeyboard.html
	private TouchScreenKeyboard keyboard;
	public GameObject understanding;
	public InputField fileNameInput;
	public Button saveMesh;
	public string saveMeshOnHoloLens;

	// Use this for initialization
	void Start () {

		spatialUnderstandingCustomMesh = understanding.GetComponent<SpatialUnderstandingCustomMesh>(); //andys Idee, meine Änderung
		isMappingRunning = false;
		isUnderstandingRunning = false;
		SMapping = false;
		SUnderStanding = false;
		//https://answers.unity.com/questions/933155/how-do-i-access-a-button-component.html
		fileNameInput = fileNameInput.GetComponent<InputField>();
		saveMesh = saveMesh.GetComponent<Button>();
		fileName = null;

	}
	
	// Update is called once per frame
	void Update () {
		//https://docs.microsoft.com/en-us/dotnet/api/system.datetime?view=netframework-4.8
		if (keyboard != null)
		{
			fileName =  keyboard.text + "_" + DateTime.Now.ToString("dd.MM.yyyy");
		}
		
	}

	//UI Logik für Spatial Mapping
	public void SpatialMappingScan(bool scan)
	{
		SMapping = true;
		if (scan == true)
		{
			//MappingObserver starten (falls nicht autoStart in Unity gesetzt)
			SpatialMappingManager.Instance.StartObserver();
			SpatialMappingManager.Instance.DrawVisualMeshes = true;// Meshes rendern
			isMappingRunning = true;
			Debug.Log("SpatialMapping startet");
		}

		if (scan == false)
		{
			// Der SurfaceObserver wird angewiesen das Updating vom SpatialMapping mesh zu unterlassen 
			SpatialMappingManager.Instance.StopObserver();
			SpatialMappingManager.Instance.DrawVisualMeshes = false; //damit Meshes auch nicht gerändert werden
			isMappingRunning = false;
			Debug.Log("SpatialMapping stopped");
		}

	}

	//UI Logik für Spatial Understanding
	private void ScanStateChanged()
	{
		if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
		{
			Debug.Log("Scanning Understanding");
		}
		else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
		{
			Debug.Log("Done Understanding");
		}
	}

	public void SpatialUnderstandingStartScan()
	{
		SUnderStanding = true;
		SpatialUnderstanding.Instance.RequestBeginScanning();
		SpatialUnderstanding.Instance.ScanStateChanged += ScanStateChanged;
		isUnderstandingRunning = true;
	}

	private void SpatialUnderstandingStopScanOrRestartScan(bool scan)
	{
		spatialUnderstandingCustomMesh.DrawProcessedMesh = scan;
		Debug.Log(" Spatialunderstanding stoppen bzw restarten" + scan);
	}

	private void SpatialUnderstandingFinishScan()
    {
		Debug.Log(" Spatialunderstanding beenden");
		SpatialUnderstanding.Instance.RequestFinishScan();
		isUnderstandingRunning = false;
	}

	//Beide Scans starten und anhalten
	//ließe sich auch über das Ui einstellen
	public void StartSpatialMappingAndSpatialUnderstanding()
	{
		SpatialUnderstandingStartScan();
		SpatialMappingScan(true);
		isMappingRunning = true;
		isUnderstandingRunning = true;
	}

	public void HaltOrRestartScan(bool scan) {
		if (SMapping && SUnderStanding)
		{
			SpatialMappingScan(scan);
			SpatialUnderstandingStopScanOrRestartScan(scan);
			Debug.Log("SMapping && SUnderStanding" + scan.ToString());
		}
		else if (SMapping) {
			SpatialMappingScan(scan);
			Debug.Log("SMapping" + scan.ToString());
		}

		else if (SUnderStanding) {
			SpatialUnderstandingStopScanOrRestartScan(scan);
			Debug.Log("SUnderStanding" + scan.ToString());
		}
		
	}

	//Für das Speichern der Meshes
	public void OpenKeyboard() {
		keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
	}

	private void checkScanstateBeforeSaving() {
		//checken ob Scans vor dem Speichern noch aktiv sind
		if (isUnderstandingRunning)
		{
			SpatialUnderstandingFinishScan();
		}

		if (isMappingRunning)
		{
			SpatialMappingScan(false);
		}

		/*if (isMappingRunning && isUnderstandingRunning)
		{
			SpatialUnderstandingFinishScan();
			SpatialMappingScan(false);
		}*/
	}
	
	public void SaveMeshFiltersToWavefront()
	{
		checkScanstateBeforeSaving();
		if (fileName == null) {
			fileName = "default" + "_" + DateTime.Now.ToString("dd.MM.yyyy");
		}
		meshfilters = SpatialMappingManager.Instance.GetMeshFilters();
		//https://answers.unity.com/questions/9862/read-time-and-date.html 
		//Sytemzeit hinzugefügt
		string filePath = Path.Combine(Application.persistentDataPath, fileName + WavefrontFileExtension);
		using (TextWriter writer = File.CreateText(filePath)) //Streamwriter führt zu Pointerfehler beim speicher auf Applikation.persistentDatapath
		{
			writer.Write(SerializeMeshFilters(meshfilters));
		}
		//saveMeshOnDownloadFolder();
	}

	private static string SerializeMeshFilters(IEnumerable<MeshFilter> meshes)
	{
		StringWriter stream = new StringWriter();
		int offset = 0;
		foreach (var mesh in meshes)
		{
			SerializeMeshFilter(mesh, stream, ref offset);
		}
		return stream.ToString();
	}
	
	private static void SerializeMeshFilter(MeshFilter meshFilter, TextWriter stream, ref int offset)
	{
		Mesh mesh = meshFilter.sharedMesh;

		// Write vertices to .obj file. Need to make sure the points are transformed so everything is at a single origin.
		foreach (Vector3 vertex in mesh.vertices)
		{
			Vector3 pos = meshFilter.transform.TransformPoint(vertex);
			stream.WriteLine(string.Format("v {0} {1} {2}", -pos.x, pos.y, pos.z));
		}

		// Write normals. Need to transform the direction.
		foreach (Vector3 meshNormal in mesh.normals)
		{
			Vector3 normal = meshFilter.transform.TransformDirection(meshNormal);
			stream.WriteLine(string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z));
		}

		// Write indices.
		for (int s = 0, sLength = mesh.subMeshCount; s < sLength; ++s)
		{
			int[] indices = mesh.GetTriangles(s);
			for (int i = 0, iLength = indices.Length - indices.Length % 3; i < iLength; i += 3)
			{
				// Format is "vertex index / material index / normal index"
				stream.WriteLine(string.Format("f {0}//{0} {1}//{1} {2}//{2}",
					indices[i + 0] + 1 + offset,
					indices[i + 1] + 1 + offset,
					indices[i + 2] + 1 + offset));
			}
		}

		offset += mesh.vertices.Length;
	}

	// Methoden um auf den Download Ordner der HoloLens zu speichern
	public void saveMeshOnDownloadFolder() {
		meshfilters = SpatialMappingManager.Instance.GetMeshFilters();
		saveMeshOnHoloLens = SerializeMeshFilters(meshfilters);
#if WINDOWS_UWP
        WriteData();
#endif
}

#if WINDOWS_UWP
        
async
	void WriteData () {
	//Get local folder
	StorageFile file = await Windows.Storage.DownloadsFolder.CreateFileAsync(fileName + WavefrontFileExtension);
	FileIO.WriteTextAsync(file,saveMeshOnHoloLens);
}
 
#endif
}
