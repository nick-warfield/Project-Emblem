using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [HideInInspector]
    public int x, y;
    float x2, y2;
    Vector3 coordinates;
    Terrain[,] MapRef;


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
        x2 = x = Mathf.RoundToInt(transform.position.x);
        y2 = y = Mathf.RoundToInt(transform.position.y);

        transform.position = new Vector3(x, y, -2);

        Cursor.visible = false;

        MapRef = FindObjectOfType<Map>().LevelMap;
    }


    private void Update()
    {
        coordinates = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y) );

        x2 = coordinates.x;
        y2 = coordinates.y;

        x = Mathf.RoundToInt(x2);
        y = Mathf.RoundToInt(y2);



        if (x < 0) { x = 0; } else if (x >= MapRef.GetLength(0)) { x = MapRef.GetLength(0) - 1; }
        if (y < 0) { y = 0; } else if (y >= MapRef.GetLength(1)) { y = MapRef.GetLength(1) - 1; }

        transform.position = new Vector3(x, y, -2);
    }

}