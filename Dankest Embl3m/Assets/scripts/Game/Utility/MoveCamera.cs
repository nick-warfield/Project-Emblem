using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    Camera cam;
    PixelPerfectCamera PixelCam;
    public float Sensitivity = 0.5f;
    float x, y;
	
	// Update is called once per frame
	void Update ()
    {
        PanCamera();
        ZoomCamera();
	}

    //Grab Reference to attatched Camera
    private void Start()
    {
        cam = GetComponent<Camera>();
        PixelCam = GetComponent<PixelPerfectCamera>();
    }
    
    //Camera Controllers
    void PanCamera()
    {
        x = transform.position.x; y = transform.position.y;

        x += (System.Math.Sign(Input.GetAxis("Horizontal")) * Sensitivity);
        y += (System.Math.Sign(Input.GetAxis("Vertical")) * Sensitivity);

        transform.position = new Vector3(x, y, transform.position.z);
    }
    void ZoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            PixelCam.targetCameraHalfHeight -= Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
            PixelCam.targetCameraHalfHeight = Mathf.Clamp(PixelCam.targetCameraHalfHeight, 2, 10);
        }

        float newZoom = PixelCam.adjustCameraFOV(true);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.smoothDeltaTime * 4f);
    }
}
