﻿using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

#if WINDOWS_UWP
using System;
using Windows.Storage;
using Windows.System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
#endif

public class TestExporter : MonoBehaviour
{

    public bool onlySelectedObjects = false;
    public bool applyPosition = true;
    public bool applyRotation = true;
    public bool applyScale = true;
    public bool generateMaterials = true;
    public bool exportTextures = true;
    public bool splitObjects = true;
    public bool autoMarkTexReadable = false;
    public bool objNameAddIdNum = false;

    //public bool materialsUseTextureName = false;

    private string versionString = "v2.0";
    private string lastExportFolder;
    private string helpString;

    //bool StaticBatchingEnabled() {
    //    PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
    //    if (playerSettings == null) {
    //        return false;
    //    }
    //    SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
    //    SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
    //    for (int i = 0; i < batchingSettings.arraySize; i++) {
    //        SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
    //        if (batchingArrayValue == null) {
    //            continue;
    //        }
    //        IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
    //        if (batchingEnumerator == null) {
    //            continue;
    //        }
    //        while (batchingEnumerator.MoveNext()) {
    //            SerializedProperty property = (SerializedProperty)batchingEnumerator.Current;
    //            if (property != null && property.name == "m_StaticBatching") {
    //                return property.boolValue;
    //            }
    //        }
    //    }
    //    return false;
    //}

