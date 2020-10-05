using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.IO; //für Stream
using UnityEngine.UI;
using System;
using System.Diagnostics;

#if WINDOWS_UWP
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System;
#endif

public class ScanUILogic : MonoBehaviour, IInputClickHandler
{

	private bool SMapping, SUnderStanding;
	private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh; //von andy
	private const string WavefrontFileExtension = ".obj"; //aus RoomMeshExporter
	private List<MeshFilter> meshfilters;
	private bool isMappingRunning, isUnderstandingRunning;
	private string fileName;
	private bool slicing;

	// Quelle: https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_SystemKeyboard.html
	private TouchScreenKeyboard keyboard;
	//

	//private bool terminateAppV;
	public GameObject understanding, mapping;
	public InputField fileNameInput;
	//public Button saveMesh;
	public string saveMeshOnHoloLens;

	//für Sclicer
	public List<GameObject> bounds;
	public Toggle slicetoggle;
	public GameObject defaultCursor;
	public Dropdown sliceDropdown;
	private AnimatedCursor animatedCurser;
	private float helperSphereSize = 0.01f;
	private List<Vector3> verts;
	private GameObject sliceObject;

	//Test
	private float spTime, suTime, sliceTime, saveTime;
	private float spTime2, suTime2, sliceTime2, saveTime2;
	PerformanceCounter cpuCounter;
	PerformanceCounter ramCounter;
	private string sms, sus, scl, ram;
	private string cpuspeichern, cpspeichenr2, cpuspatailM, cpuSpatialU, cpuspatailM2, cpuSpatialU2, cpS, cpS2, startCPu;

	//Declare these in your class
	private int m_frameCounter = 0;
	private float m_timeCounter = 0.0f;
	private float m_lastFramerate = 0.0f;
	public float m_refreshTime = 0.5f;


	private float spatialmappingTime;

	// Use this for initialization
	void Start () {
		cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		ramCounter = new PerformanceCounter("Memory", "Available MBytes");
		startCPu = getCurrentCpuUsage();
		ram = getAvailableRAM();

		spatialUnderstandingCustomMesh = understanding.GetComponent<SpatialUnderstandingCustomMesh>(); //andys Idee, meine Änderung
		isMappingRunning = false;
		isUnderstandingRunning = false;
		SMapping = false;
		SUnderStanding = false;
		
		fileNameInput = fileNameInput.GetComponent<InputField>();
		//saveMesh = saveMesh.GetComponent<Button>();
		fileName = null;
		//terminateAppV = false;

		//für sclicer
		animatedCurser = defaultCursor.GetComponent<AnimatedCursor>();
		verts = new List<Vector3>();

		bounds = new List<GameObject>();
		sliceDropdown = sliceDropdown.gameObject.GetComponent<Dropdown>();
		//GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//sphere.transform.position = new Vector3(0, 1.5f, 0);
		//sphere.transform.localScale = new Vector3(2, 2, 6);
		spatialmappingTime = 0;
		slicing = false;
		

	}

	// Update is called once per frame
	void Update () {
		// Quelle: https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_SystemKeyboard.html
		//Quelle: https://docs.microsoft.com/en-us/dotnet/api/system.datetime?view=netframework-4.8
		if (keyboard != null)
		{
			fileName =  keyboard.text + "_" + DateTime.Now.ToString("ss.dd.MM.yyyy");
		}
		//
		//
		
		//sclice
		if (verts.Count > 2)
		{
			getObserverType();
			//sliceObject = createCube();
			if (sliceObject != null) {
				bounds.Add(sliceObject);
				//Debug.Log("Objekt ausgewählt: " + sliceObject.ToString()); 
			}
		}

		if (m_timeCounter < m_refreshTime)
		{
			m_timeCounter += Time.deltaTime;
			m_frameCounter++;
		}
		else
		{
			//This code will break if you set your m_refreshTime to 0, which makes no sense.
			m_lastFramerate = (float)m_frameCounter / m_timeCounter;
			m_frameCounter = 0;
			m_timeCounter = 0.0f;
		}
	
	}

	//Slice

