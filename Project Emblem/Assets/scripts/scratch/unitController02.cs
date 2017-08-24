using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitController02 : MonoBehaviour
{
    Vector3 startPoint;
    Vector3 endPoint;
    public float speed = 10;
    float increment;
    bool isMoving;

    // Use this for initialization
    void Start ()
    {
        startPoint = endPoint = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        //detects if the unit is moving, prevents inputs if so, else allows new inputs to be made
        if (increment <= 1 && isMoving == true)
        {
            increment += speed / 100;
        }
        else
        {
            isMoving = false;
        }

        //the magic that actually moves from point a to point
        if (isMoving) { transform.position = Vector3.Lerp(startPoint, endPoint, increment); }

        //manages inputs, lot of code so I made it a function to tidy up
        inputManager();
    }

    //detects an input, then does something depending on the input
    void inputManager()
    {
        //move up
        if (Input.GetKey("w") )
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }

        //move down
        if (Input.GetKey("s") )
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }

        //move left
        if (Input.GetKey("a") )
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }

        //move right
        if (Input.GetKey("d") )
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }
    }
}
