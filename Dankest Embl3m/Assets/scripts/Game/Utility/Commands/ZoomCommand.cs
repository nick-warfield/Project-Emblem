using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCommand : Command
{
    //declare z and sens so they can be adjusted by other scripts
    float z = 0, sens = 4f;

    public override void Execute(GameObject GameActor)
    {
        //Grab the cameras I need to adjust, this assumes that the gameobject passed in has both cameras attatched
        PixelPerfectCamera PixelCam = GameActor.GetComponent<PixelPerfectCamera>();
        Camera ActualCam = GameActor.GetComponent<Camera>();

        //Check if a new zoom level has been requested
        if (z != 0)
        {
            //The pixel camera will only allow orthographic sizes that don't distort pixels. So by adjusting z, a desired size is being requested not set.
            PixelCam.targetCameraHalfHeight -= Mathf.Clamp(z, -1, 1);
            PixelCam.targetCameraHalfHeight = Mathf.Clamp(PixelCam.targetCameraHalfHeight, 2, 10);
        }

        //This returns a valid orthographic size to be lerped to. (I can probably figure out a way to not call this every frame)
        float newZoom = PixelCam.adjustCameraFOV(true);

        //Now we lerp to the closest valid size. this can be smoothed out better, but good enough for now.
        ActualCam.orthographicSize = Mathf.Lerp(ActualCam.orthographicSize, newZoom, Time.smoothDeltaTime * sens);
    }

    //This lets an outside script request a new zoom level
    public void newZoomValue(float increment)
    { z = increment; }

    //This lets an outside script override the default sensitivity
    public void setSensitivity(float newSensitivity)
    { sens = newSensitivity; }
}
