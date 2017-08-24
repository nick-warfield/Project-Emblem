using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayWorldPosition : MonoBehaviour
{
    public float x, y, z;

	// Use this for initialization
	void Start ()
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
