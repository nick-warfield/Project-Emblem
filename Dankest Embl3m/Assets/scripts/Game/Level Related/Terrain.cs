using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    //used to determine how Units will interact with the tile
    public enum Tile { Wall, Plain, Forrest, Fort };
    public Tile Type;

    //Tile Stats, how much an average unit pays to travel it, plus any bonuses the terrain offers
    public int MovementCost;
    public int DodgeBonus;
    public int DefenseBonus;
    public int HealPercent;

    //Used to make referencing easier, just converts transform position into x and y ints.
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    //probably need to update this section when I touch up pathfinding
    [HideInInspector] public float EstimatedMovementCost = Mathf.Infinity, F;
    [HideInInspector] public float G;
    [HideInInspector] public float H = Mathf.Infinity;
    [HideInInspector] public Terrain Parent, parent;


    // Use this for initialization
    void Start()
    {
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
    }
}