	public void OnInputClicked(InputClickedEventData eventData)
	{
		//throw new NotImplementedException();
		
		Vector3 hitPoint = defaultCursor.transform.position;
		if (slicetoggle.isOn)
		{
			//getObserverType();
			verts.Add(hitPoint);
			GameObject helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			helperSphere.transform.position = hitPoint;
			helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);

			Renderer helperSphereRenderer = helperSphere.GetComponent<Renderer>();
			//Debug.Log(hitPoint.ToString());
			//helperSphereRenderer.material = sp;
			slicing = true;
		}
		//
		//

	}

	private void getObserverType()
	{
		//
	
		switch (sliceDropdown.options[sliceDropdown.value].text)
		{
			case "cube":
				sliceObject = createCube();
				//Debug.Log("cube");
				break;
			case "circle":
				sliceObject = createCircle();
				//Debug.Log("circle");
				break;
		
			default:
				break;
		}
		//
		//
	}

	public void re()
	{
		cpS = getCurrentCpuUsage();
		bounds.Add(sliceObject);
		RemoveVertices(bounds);
		//Debug.Log("remove wird durchlafen");
		//bounds.Clear();
		sliceTime2 = DateTime.Now.Millisecond;
		cpS2 = getCurrentCpuUsage();
	}

	private GameObject createCube()
	{
		
		
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		float dist = 1;
		if (verts.Count >= 2)
		{
			dist = Vector3.Distance(verts[0], verts[1]);
		}
		cube.transform.position = verts[0];
		cube.transform.localScale = new Vector3(dist, dist, 6);
		
		var boxCollider2 = cube.AddComponent<BoxCollider>();
		return cube;
		//
		//
		
	}

	private GameObject createCircle()
	{
		
		GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		float dist = 1;
		if (verts.Count >= 2)
		{
			dist = Vector3.Distance(verts[0], verts[1]);
		}
		sph.transform.position = verts[0];
		sph.transform.localScale = new Vector3(dist, dist, 6);
	
		var boxCollider2 = sph.AddComponent<SphereCollider>();
		return sph;
		
	}

	public void RemoveVertices(IEnumerable<GameObject> boundingObjects)
	{
		//Quelle:SDK HoloToolkit
		//sliceTime = DateTime.Now.Millisecond;
		RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
		if (removeVerts != null && removeVerts.enabled)
		{
			removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
			//Debug.Log("remove ngjklhjhjk");
		}

		
	}



	public string getCurrentCpuUsage()
	{
		return cpuCounter.NextValue() + "%";
	}

	public string getAvailableRAM()
	{
		return ramCounter.NextValue() + "MB";
	}



	//UI Logik für Spatial Mapping
	public void SpatialMappingScan(bool scan)
	{
		SMapping = true;
		if (scan == true)
		{
			cpuspatailM = getCurrentCpuUsage();
			//MappingObserver starten 
			SpatialMappingManager.Instance.StartObserver();
			SpatialMappingManager.Instance.DrawVisualMeshes = true; // Meshes rendern

			isMappingRunning = true;
			//Debug.Log("spatialMapping startet");
			spTime = DateTime.Now.Millisecond;
			//spatialmappingTime =  SpatialMappingManager.Instance.StartTime;
		}

		if (scan == false)
		{
			//Der SurfaceObserver wird angewiesen das Updating vom SpatialMapping Mesh zu unterlassen 
			SpatialMappingManager.Instance.StopObserver();
			SpatialMappingManager.Instance.DrawVisualMeshes = false; //damit Meshes nicht mehr gerendert werden
			isMappingRunning = false;
			//Debug.Log("spatialMapping stopped");
			spTime2 = DateTime.Now.Millisecond;
		}

	}

	public void SpatialMappingScanStop(bool scan)
	{
		SMapping = true;
		if (scan == true)
		{
			//MappingObserver starten 
			SpatialMappingManager.Instance.StartObserver();
			SpatialMappingManager.Instance.DrawVisualMeshes = true; // Meshes rendern
			isMappingRunning = true;
			//Debug.Log("spatialMapping startet");
			spTime = DateTime.Now.Millisecond;
			spatialmappingTime = SpatialMappingManager.Instance.StartTime;
		}

		if (scan == false)
		{
			//Der SurfaceObserver wird angewiesen das Updating vom SpatialMapping Mesh zu unterlassen 
			SpatialMappingManager.Instance.StopObserver();
			SpatialMappingManager.Instance.DrawVisualMeshes = false; //damit Meshes nicht mehr gerendert werden
			isMappingRunning = false;
			//Debug.Log("spatialMapping stopped");
			spTime2 = DateTime.Now.Millisecond;
		}

	}

	//UI Logik für Spatial Understanding
	private void ScanStateChanged()
	{
		if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
		{
			//Debug.Log("Scanning Understanding");
		}
		else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
		{
			//Debug.Log("Done Understanding");
		}
	}

	public void SpatialUnderstandingStartScan()
	{
		SUnderStanding = true;
		SpatialUnderstanding.Instance.RequestBeginScanning();
		//SpatialUnderstanding.Instance.ScanStateChanged += ScanStateChanged;
		isUnderstandingRunning = true;
	//	Debug.Log("spatialunderstanding startet");
		suTime = DateTime.Now.Millisecond;
	}

	private void SpatialUnderstandingStopScanOrRestartScan(bool scan)
	{
		spatialUnderstandingCustomMesh.DrawProcessedMesh = scan;
	//	Debug.Log(" spatialunderstanding " + scan);
	}

	private void SpatialUnderstandingFinishScan()
    {
		SpatialUnderstanding.Instance.RequestFinishScan();
		isUnderstandingRunning = false;
		//Debug.Log("spatialunderstanding finished");
		suTime2 = DateTime.Now.Millisecond;
	}

	//Beide Scans starten 
	public void StartSpatialMappingAndSpatialUnderstanding()
	{
		SpatialUnderstandingStartScan();
		SpatialMappingScan(true);
		isMappingRunning = true;
		isUnderstandingRunning = true;
	}

	public void HaltOrRestartScan(bool scan) {

		if (SMapping && SUnderStanding) {
			SpatialMappingScan(scan);
			SpatialUnderstandingStopScanOrRestartScan(scan);
			//Debug.Log("SMapping && SUnderStanding" + scan.ToString());
		}
		else if (SMapping) {
			SpatialMappingScan(scan);
		//	Debug.Log("SMapping" + scan.ToString());
		}

		else if (SUnderStanding) {
			SpatialUnderstandingStopScanOrRestartScan(scan);
			//Debug.Log("SUnderStanding" + scan.ToString());
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
		
		saveTime = DateTime.Now.Millisecond;
		
		checkScanstateBeforeSaving();
		if (slicing == true) {
			re();
		}
		if (fileName == null) {
			fileName = "default" + "_" + DateTime.Now.ToString("dd.MM.yyyy");
		}
		meshfilters = SpatialMappingManager.Instance.GetMeshFilters();
		
		string filePath = Path.Combine(Application.persistentDataPath, fileName + WavefrontFileExtension);
		using (TextWriter writer = File.CreateText(filePath)) 
		{
			writer.Write(SerializeMeshFilters(meshfilters));
		}
		cpuspeichern = getCurrentCpuUsage();
		saveMeshOnDownloadFolder();
		//terminateAppV = true;
		//
		saveTime2 = DateTime.Now.Millisecond;
		sms = getAvailableRAM();
		cpspeichenr2 = getCurrentCpuUsage();
		s();

		SpatialMappingManager.Instance.CleanupObserver();
		
		
	}

	private void s() {

		float t1, t2, t3, t4;
		t1 = spTime2 - spTime;
		t2 = suTime2 - suTime;
		t3 = saveTime2 - saveTime;
		t4 = sliceTime2 - sliceTime;
		string testText = "Systemzeit SpatialMapping " +spTime.ToString() + "\t" + spTime2.ToString() + "\n";
		testText += "Systemzeit SpatialUnderstanding " + "\t" + suTime.ToString() + "  \t  " + suTime2.ToString() + "\n";
		testText += " Systemzeit RemoveTime " + "\t" + sliceTime.ToString() + "  \t  " + sliceTime2.ToString() + "  \t  " + "Time:  "  + t4.ToString() +"\n";
		testText += "SpatialMapping " + t1.ToString() + "  \t  " + "SpatialUnderstanding "  + t2.ToString() + "\t" + "Savingtime " + t3.ToString() + "\n";
		testText += "Framerate " + "  \t  " + m_lastFramerate.ToString() + "\n\n" ;
		//Ram
		testText += "Ram " + "  \t  " + ram  + "  \t  " + sms + "\n\n";

		string filePath = Path.Combine(Application.persistentDataPath, "Testwerte");
		using (TextWriter writer = File.CreateText(filePath))
		{
			writer.Write(testText);
		}
	}
	// 
	// Quelle: SDK
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
	//
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
	void WriteData() {
	//Get local downloadFolder
	StorageFile file = await Windows.Storage.DownloadsFolder.CreateFileAsync(fileName + WavefrontFileExtension);
	FileIO.WriteTextAsync(file,saveMeshOnHoloLens);
}
 
#endif

	public void terminateApp() {
		if (true) {
			Application.Quit();
		}
	}
}
