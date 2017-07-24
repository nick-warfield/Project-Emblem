using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    //[HideInInspector]
    public int x, y;
    float x2, y2, offset;
    Vector3 coordinates;


    public RPGClass GetUnitAtCursorPosition()
    {
        RPGClass[] units = GameObject.FindObjectsOfType<RPGClass>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].x == x && units[i].y == y)
            { return units[i]; }
        }

        return null;
    }


    private void Start()
    {
        x2 = x = Mathf.RoundToInt(transform.position.x);
        y2 = y = Mathf.RoundToInt(transform.position.y);

        offset = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        transform.position = new Vector3(x, y, -2);

        Cursor.visible = false;
    }


    private void Update()
    {
        coordinates = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x - offset, Input.mousePosition.y - offset) );

        x2 = coordinates.x;
        y2 = coordinates.y;

        x = Mathf.RoundToInt(x2);
        y = Mathf.RoundToInt(y2);

        transform.position = new Vector3(x, y, -2);
    }

}