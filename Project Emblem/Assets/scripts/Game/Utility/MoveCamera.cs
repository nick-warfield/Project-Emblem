using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //Camera cam;
    //PixelPerfectCamera PixelCam;
    public float Sensitivity = 0.5f, ZoomSpeed = 2f;
    float x, y;
    InputManager inputs;
	
	// Update is called once per frame
	void Update ()
    {
        PanCamera();
        ZoomCamera();
	}

    //Grab Reference to attatched Camera
    private void Start()
    {
        //cam = GetComponent<Camera>();
        //PixelCam = GetComponent<PixelPerfectCamera>();
        inputs = FindObjectOfType<InputManager>();
    }
    
    //Camera Controllers
    void PanCamera()
    {
        MoveCommand[] command = inputs.CameraInputHandler();
        if (command.Length > 0)
        {
            for (int i = 0; i < command.Length; i++)
            { command[i].setSensitivity(inputs.Sensitivity); command[i].Execute(gameObject); }
        }

    }
    void ZoomCamera()
    {
        ZoomCommand zoom = inputs.ZoomInputHandler();
        zoom.setSensitivity(ZoomSpeed);
        zoom.Execute(gameObject);
    }
}
