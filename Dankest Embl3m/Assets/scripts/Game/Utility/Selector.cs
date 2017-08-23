using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : TacticsBehaviour
{
    [HideInInspector]
    public int x, y;
    Vector3 coordinates;
    Terrain[,] MapRef;
    AudioSource soundMaker;
    InputManager inputs;


    //returns the unit with the same x and y coordinates as the cursor.
    public RPGClass GetUnitAtCursorPosition()
    {
        //Grabs all of the unit objects in the scene
        RPGClass[] units = GameObject.FindObjectsOfType<RPGClass>();

        //the linear search through them until one with matching coordinates are found
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].x == x && units[i].y == y)
            { return units[i]; }
        }

        //return null if no object was found
        return null;
    }

    void sample(TurnManager.TeamColor team, int Turn)
    { }//print(team + " is blue and the current turn"); }

    private void Awake()
    {
        TurnManager tMan = FindObjectOfType<TurnManager>();
        tMan.OnPhaseStart += sample;
    }

    private void Start()
    {
        //snap position
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(x, y);

        //cursor should not show up
        Cursor.visible = false;

        //grab references for methods
        MapRef = FindObjectOfType<Map>().LevelMap;
        soundMaker = GetComponent<AudioSource>();
        inputs = FindObjectOfType<InputManager>();
    }


    private void Update()
    {
        //grab the movement requests from the input manager
        MoveCommand[] commands = inputs.MoveInputHandler();

        //if new requests have been made, execute them
        if (commands.Length > 0)
        {
            int oldx = x, oldy = y;

            //set sensitivity to 1 so that the cursor moves in 1 unit intervals. 
            //Then execute each command, this will create diagonal movement if both vertical and horizontal commands are issued.
            for (int i = 0; i < commands.Length; i++)
            { commands[i].setSensitivity(1); commands[i].Execute(gameObject); }

            //next, check to make sure the cursor stays within bounds
            x = Mathf.RoundToInt(transform.position.x); y = Mathf.RoundToInt(transform.position.y);

            if (x < 0) { x = 0; } else if (x >= MapRef.GetLength(0)) { x = MapRef.GetLength(0) - 1; }
            if (y < 0) { y = 0; } else if (y >= MapRef.GetLength(1)) { y = MapRef.GetLength(1) - 1; }

            transform.position = new Vector3(x, y, 0);

            //then play a little sound if the position got changed
            if (x != oldx || y != oldy)
            { soundMaker.Play(); }
        }
    }

}