using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class Slicer : MonoBehaviour//, IInputClickHandler
{

    // Vertices for a rect
    List<Vector3> verts = new List<Vector3>();

    // Line from last to actual pick location
    List<GameObject> interactionLines = new List<GameObject>();
    public Material lineMaterial;

    // Spheres at hit location
    List<GameObject> helperSpheres = new List<GameObject>();
    public Material helperSphereMaterial;
    private float helperSphereSize = 0.05f;

    // Lines for unfinished rect
    List<GameObject> helperLines = new List<GameObject>();

    // Gaze cursor
    public GameObject defaultCursor;
    private AnimatedCursor animatedCurser;

    // UIController script
    public GameObject ui;
    //private Scan uiController;

    // List of all sliced objects
    List<SlicedObject> slicedObjects;

    // Line properties
    private float helperLineWidth = 0.02f;
    public Color helperLineColor = Color.blue;

    // Use this for initialization
   /* void Start()
    {

        // Init ref to UIController
        uiController = ui.GetComponent<UIController>();

        // Init ref to curser (state)
        animatedCurser = defaultCursor.GetComponent<AnimatedCursor>();

        slicedObjects = new List<SlicedObject>();
    }

    public List<SlicedObject> GetSlicedObjects()
    {
        return slicedObjects;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!uiController.isAnyUIObjectInFocus && uiController.isSlicingEnabled)
        {
            // Get hit of hand input by cursor
            Vector3 hit = defaultCursor.transform.position;
            verts.Add(hit);

            switch (uiController.selectedSliceGeometry)
            {
                case UIController.SelectedSliceGeometry.Triangle:
                    // Draw final rect
                    AddRectangle(hit);
                    break;
                case UIController.SelectedSliceGeometry.Rectangle:
                    // Draw final rect
                    AddRectangle(hit);
                    break;
                case UIController.SelectedSliceGeometry.Circle:
                    // Draw final rect
                    AddRectangle(hit);
                    break;
                case UIController.SelectedSliceGeometry.Polygon:
                    // Draw final rect
                    AddRectangle(hit);
                    break;
                default:
                    // Draw final rect
                    AddRectangle(hit);
                    break;
            }
        }
    }

    // Draw rectangle and add sliced object
    private void AddRectangle(Vector3 hit)
    {
        // Draw helper sphere at hit location
        GameObject helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);
        helperSphere.transform.position = hit;
        Renderer helperSphereRenderer = helperSphere.GetComponent<Renderer>();
        helperSphereRenderer.material = helperSphereMaterial;
        helperSpheres.Add(helperSphere);

        // Draw helper line from last hit location to actual hit location
        if (verts.Count >= 2)
        {
            helperLines.Add
            (
                DrawHelperLine
                (
                    verts[verts.Count - 2],
                    verts[verts.Count - 1],
                    helperLineColor,
                    helperLineWidth
                )
            );
        }

        // Draw the final rect
        if (verts.Count == 4)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                // Draw rect
                if (i <= verts.Count - 2)
                {
                    DrawLine(verts[i], verts[i + 1], helperLineColor, helperLineWidth);
                }
                else
                {
                    // Close rect
                    DrawLine(verts[i], verts[0], helperLineColor, helperLineWidth);

                    // Delete vertices for next rect
                    verts = new List<Vector3>();

                    // Delete helper spheres
                    foreach (GameObject hsp in helperSpheres)
                        Destroy(hsp);
                    helperSpheres.Clear();

                    // Delte helper lines
                    foreach (GameObject line in helperLines)
                        Destroy(line);
                    helperLines.Clear();

                    // Create new sliced object and add it to list
                    slicedObjects.Add
                    (
                        new SlicedObject
                        (
                            verts,                                  // anker points
                            SlicedObject.GeometryType.Rectangle     // geometry type
                        )
                    );

                    //// Draw helperspheres again on final rect edges
                    //for (int j = 0; j < verts.Count; j++)
                    //{
                    //    helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //    helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);
                    //    helperSphere.transform.position = new Vector3(verts[j].x, verts[j].y, verts[j].z);
                    //    helperSphereRenderer = helperSphere.GetComponent<Renderer>();
                    //    helperSphereRenderer.material = helperSphereMaterial;
                    //}
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (uiController.isSlicingEnabled)
        {
            switch (uiController.selectedSliceGeometry)
            {
                case UIController.SelectedSliceGeometry.Triangle:
                    // Draw final rect
                    UpdateRectangle();
                    break;
                case UIController.SelectedSliceGeometry.Rectangle:
                    // Draw final rect
                    UpdateRectangle();
                    break;
                case UIController.SelectedSliceGeometry.Circle:
                    // Draw final rect
                    UpdateRectangle();
                    break;
                case UIController.SelectedSliceGeometry.Polygon:
                    // Draw final rect
                    UpdateRectangle();
                    break;
                default:
                    // Draw final rect
                    UpdateRectangle();
                    break;
            }
        }
    }

    private void UpdateRectangle()
    {
        // Destroy interactionLine
        for (int i = 0; i < interactionLines.Count; i++)
            Destroy(interactionLines[i]);
        interactionLines.Clear();

        // Draw helper lines
        if (animatedCurser.CursorState == HoloToolkit.Unity.InputModule.Cursor.CursorStateEnum.InteractHover)
        {
            if (verts.Count > 0)
            {
                interactionLines.Add
                (
                    DrawHelperLine
                    (
                        verts[verts.Count - 1],
                        defaultCursor.transform.position,
                        helperLineColor,
                        helperLineWidth
                    )
                );
            }

            if (verts.Count == 3)
            {
                interactionLines.Add
                (
                    DrawHelperLine
                    (
                        verts[0],
                        defaultCursor.transform.position,
                        helperLineColor,
                        helperLineWidth
                    )
                );
            }
        }
    }

    // https://answers.unity.com/questions/1533185/render-lines.html
    private void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth)
    {
        GameObject myLine = new GameObject();
        LineRenderer lr;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        lr = myLine.GetComponent<LineRenderer>();
        lr.material = lineMaterial; //new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.material.color = color;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.useWorldSpace = true;


        // var mesh = new Mesh();
        // mesh.name = "My lines";
        // mesh.vertices = new Vector3[] {
        //      start, end
        // };
        // mesh.colors = new Color[] {
        //      Color.red, Color.red
        // };
        // mesh.SetIndices(new int[] { 0, 1 }, MeshTopology.Lines, 0, true);
        // GameObject g = new GameObject();
        // g.AddComponent<MeshFilter>();
        // g.GetComponent<MeshFilter>().mesh = mesh;//sharedMesh = mesh;
        // g.AddComponent<MeshRenderer>();
        //// g.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));


        //GameObject gameObject = new GameObject();
        //gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();
        //Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        //mesh.Clear();
        //mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(4, 3, 0)};
        //mesh.SetIndices(new int[] { 0, 1 }, MeshTopology.Lines, 0, true);
    }

    private GameObject DrawHelperLine(Vector3 start, Vector3 end, Color color, float lineWidth)
    {
        GameObject myLine = new GameObject();
        LineRenderer lr;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        lr = myLine.GetComponent<LineRenderer>();
        lr.material = lineMaterial; // new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.material.color = color;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.useWorldSpace = true;

        return myLine;
    }*/
}

