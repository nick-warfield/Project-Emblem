using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitController03 : MonoBehaviour
{
    Vector3 startPoint;
    Vector3 endPoint;
    public float speed = 10;
    float increment;
    bool isMoving;

    // Use this for initialization
    void Start()
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
        //manages inputs, lot of code so I made it a function to tidy up
        endPoint = inputManager();

        //the magic that actually moves from point a to point
        if (isMoving) { transform.position = Vector3.Lerp(startPoint, endPoint, increment); }

    }

    //detects an input, then does something depending on the input
    Vector3 inputManager()
    {
        Vector3 newEndPoint;
        Vector3 direct0;
        Vector3 direct1;
        Vector3 direct2;
        Vector3 direct3;
        Vector3 direct4 = direct3 = direct2 = direct1 = direct0 = newEndPoint = endPoint;

        //move up
        if (Input.GetKey("w"))
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            direct0 = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }

        //move down
        if (Input.GetKey("s"))
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            direct1 = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }

        //move left
        if (Input.GetKey("a"))
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            direct2 = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }

        //move right
        if (Input.GetKey("d"))
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            direct3 = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }

        direct4 = direct0 + direct1 + direct2 + direct3;

        newEndPoint = direct4;

        return newEndPoint;
    }
}
