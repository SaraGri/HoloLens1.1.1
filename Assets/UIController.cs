﻿using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using System.IO;
using UnityEditor;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.UI;

#if WINDOWS_UWP
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System;
#endif
public class UIController : MonoBehaviour
{

    // WaveFrontFileErweiterung für die Speicherung in obj
    private const string WavefrontFileExtension = ".obj";

    // Ref to spatial mapping
    public GameObject spatialMapping;
    private SpatialMappingManager spatialMappingManager;

    // Ref to spatial understanding
    public GameObject spatialUnderstanding;
    private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh;

    // Ref to scan manager (management of spatial understanding)
    public GameObject mappingOrchestrator;
    private ScanManager scanManager;
    private SpatialMappingObserver surfaceObserver;

    //////////////////

    // UI-Elements with values (exept buttons)
    
    public Dropdown dropDownSliceGeometry;

    public UnityEngine.UI.Toggle toggleSlice;
    public bool isSlicingEnabled = false;

    public bool isAnyUIObjectInFocus = false;
    
    public enum SelectedSliceGeometry { Triangle, Rectangle, Circle, Polygon };
    public SelectedSliceGeometry selectedSliceGeometry;
    /// /////////////////////////////////////////
    public string data;
    //ref to meshSaver
    public List<GameObject> meshobjects;
    private string textName;  // Meshdateiname

    private List<Mesh> meshFilters;

    private List<MeshFilter> m;

    public InputField field; // für die Texteingabe

    private bool su, sm;
    // Use this for initialization
    void Start()
    {
        // Init ref to spatial mapping
        spatialMappingManager = spatialMapping.GetComponent<SpatialMappingManager>();

        // Init ref to spatial understanding
        spatialUnderstandingCustomMesh = spatialUnderstanding.GetComponent<SpatialUnderstandingCustomMesh>();

        // Init ref to scan manager (management of spatial understanding)
        scanManager = mappingOrchestrator.GetComponent<ScanManager>();
        spatialMappingManager.DrawVisualMeshes = false;
        //drawSpatialMapping = false;
        //drawSpatialUnderstanding = false;
        surfaceObserver = gameObject.GetComponent<SpatialMappingObserver>();

        selectedSliceGeometry = SelectedSliceGeometry.Triangle;

        su = false;
        sm = false;
       
    }


    // Update is called once per frame
    void Update()
    {
        // rufe die Methode ScanUpdate... auf um die Scanstates zu abchecken
        scanManager.ScanstateUpdate();
        // greife auf und speichere die Meshes vom SpatialUnderstanding in meshFilters
        meshFilters = SpatialMappingManager.Instance.GetMeshes();
        m = SpatialMappingManager.Instance.GetMeshFilters();
       // meshFilters = GetComponent<MeshFilter>().m;
    }

    /*  public void HandleSpatialMapping (bool drawSpatialMapping) {

          if (drawSpatialMapping) {


              spatialMappingManager.DrawVisualMeshes = true;
              spatialMappingManager.StartObserver();


      // drawSpatialMapping = false;
  } else {
              spatialMappingManager.DrawVisualMeshes = false;
             // drawSpatialMapping = true;
          }
      }



      public void HandleSpatialUnderstanding (bool drawSpatialUnderstanding) {
          if (drawSpatialUnderstanding) {
              spatialUnderstandingCustomMesh.DrawProcessedMesh = true;
             // drawSpatialUnderstanding = false;
          } else {
              spatialUnderstandingCustomMesh.DrawProcessedMesh = false;
           //   drawSpatialUnderstanding = true;
          }
      }*/
    public void HandleSliceGeometry()
    {
        // Index:
        // 0 = Triangle
        // 1 = Rectangle
        // 2 = Circle
        // 3 = Polygon
        int currentDropDownIndex = dropDownSliceGeometry.value;

        switch (currentDropDownIndex)
        {
            case 0: // NONE
                    //   Debug.Log("Triangle");
                    //   Debug.Log(currentDropDownIndex);
                selectedSliceGeometry = SelectedSliceGeometry.Triangle;
                break;

            case 1: // Triangle
                    //  Debug.Log("Rectangle");
                    //      Debug.Log(currentDropDownIndex);
                selectedSliceGeometry = SelectedSliceGeometry.Rectangle;
                break;

            case 2: // Circle
                     Debug.Log("Circle");
                        Debug.Log(currentDropDownIndex);
                selectedSliceGeometry = SelectedSliceGeometry.Circle;
                break;

            case 3: // Polygon
                    //   Debug.Log("Polygon");
                    //  Debug.Log(currentDropDownIndex);
                selectedSliceGeometry = SelectedSliceGeometry.Polygon;
                break;

            default:
                currentDropDownIndex = 0;
                break;
        }
    }

