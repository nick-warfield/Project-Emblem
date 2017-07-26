using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Camera cam, thisCam;


	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);

        thisCam.orthographicSize = cam.orthographicSize;
	}
}
