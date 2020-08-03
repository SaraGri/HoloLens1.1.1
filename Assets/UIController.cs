using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class UIController : MonoBehaviour {
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
    
    // public bool drawSpatialMapping;
    //public bool drawSpatialUnderstanding;

    // Use this for initialization
    void Start() {
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





        //Änderungen von mir
        manager = spatialMapping.GetComponent<SpatialMappingManager>();

    }

    



    //Meins
    //Für das Einschalten des Observers, damit Netz nicht automatisch starten
    public SpatialMappingManager manager;
    //Obseverobjekt
    //private SpatialMappingObserver surfaceObserver2;
   // public SpatialMappingObserver SurfaceObserver { get { return surfaceObserver2; } }









    // Update is called once per frame
    void Update () {
		
	}

   

    public void HandleSpatialMapping (bool drawSpatialMapping) {
         
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
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    //Methoden von mir

    public void SpatialMappingStarten_Stoppen(bool starten)
    {
        //Mesh visuell anzeigen lassen
        spatialMappingManager.DrawVisualMeshes = true;

        if (starten)
        {
           
            //MappingObserver starten
            spatialMappingManager.StartObserver();
        }
       // Der SurfaceObserver wird angewiesen das Updating vom SpatialMapping mesh zu unterlassen 
        else
        {
            spatialMappingManager.StopObserver();
        }
    }

    //Befielt den SurfaceObserver zu stoppen und alle meshes zu cleanen
    public void AnwendungBeenden()
    {
        spatialMappingManager.CleanupObserver();
    }
    
}