    //void OnWizardUpdate() {
    //    helpString = "Aaro4130's OBJ Exporter " + versionString;
    //}

    Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }
    Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    //void OnWizardCreate() {
    //    if (StaticBatchingEnabled() && Application.isPlaying) {
    //        EditorUtility.DisplayDialog("Error", "Static batching is enabled. This will cause the export file to look like a mess, as well as be a large filesize. Disable this option, and restart the player, before continuing.", "OK");
    //        goto end;
    //    }
    //    if (autoMarkTexReadable) {
    //        int yes = EditorUtility.DisplayDialogComplex("Warning", "This will convert all textures to Advanced type with the read/write option set. This is not reversible and will permanently affect your project. Continue?", "Yes", "No", "Cancel");
    //        if (yes > 0) {
    //            goto end;
    //        }
    //    }
    //    string lastPath = EditorPrefs.GetString("a4_OBJExport_lastPath", "");
    //    string lastFileName = EditorPrefs.GetString("a4_OBJExport_lastFile", "unityexport.obj");
    //    string expFile = EditorUtility.SaveFilePanel("Export OBJ", lastPath, lastFileName, "obj");
    //    if (expFile.Length > 0) {
    //        var fi = new System.IO.FileInfo(expFile);
    //        EditorPrefs.SetString("a4_OBJExport_lastFile", fi.Name);
    //        EditorPrefs.SetString("a4_OBJExport_lastPath", fi.Directory.FullName);
    //        Export(expFile);
    //    }
    //    end:;
    //}

    public void Export(string exportPath, MeshFilter[] meshFiltersSU, MeshFilter[] meshFiltersSM)
    {

        //init stuff
        Dictionary<string, bool> materialCache = new Dictionary<string, bool>();
        var exportFileInfo = new System.IO.FileInfo(exportPath);
        lastExportFolder = exportFileInfo.Directory.FullName;
        string baseFileName = System.IO.Path.GetFileNameWithoutExtension(exportPath);
        //EditorUtility.DisplayProgressBar("Exporting OBJ", "Please wait.. Starting export.", 0);

        //get list of required export things
        MeshFilter[] sceneMeshes;
        //sceneMeshes = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

        // Export spatial understanding mesh only
        if (meshFiltersSU != null && meshFiltersSM == null)
        {
            sceneMeshes = meshFiltersSU;
        }
        // Export spatial mapping mesh only
        else if (meshFiltersSU == null && meshFiltersSM != null)
        {
            sceneMeshes = meshFiltersSM;
        }
        // Export spatial mapping mesh and spatial understanding mesh
        else
        {
            sceneMeshes = new MeshFilter[meshFiltersSM.Length + meshFiltersSU.Length];
            Array.Copy(meshFiltersSM, sceneMeshes, meshFiltersSM.Length);
            Array.Copy(meshFiltersSU, 0, sceneMeshes, meshFiltersSM.Length, meshFiltersSU.Length);
        }

        if (Application.isPlaying)
        {
            foreach (MeshFilter mf in sceneMeshes)
            {
                MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    if (mr.isPartOfStaticBatch)
                    {
                        //EditorUtility.ClearProgressBar();
                        //EditorUtility.DisplayDialog("Error", "Static batched object detected. Static batching is not compatible with this exporter. Please disable it before starting the player.", "OK");
                        return;
                    }
                }
            }
        }

        //work on export
        StringBuilder sb = new StringBuilder();
        StringBuilder sbMaterials = new StringBuilder();
        sb.AppendLine("# Export of " + Application.loadedLevelName);
        sb.AppendLine("# from Aaro4130 OBJ Exporter " + versionString);
        if (generateMaterials)
        {
            sb.AppendLine("mtllib " + baseFileName + ".mtl");
        }
        float maxExportProgress = (float)(sceneMeshes.Length + 1);
        int lastIndex = 0;
        for (int i = 0; i < sceneMeshes.Length; i++)
        {
            string meshName = sceneMeshes[i].gameObject.name;
            float progress = (float)(i + 1) / maxExportProgress;
            //EditorUtility.DisplayProgressBar("Exporting objects... (" + Mathf.Round(progress * 100) + "%)", "Exporting object " + meshName, progress);
            MeshFilter mf = sceneMeshes[i];
            MeshRenderer mr = sceneMeshes[i].gameObject.GetComponent<MeshRenderer>();

            if (splitObjects)
            {
                string exportName = meshName;
                if (objNameAddIdNum)
                {
                    exportName += "_" + i;
                }
                sb.AppendLine("g " + exportName);
            }
            if (mr != null && generateMaterials)
            {
                Material[] mats = mr.sharedMaterials;
                for (int j = 0; j < mats.Length; j++)
                {
                    Material m = mats[j];
                    if (!materialCache.ContainsKey(m.name))
                    {
                        materialCache[m.name] = true;
                        //sbMaterials.Append(MaterialToString(m));
                        sbMaterials.AppendLine();
                    }
                }
            }

            //export the meshhh :3
            Mesh msh = mf.sharedMesh;
            int faceOrder = (int)Mathf.Clamp((mf.gameObject.transform.lossyScale.x * mf.gameObject.transform.lossyScale.z), -1, 1);

            //export vector data (FUN :D)!
            foreach (Vector3 vx in msh.vertices)
            {
                Vector3 v = vx;
                if (applyScale)
                {
                    v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale);
                }

                if (applyRotation)
                {

                    v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                }

                if (applyPosition)
                {
                    v += mf.gameObject.transform.position;
                }
                v.x *= -1;
                sb.AppendLine("v " + v.x + " " + v.y + " " + v.z);
            }
            foreach (Vector3 vx in msh.normals)
            {
                Vector3 v = vx;

                if (applyScale)
                {
                    v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale.normalized);
                }
                if (applyRotation)
                {
                    v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                }
                v.x *= -1;
                sb.AppendLine("vn " + v.x + " " + v.y + " " + v.z);

            }
            foreach (Vector2 v in msh.uv)
            {
                sb.AppendLine("vt " + v.x + " " + v.y);
            }

            for (int j = 0; j < msh.subMeshCount; j++)
            {
                if (mr != null && j < mr.sharedMaterials.Length)
                {
                    string matName = mr.sharedMaterials[j].name;
                    sb.AppendLine("usemtl " + matName);
                }
                else
                {
                    sb.AppendLine("usemtl " + meshName + "_sm" + j);
                }

                int[] tris = msh.GetTriangles(j);
                for (int t = 0; t < tris.Length; t += 3)
                {
                    int idx2 = tris[t] + 1 + lastIndex;
                    int idx1 = tris[t + 1] + 1 + lastIndex;
                    int idx0 = tris[t + 2] + 1 + lastIndex;
                    if (faceOrder < 0)
                    {
                        sb.AppendLine("f " + ConstructOBJString(idx2) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx0));
                    }
                    else
                    {
                        sb.AppendLine("f " + ConstructOBJString(idx0) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx2));
                    }

                }
            }

            lastIndex += msh.vertices.Length;
        }

        //write to disk
        //System.IO.File.WriteAllText(exportPath, sb.ToString());
        //if (generateMaterials) {
        //    System.IO.File.WriteAllText(exportFileInfo.Directory.FullName + "\\" + baseFileName + ".mtl", sbMaterials.ToString());
        //}

#if WINDOWS_UWP
        WriteData(sb);
#endif

    }

#if WINDOWS_UWP
    async
        void WriteData (StringBuilder sb) {

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile;
            sampleFile = await storageFolder.CreateFileAsync("test.obj", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, sb.ToString());
        }    
#endif

    private string ConstructOBJString(int index)
    {
        string idxString = index.ToString();
        return idxString + "/" + idxString + "/" + idxString;
    }

    // Update is called once per frame
    void Update()
    {

    }
}