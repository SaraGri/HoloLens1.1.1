using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class test : MonoBehaviour {

	public void createFile() {
		string path = Path.Combine(Application.persistentDataPath, "hallohieristderleere.txt");
		using (TextWriter writer = File.CreateText(path))
		{
			// TODO write text here 
			writer.WriteLine("hallo, hier konntest du ein text schreiben :)");
		}
	}
}
