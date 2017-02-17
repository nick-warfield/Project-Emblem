using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class row
{
    public const int width = 10;
    public GameObject[] rowData = new GameObject[width];
}

public class mapConstructor : MonoBehaviour
{
    public row[] mapLayout;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
