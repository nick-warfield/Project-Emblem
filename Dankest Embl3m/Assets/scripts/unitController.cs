using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitController : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float speed = 7;
    private float increment;
    bool isMoving;

    //bool isUnitSelected = false;
    public GameObject selectedUnit = null;
    public GameObject tempObject;

	// Use this for initialization
	void Start ()
    {
        //initialize starting and ending vectors
        startPoint = endPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
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

        //move selected unit with cursor
        if (selectedUnit != null)
        {
            selectedUnit.transform.position = transform.position;
        }
    }

    //detects the unit being hovered over
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            tempObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") { tempObject = null; }
    }

    //detects an input, then does something depending on the input
    void inputManager ()
    {
        //move up
        if (Input.GetKey("w") && isMoving == false)
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }

        //move down
        else if (Input.GetKey("s") && isMoving == false)
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }

        //move left
        else if (Input.GetKey("a") && isMoving == false)
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }

        //move right
        else if (Input.GetKey("d") && isMoving == false)
        {
            increment = 0;
            isMoving = true;
            startPoint = transform.position;
            endPoint = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }

        //select object, so that only a selected unit may be moved
        else if (Input.GetKey("space") && isMoving == false)
        {
            //select or deselect depending on current state
            if (selectedUnit != null)
            {
                print("if");
                selectedUnit = tempObject;
            }
            else
            {
                print("else");
                selectedUnit = null;
            }
        }

    }
}