    public void HandleToggleSlice()
    {
        if (toggleSlice.isOn)
            isSlicingEnabled = true;
        else
            isSlicingEnabled = false;
    }
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    //Methoden von mir
    ///////
    ///
    /// 
    /// 
    /// 
    /// Umgeänderte Methoden
    /// 
    /// 

  

    //Scan Anhalten
    public void buttonScanAnhalten() {
        if (sm) { 
            SpatialMappingStarten_Stoppen(false);
            Debug.Log("sm deaktiv");
        }

        else if (su)
        {
            UnderstandingFortsetzenUndUnderstandingStoppen(false);
            Debug.Log("su deaktiv");
        }

        else if (su && sm)
        {
            SpatialMappingStarten_Stoppen(false);
            UnderstandingFortsetzenUndUnderstandingStoppen(false);
            Debug.Log("beide deaktiv");
        }

        else
        {
            Debug.Log("Fehler beim ScanAnhalten");
        }

    }

    public void buttonScanFortsetzen()
    {
        if (sm)
        {
            SpatialMappingStarten_Stoppen(true);
            Debug.Log("sm aktiv");
        }

        else if (su)
        {
            UnderstandingFortsetzenUndUnderstandingStoppen(true);
            Debug.Log("su aktiv");
        }

        else if (su && sm)
        {
            SpatialMappingStarten_Stoppen(true);
            UnderstandingFortsetzenUndUnderstandingStoppen(true);
            Debug.Log("beide aktiv");
        }

        else
        {
            Debug.Log("Fehler beim ScanFortsetzen");
        }
    }


    /// <summary>
    /// ///////////// SpatialMapping
    /// </summary>
    /// <param name="starten"></param>
    public void SpatialMappingStarten_Stoppen(bool starten)
    {
        spatialMappingManager.DrawVisualMeshes = true;

        if (starten)
        {
            //MappingObserver starten (falls nicht autoStart in Unity gesetzt)
            spatialMappingManager.StartObserver();
            sm = true;
        }

        else
        {
            // Der SurfaceObserver wird angewiesen das Updating vom SpatialMapping mesh zu unterlassen 
            spatialMappingManager.StopObserver();
            sm = false;
        }
    }

  

    //Befielt den SurfaceObserver zu stoppen und alle meshes zu cleanen
    public void AnwendungBeenden()
    {
        scanManager.BClicked();
        //spatialMappingManager.CleanupObserver();
    }
    /// <summary>
    /// ////////////////////////////////////////
    /// SpatialUnderstanding
    /// </summary>
    public void UnderstandingStarten()
    {
        // rufe Starten aus der Klasse ScanManager
        scanManager.SpatialUnderstandingStart();
        su = true;
    }

    private void UnderstandingFortsetzenUndUnderstandingStoppen(bool starten)
    {
        spatialUnderstandingCustomMesh.DrawProcessedMesh = starten;
        su = starten;
    }

    /*private void UnderstandingStoppen()
    {
        // rufe stoppen aus der Klasse ScanManager
        //scanManager.BClicked();
        spatialUnderstandingCustomMesh.DrawProcessedMesh = false;
        su = false;

    }*/
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Aus der Testklasse RoomMeshSaver vom Holotoolkit umgeschriebene und angepasste Methoden

