using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slice : MonoBehaviour, IInputClickHandler
{
	private List<MeshFilter> meshfilters;
	public List <GameObject> bounds;
	public GameObject c, b;

	//public GameObject toogle;
	public Toggle slicetoggle;
	// Gaze cursor
	public GameObject defaultCursor;
	private AnimatedCursor animatedCurser;
	private float helperSphereSize = 0.01f;
	private List<Vector3> verts;
	// Use this for initialization
	void Start () {
		//slicetoogle = toogle.GetComponent<Toggle>();
		// Init ref to curser (state)
		animatedCurser = defaultCursor.GetComponent<AnimatedCursor>();
		verts = new List<Vector3>();
		
		bounds = new List<GameObject>();
		c = GameObject.Find("Cube");
		bounds.Add(c);
		//GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//sphere.transform.position = new Vector3(0, 1.5f, 0);
		//sphere.transform.localScale = new Vector3(2, 2, 6);
		//slicetoogle = slicetoogle.

	}

	public void OnInputClicked(InputClickedEventData eventData)
	{
		//throw new NotImplementedException();
		//Andys
		//https://answers.unity.com/questions/1362117/images-blocking-buttons.html
		Vector3 hitPoint = defaultCursor.transform.position;
		if (slicetoggle.isOn) {
			verts.Add(hitPoint);
			GameObject helperSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			helperSphere.transform.position = hitPoint;
			helperSphere.transform.localScale = new Vector3(helperSphereSize, helperSphereSize, helperSphereSize);

			Renderer helperSphereRenderer = helperSphere.GetComponent<Renderer>();
			Debug.Log(hitPoint.ToString());
			//helperSphereRenderer.material = sp;
		}

	}

	// Update is called once per frame
	void Update () {
		if (verts.Count > 2) { 
			b = createCube();
			bounds.Add(b);
		}
		
	}


    public void RemoveVertices(IEnumerable<GameObject> boundingObjects)
	{
		RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
		if (removeVerts != null && removeVerts.enabled)
		{
			removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
			Debug.Log("remove ngjklhjhjk");
		}
	}

	public void re() {
		RemoveVertices(bounds);
		Debug.Log("remove wird durchlafen");
	}

	public GameObject createCube() {
		//https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		float dist = 1;
		if (verts.Count > 2) {
			 dist = Vector3.Distance(verts[0], verts[1]);
		}
		cube.transform.position = verts[0];
		cube.transform.localScale = new Vector3(dist,dist,6);
		// https://answers.unity.com/questions/185268/adding-a-box-collider-to-an-object-in-csharp-scrip.html
		//https://docs.unity3d.com/ScriptReference/SphereCollider.html 
		var boxCollider2 = cube.AddComponent<BoxCollider>();
		return cube;
	}

  
}
