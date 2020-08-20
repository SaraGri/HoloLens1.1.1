using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine.UI;
//using UnityScript.Steps;

public class Slicer : MonoBehaviour, IInputClickHandler
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
    private UIController uiController;

    // List of all sliced objects
    List<SlicedObject> slicedObjects;

    // Line properties
    private float helperLineWidth = 0.02f;
    public Color helperLineColor = Color.blue;

    // von mir geändert
    public float radius; // für den Kreisradius
    private int indexp; // für Polygonindex
    private bool pFinish; // für den finishingProzess vom Polygon
    public Toggle pfinish;
    // Use this for initialization
    void Start()
    {

        // Init ref to UIController
        uiController = ui.GetComponent<UIController>();

        // Init ref to curser (state)
        animatedCurser = defaultCursor.GetComponent<AnimatedCursor>();

        slicedObjects = new List<SlicedObject>();

        //Init ref to finish Toggle

        radius = 0.00f;
        indexp = 2;
        pFinish = false;
        // int currentDropDownIndex = dropDownSliceGeometry.value;
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
                    AddTriangle(hit);
                    break;
                case UIController.SelectedSliceGeometry.Rectangle:
                    // Draw final rect
                    AddRectangle(hit);
                    break;
                case UIController.SelectedSliceGeometry.Circle:
                    // Draw final rect
                    AddCircle(hit);
                    //  radius += 0.03f;
                    break;
                case UIController.SelectedSliceGeometry.Polygon:
                    // Draw final rect
                    AddPolygon(hit);
                    // indexp++;
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
        //Zeichenfunktion allgemeiner machen
        drawhelperline(hit, 4, SlicedObject.GeometryType.Rectangle); // bei n = 4 zeichnet nur bis zu 3 Punkte, bei n 3 nur 2 punkte, Count fängt bei 1 an zu zählen, wieder audf 4 geändert, da 5 punkte gezeichet
    }

    // meins
    private void AddTriangle(Vector3 hit)
    {
        drawhelperline(hit, 3, SlicedObject.GeometryType.Triangle);
    }

    private void AddCircle(Vector3 hit)
    {
        //drawhelperline(hit, 1, SlicedObject.GeometryType.Circle);
        updateCircle(hit);

    }

    private void AddPolygon(Vector3 hit)
    {
        /*// n speichert die Größe/Size des Vektors
         int n = verts.Count + 1;
         int m = 0;
         //Zwischenspeicher für den nahestendene Vektor zu 0
         float temp = 0;
         temp = Vector3.Distance(verts[0], verts[n]);
         // https://docs.unity3d.com/ScriptReference/Vector3.Distance.html
         float distanze = 0;
       //  Vector3 abstandVP;
         for (int i = 1; i < n - 1; i++) {
            // abstandVP.x = verts[0].x - verts[i].x;
            // abstandVP.y = verts[0].y - verts[i].y;
            // abstandVP.z = verts[0].z - verts[i].z;
             distanze = Vector3.Distance(verts[0], verts[i]);
             if (distanze < temp) {
                 temp = distanze;
                 m = i;
             }
         }*/
        //drawhelperline(hit, indexp, SlicedObject.GeometryType.Polygon); // von m auf n geändert, nichts passiert
        // Draw helper sphere at hit location
        GameObject helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);
        helperSphere.transform.position = hit;
        Renderer helperSphereRenderer = helperSphere.GetComponent<Renderer>();
        helperSphereRenderer.material = helperSphereMaterial;
        helperSpheres.Add(helperSphere);

        // Draw helper line from last hit location to actual hit location
        /*  if (verts.Count >= 2)
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
          }*/


    }

    private void updatePoly(bool finish)
    {

        //  GameObject helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);

        //int x, y, z;

        // Draw helper lines
        /* if (animatedCurser.CursorState == HoloToolkit.Unity.InputModule.Cursor.CursorStateEnum.InteractHover)
          {
              if (verts.Count > 0)
              {

                  // https://www.reddit.com/r/Unity3D/comments/6tefif/convert_float_to_int/
                  /* x = (int)defaultCursor.transform.position.x;
                   y = (int)defaultCursor.transform.position.y;
                   z = (int)defaultCursor.transform.position.z;

                  // https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Texture3D.SetPixel.html
                  //Texture3D.SetPixel( x,  y,  z, helperLineColor);
                  helperSphere.transform.position = defaultCursor.transform.position;
                  Renderer helperSphereRenderer = helperSphere.GetComponent<Renderer>();
                  helperSphereRenderer.material = helperSphereMaterial;
                  helperSpheres.Add(helperSphere);
              }


          }*/





        if (finish)
        {
            /*
             // n speichert die Größe/Size des Vektors
             int n = verts.Count;
             Debug.Log("n =   " + n);
             int m = 0;
             //Zwischenspeicher für den nahestendene Vektor zu 0
             float temp = 0;
             temp = Vector3.Distance(verts[verts.Count - 1], verts[0]);
             // https://docs.unity3d.com/ScriptReference/Vector3.Distance.html
             float distanz = 0;

             //Vector3 abstandVP;
            for (int j = 0; j < verts.Count - 1; j++)
             {
                 //DrawLine(verts[j], verts[j + 1], helperLineColor, helperLineWidth);

                 for (int i = 0; i < verts.Count; i++)
                 {
                     //abstandVP.x = verts[0].x - verts[i].x;
                     // abstandVP.y = verts[0].y - verts[i].y;
                     // abstandVP.z = verts[0].z - verts[i].z;
                     distanz = Vector3.Distance(verts[m], verts[i]);
                     if (distanz < temp)
                     {
                         temp = distanz;
                         m = i;

                     }

                 }
                 DrawLine(verts[j], verts[m], helperLineColor, helperLineWidth);
                 //https://stackoverflow.com/questions/59137577/how-to-remove-a-vector-from-a-vector-3-list-in-c-sharp-for-unity
                 //helperSpheres.RemoveAt(m);
                 //helperSpheres.RemoveAt(j);

             }

                // DrawLine(verts[verts.Count-2], verts[verts.Count - 1], helperLineColor, helperLineWidth);
             //drawhelperline(verts[m],verts.Count-1, SlicedObject.GeometryType.Polygon);
             pfinish.enabled = false;
             */
            //Letzten Punkt möchte ich nicht gezeichnet bekommen
            for (int i = 0; i < verts.Count - 2; i++)
            {
                DrawLine(verts[i], verts[i + 1], helperLineColor, helperLineWidth);
            }
            DrawLine(verts[0], verts[verts.Count - 2], helperLineColor, helperLineWidth);

            slicedObjects.Add
                   (
                       new SlicedObject
                       (
                           verts,                                  // anker points
                           SlicedObject.GeometryType.Polygon     // geometry type //abgeändert von mir zu Type statt nur zu Viereck
                       )
                   );

        }
        else
        {


        }
    }

    private void drawhelperline(Vector3 hit, int n, SlicedObject.GeometryType type)
    { //Parameter erweitert um n und type
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
        if (verts.Count == n)
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
                            type     // geometry type //abgeändert von mir zu Type statt nur zu Viereck
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
        uiController.HandleSliceGeometry(); // von mir hinzugefügt damit auswahl geupdatet wird
        if (uiController.isSlicingEnabled)
        {
            //uiController.HandleSliceGeometry();// in unity geändert
            switch (uiController.selectedSliceGeometry)
            {
                case UIController.SelectedSliceGeometry.Triangle:
                    // Draw final rect
                    UpdateFigure(3); // von mir geändert, von 2 auf3
                    break;
                case UIController.SelectedSliceGeometry.Rectangle:
                    // Draw final rect
                    UpdateFigure(4); // von mir geändert, von 3 auf 5
                    break;
                case UIController.SelectedSliceGeometry.Circle:
                    // Draw final rect
                    //UpdateRectangle();


                    break;
                case UIController.SelectedSliceGeometry.Polygon:
                    // Draw final rect
                    // updatePolygon();

                    updatePoly(pfinish.isOn);
                    // updatePolygon();
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
        {
            Destroy(interactionLines[i]);
        }
        interactionLines.Clear();
        // geschwewifte Klammern ergänzt

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
    //https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html Kreis
    //Kopiert und verändert vomn mir
    private void UpdateFigure(int n) // Parameter n ergänzt
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

            if (verts.Count == n)
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


    //meins 
    public void updateCircle(Vector3 hit)
    {
        GameObject helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);
        helperSphere.transform.position = hit;
        Renderer helperSphereRenderer = helperSphere.GetComponent<Renderer>();
        helperSphereRenderer.material = helperSphereMaterial;
        helperSpheres.Add(helperSphere);
        if (verts.Count == 2)
        {
            //Vektordistanz berechnen, dies wird der Umfang des Kreises
            var distanz = Vector3.Distance(verts[verts.Count - 1], verts[verts.Count - 2]);
            radius = distanz / 2;
            DrawCircle(radius, hit);
        }
    }
    public void DrawCircle(float radius, Vector3 hit)
    {
        var segments = 360;
        GameObject myLine = new GameObject();
        LineRenderer line;
        myLine.AddComponent<LineRenderer>();
        line = myLine.GetComponent<LineRenderer>();
        line.startWidth = helperLineWidth;
        line.endWidth = helperLineWidth;
        line.positionCount = segments + 1;
        // line.transform.position = hit;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];
        // Quaternion rotation = Quaternion.Euler(90, 0, 0);

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            // https://answers.unity.com/questions/20354/is-it-possible-to-use-a-rotation-on-a-vector3.html

            points[i] = new Vector3(0, Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius);

            //  points[i].x += hit.x;
            //points[i].y *= 2*hit.y;
            //  points[i].z += hit.z;
            // zum drehen mal 90
            //  points[i] = rotation * points[i];
            points[i] += hit.normalized;
            //  points[i] = Vector3.forward;
            verts.Add(points[i]);
        }

        line.SetPositions(points);

        if (pfinish.isOn)
        {
            slicedObjects.Add
                   (
                       new SlicedObject
                       (
                           verts,                                  // anker points
                           SlicedObject.GeometryType.Circle     // geometry type //abgeändert von mir zu Type statt nur zu Viereck
                       )
                   );
            verts = new List<Vector3>();
        }



    }


    private void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth)
    {
        GameObject myLine2 = new GameObject();
        LineRenderer lr2;
        myLine2.transform.position = start;
        myLine2.AddComponent<LineRenderer>();
        lr2 = myLine2.GetComponent<LineRenderer>();
        lr2.material = lineMaterial; //new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr2.material.color = color;
        lr2.startColor = color;
        lr2.endColor = color;
        lr2.startWidth = lineWidth;
        lr2.endWidth = lineWidth;
        lr2.SetPosition(0, start);
        lr2.SetPosition(1, end);
        lr2.useWorldSpace = true;


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
    }
}
