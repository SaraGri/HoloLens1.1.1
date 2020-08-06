using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ScanManager : MonoBehaviour //Handled den Air Tap Gesture
{
    //Variable für den 3D Text vom Editor, Infomrationen an den User werden dadurch angezeigt
    public TextMesh InstructionTextMesh;
    public Transform FloorPrefab;
    public TestExporter exporter;
    bool once = true;
    // Use this for initialization
   public void SpatialUnderstandingStart()
    {
        //SpatialUnderstanding.Instance.UnderstandingSourceMesh.
        InputManager.Instance.PushFallbackInputHandler(this.gameObject);
        //rufe ich auf, um den Scan Process zu starten
        SpatialUnderstanding.Instance.RequestBeginScanning();
        SpatialUnderstanding.Instance.ScanStateChanged += ScanStateChanged;
    }

    //Informiert werden, wenn der Scan beendet ist
    private void ScanStateChanged()
    {
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
        {
            LogSurfaceState();
        }
        else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {

            //meine Änderung
           // InstanciateObjectOnFloor();
            //InstanciateObjectOnSurface();
           // InstanciateObjectOnWall();

            //exporter.Export("test.obj"); ; // InstanciateObjectOnFloor();
        }
    }

    private void OnDestroy()
    {
       // SpatialUnderstanding.Instance.ScanStateChanged -= ScanStateChanged;
    }

    // Update is called once per frame
    // Hier checke ich, ob Scan beendet ist, checke die Scanstates
    public void ScanstateUpdate()
    {

        switch (SpatialUnderstanding.Instance.ScanState)
        {
            case SpatialUnderstanding.ScanStates.None:
                break;
            case SpatialUnderstanding.ScanStates.ReadyToScan:
                break;
            case SpatialUnderstanding.ScanStates.Scanning:
                this.LogSurfaceState();
                break;
            case SpatialUnderstanding.ScanStates.Finishing:
                this.InstructionTextMesh.text = "State: Finishing Scan";
                //if(once) {
                //    exporter.Export("test.obj");
                //    once = false;
                //}
                break;
            case SpatialUnderstanding.ScanStates.Done:
                this.InstructionTextMesh.text = "State: Scan DONE";
                //this.LogSurfaceState();
//#if WINDOWS_UWP
  //              Windows.StorageFolder storageFolder = KnownFolders.Object3D;
    //            StorageFile file = await storageFolder.CreateFileAsync("sample.png", CreationCollisionOption.ReplaceExisting);
//#endif
                break;
            default:
                break;
        }
    }

    private void LogSurfaceState()
    {
        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) != 0)
        {
            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            this.InstructionTextMesh.text = string.Format("TotalSurfaceArea: {0:0.##} - WallSurfaceArea: {1:0.##} - HorizSurfaceArea: {2:0.##}", stats.TotalSurfaceArea, stats.WallSurfaceArea, stats.HorizSurfaceArea);

            var rayResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();
            //this.InstructionTextMesh.text = rayResult.SurfaceType.ToString();
        }
    }

    //Handelt den Air Tap um den Scanprocess zu beenden
    public void BClicked()
    {
        this.InstructionTextMesh.text = "Requested Finish Scan";
        Debug.Log("beenden");
        //Scan beenden, take all the scanned surfaces and merge them together
        SpatialUnderstanding.Instance.RequestFinishScan();
    }

    private void InstanciateObjectOnFloor()
    {
        const int QueryResultMaxCount = 512;

        SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];

        var minLengthFloorSpace = 0.25f;
        var minWidthFloorSpace = 0.25f;

        var resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
        var locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(minLengthFloorSpace, minWidthFloorSpace, _resultsTopology.Length, resultsTopologyPtr);

        if (locationCount > 0)
        {
            Instantiate(this.FloorPrefab, _resultsTopology[0].position, Quaternion.LookRotation(_resultsTopology[0].normal, Vector3.up));

            this.InstructionTextMesh.text = "Placed the hologram";
        }
        else
        {
            this.InstructionTextMesh.text = "I can't found the enough space to place the hologram.";
        }
    }

    public void ScanstateAnzeigen() {
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        { this.InstructionTextMesh.text = "State: Scan DONE";
            Debug.Log("scan fertig");
            //return true;
        }
        //return false;

    }

    //public void Test() { }
}