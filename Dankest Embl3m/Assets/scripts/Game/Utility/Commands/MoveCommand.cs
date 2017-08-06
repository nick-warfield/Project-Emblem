using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCommand : Command
{
    //variables to adjust position by
    protected float x = 0, y = 0, sens = 0;

    //moves an object along a 2d plane
    public override void Execute(GameObject GameActor)
    {
        //see which axis to adjust
        setIncrements();

        //determine how much to adjust the axis by
        if (sens == 0) { sens = Object.FindObjectOfType<InputManager>().Sensitivity; }
        x *= sens; y *= sens;

        //factor in the position of the object
        x += GameActor.transform.position.x;
        y += GameActor.transform.position.y;

        //update the objects actual position
        GameActor.transform.position = new Vector3(x, y, GameActor.transform.position.z);
    }

    //lets child classes determine which axis to adjust
    protected virtual void setIncrements()
    { x = y = 0f; }

    //let an external script override the standard sensitivity, this will let me move a lot more things than just the camera
    public void setSensitivity(float newSensitivity)
    { sens = newSensitivity; }
}

public class MousePositionCommand : MoveCommand
{
    public override void Execute(GameObject GameActor)
    {
        Vector3 coordinates = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        int x2 = Mathf.RoundToInt(coordinates.x);
        int y2 = Mathf.RoundToInt(coordinates.y);

        GameActor.transform.position = new Vector3(x2, y2, GameActor.transform.position.z);
    }
}

public class DownCommand : MoveCommand
{
    protected override void setIncrements() { x = 0; y = -1; }
}

public class UpCommand : MoveCommand
{
    protected override void setIncrements() { x = 0; y = 1; }
}

public class LeftCommand : MoveCommand
{
    protected override void setIncrements() { x = -1; y = 0; }
}

public class RightCommand : MoveCommand
{
    protected override void setIncrements() { x = 1; y = 0; }
}