    /// <summary>
    /// Write single mesh to the stream passed in.
    /// </summary>
    /// <param name="meshFilter">Mesh to be serialized.</param>
    /// <param name="stream">Stream to write the mesh into.</param>
    /// <param name="offset">Index offset for handling multiple meshes in a single stream.</param>
    private static void SerializeMesh(Mesh mesh, TextWriter stream, ref int offset)
    {
        //Mesh mesh = meshFilter.sharedMesh;
        // Write vertices to .obj file. Need to make sure the points are transformed so everything is at a single origin.
        foreach (Vector3 vertex in mesh.vertices)
        {
            stream.WriteLine(string.Format("v {0} {1} {2}", -vertex.x, vertex.y, vertex.z));
        }

        // Write normals. Need to transform the direction.
        foreach (Vector3 normal in mesh.normals)
        {
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
                    indices[i + 2] + 1 + offset,
                    indices[i + 1] + 1 + offset,
                    indices[i + 0] + 1 + offset));
            }
        }

        offset += mesh.vertices.Length;
    }


    /// <summary>
    /// Saves meshes without any modifications during serialization.
    /// </summary>
    /// <param name="fileName">Name of the file, without path and extension.</param>
    public static void SaveMeshesToWavefront(string fileName, IEnumerable<Mesh> meshes)
    {
        //if (!MakeExportDirectory())
        //  {
        // EditorUtility.DisplayDialog(ExportDialogErrorTitle, "Failed to create export directory.", "Ok");

        // return;
        // }
        //string v = SerializeMeshes(meshes);
        /*  string filePath = Path.Combine(Application.persistentDataPath, fileName + WavefrontFileExtension);
          using (StreamWriter stream = new StreamWriter(filePath))
          {
              //stream.Write(v);
          }*/

        string path = Path.Combine(Application.persistentDataPath, fileName + WavefrontFileExtension);
        using (TextWriter writer = File.CreateText(path))
        {
            // TODO write text here 
            writer.Write(SerializeMeshes(meshes));
        }
    }

    public static string SerializeMeshes(IEnumerable<Mesh> meshes)
    {
       
            // Serialize and write the meshes to the file.
        byte[] data = SimpleMeshSerializer.Serialize(meshes);
            //strea.Write(data, 0, data.Length);
            //strea.Flush();
        



        StringWriter stream = new StringWriter();
        int offset = 0;
        foreach (var mesh in meshes)
        {
            SerializeMesh(mesh, stream, ref offset);
        }
        return stream.ToString();
    }

    //Name verwirrend, wird noch umgeändert, praktisch dafür da, dass Mesh in obj abzuspeichern
    public void getMeshObject()
    {
        textName = field.text;
        SaveMeshesToWavefront(textName, meshFilters);
        Debug.Log("mesh gespeichert" + textName);
    }
    //Momentan funktioniert die Texteingabe auf der Hololens nicht, deshalb diese Methode, die dafür sorgt, dass der Dateiname nicht Null ist
    
    public void meshSpeichernErzwingen()
    {
        data = SerializeMeshFilters(m);
        // scanManager.BClicked();
        //textName = field.text;
        //  SaveMeshesToWavefront(textName, meshFilters);
        // Debug.Log("mesh gespeichert b " + textName);

#if WINDOWS_UWP
        WriteData();
#endif

    }

#if WINDOWS_UWP

           
async
	void WriteData () {
	
	//Get local folder
	StorageFile file = await Windows.Storage.DownloadsFolder.CreateFileAsync("shkelloample" + WavefrontFileExtension);
	FileIO.WriteTextAsync(file,data);



}
 

#endif












    public static string SerializeMeshFilters(IEnumerable<MeshFilter> meshes)
   {
       StringWriter stream = new StringWriter();
       int offset = 0;
       foreach (var mesh in meshes)
       {
           SerializeMeshFilter2(mesh, stream, ref offset);
       }
       return stream.ToString();
   }
   public static void SerializeMeshFilter2(MeshFilter meshFilter, TextWriter stream, ref int offset)
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

}
