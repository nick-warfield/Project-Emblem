using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    

public class InputManager : MonoBehaviour
{
    [HideInInspector]
    public float LeftVertical, LeftHorizontal, XboxTriggers, XboxRightHorizontal, XboxRightVertical, PS4RightHorizontal, PS4RightVertical;

    public float Sensitivity;
    public KeyCode[] MoveButton, PanButton, ZoomButton, FaceButton;


    //Declare all of my commands since I will be calling them a bunch
    protected MoveCommand left = new LeftCommand();
    protected MoveCommand right = new RightCommand();
    protected MoveCommand up = new UpCommand();
    protected MoveCommand down = new DownCommand();
    protected ZoomCommand zoom = new ZoomCommand();


    public virtual MoveCommand[] CameraInputHandler()
    {
        List<MoveCommand> pans = new List<MoveCommand> { };

        //Check controller inputs and then add them to the lise
        if (Input.GetAxis("Xbox Right Horizontal") > 0) { pans.Add(right); }
        if (Input.GetAxis("Xbox Right Horizontal") < 0) { pans.Add(left) ; }
        if (Input.GetAxis("Xbox Right Vertical") > 0) { pans.Add(up); }
        if (Input.GetAxis("Xbox Right Vertical") < 0) { pans.Add(down); }

        //return list if there is something so that controller inputs override keyboard inputs
        if (pans.Count > 0) { return pans.ToArray(); }

        //check keyboard inputs here
        if (Input.GetKey(PanButton[0])) { pans.Add(right); }
        if (Input.GetKey(PanButton[1])) { pans.Add(left); }
        if (Input.GetKey(PanButton[2])) { pans.Add(up); }
        if (Input.GetKey(PanButton[3])) { pans.Add(down); }

        //return whatever here
        return pans.ToArray();
    }

    //The same function as above but looks for different buttons, disallows 2 inputs along the same axis, and uses a timer.
    float timeStamp = 0;
    public virtual MoveCommand[] MoveInputHandler()
    {
        List<MoveCommand> pans = new List<MoveCommand> { };

        if (Time.time >= timeStamp)
        {
            timeStamp = Time.time + 0.075f;

            //Check controller inputs and then add them to the lise
            if (Input.GetAxis("Left Horizontal") > 0) { pans.Add(right); }
            else if (Input.GetAxis("Left Horizontal") < 0) { pans.Add(left); }
            if (Input.GetAxis("Left Vertical") > 0) { pans.Add(up); }
            else if (Input.GetAxis("Left Vertical") < 0) { pans.Add(down); }

            //return list if there is something so that controller inputs override keyboard inputs
            if (pans.Count > 0) { return pans.ToArray(); }

            //check keyboard inputs here
            if (Input.GetKey(MoveButton[0])) { pans.Add(right); }
            else if (Input.GetKey(MoveButton[1])) { pans.Add(left); }
            if (Input.GetKey(MoveButton[2])) { pans.Add(up); }
            else if (Input.GetKey(MoveButton[3])) { pans.Add(down); }
        }

        //return whatever here
        return pans.ToArray();
    }


    public virtual ZoomCommand ZoomInputHandler()
    {
        //this command needs to get called every frame for lerping. Setting the zoomValue will adjust the size being zoomed to

        //check controller inputs here
        if (Input.GetAxis("Xbox Triggers") != 0)
        { zoom.newZoomValue(Input.GetAxis("Xbox Triggers")); }

        //check mouse inputs here
        else
        { zoom.newZoomValue(Input.mouseScrollDelta.y / 5); }

        //return the zoom command. Zoomvalue will be 0 if the input checks failed
        return zoom;
    }

    //Show Axis Values in the inspector
    void DisplayAxisValues ()
    {
        LeftHorizontal = Input.GetAxis("Left Horizontal");
        LeftVertical = Input.GetAxis("Left Vertical");
        XboxTriggers = Input.GetAxis("Xbox Triggers");
        XboxRightHorizontal = Input.GetAxis("Xbox Right Horizontal");
        XboxRightVertical = Input.GetAxis("Xbox Right Vertical");
        PS4RightHorizontal = Input.GetAxis("PS4 Right Horizontal");
        PS4RightVertical = Input.GetAxis("PS4 Right Vertical");
    }
}
