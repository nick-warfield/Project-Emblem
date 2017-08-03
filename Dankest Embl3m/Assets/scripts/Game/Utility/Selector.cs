﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [HideInInspector]
    public int x, y;
    Vector3 coordinates;
    Terrain[,] MapRef;
    AudioSource soundMaker;
    InputManager inputs;


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


    private void Start()
    {
        transform.position = new Vector3(x, y);

        Cursor.visible = false;

        MapRef = FindObjectOfType<Map>().LevelMap;

        soundMaker = GetComponent<AudioSource>();
        inputs = FindObjectOfType<InputManager>();
    }


    private void Update()
    {
        MoveCommand[] commands = inputs.MoveInputHandler();
        if (commands.Length > 0)
        {
            for (int i = 0; i < commands.Length; i++)
            { commands[i].setSensitivity(1); commands[i].Execute(gameObject); }

            x = Mathf.RoundToInt(transform.position.x); y = Mathf.RoundToInt(transform.position.y);

            if (x < 0) { x = 0; } else if (x >= MapRef.GetLength(0)) { x = MapRef.GetLength(0) - 1; }
            if (y < 0) { y = 0; } else if (y >= MapRef.GetLength(1)) { y = MapRef.GetLength(1) - 1; }

            transform.position = new Vector3(x, y, 0);

            soundMaker.Play();
        }
        /*
        coordinates = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y) );

        x2 = coordinates.x;
        y2 = coordinates.y;

        x = Mathf.RoundToInt(x2);
        y = Mathf.RoundToInt(y2);



        if (x < 0) { x = 0; } else if (x >= MapRef.GetLength(0)) { x = MapRef.GetLength(0) - 1; }
        if (y < 0) { y = 0; } else if (y >= MapRef.GetLength(1)) { y = MapRef.GetLength(1) - 1; }


        if (transform.position.x != x || transform.position.y != y) { soundMaker.Play(); }

        transform.position = new Vector3(x, y, -2);
        */
    }

}