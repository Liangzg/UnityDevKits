using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class TestCommondLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public static void TestStart()
    {
        Debug.Log("Test Commond Line Start");

        //AssetDatabase.CreateAsset(this , "Assets/Editor/Test/creta.txt");
        string path = Path.Combine(Application.dataPath, "Editor/creata.txt");
        File.CreateText(path);
    }
}
