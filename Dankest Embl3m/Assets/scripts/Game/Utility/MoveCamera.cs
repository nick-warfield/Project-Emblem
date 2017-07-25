using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //Camera cam;
    public float Sensitivity = 0.5f;
    float x, y;


	// Use this for initialization
	void Start ()
    {
        //cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        x = transform.position.x; y = transform.position.y;

        x += (System.Math.Sign(Input.GetAxis("Horizontal")) * Sensitivity);
        y += (System.Math.Sign(Input.GetAxis("Vertical")) * Sensitivity);

        transform.position = new Vector3(x, y, transform.position.z);
	}
}
