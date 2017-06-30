using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public string tileName;

    
    public enum tileTag { Wall, Plain, Forrest, Fort };
    public tileTag Tag;
    tileTag ogTag;

    public int movementCost;
    public int dodgeBonus;
    public int defenseBonus;
    public int heal;

    public int x;
    public int y;

    GameObject[,,] mapRef;


    // Use this for initialization
    void Start()
    {
        mapRef = GameObject.Find("Director").GetComponent<GridMap>().MAP;
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        ogTag = Tag;
    }

    // Update is called once per frame
    void Update()
    {
        if (mapRef[x, y, 1] != null && mapRef[x, y, 1].tag == "Red Team")
        { Tag = tileTag.Wall; }
        else
        { Tag = ogTag; }
    }
}
