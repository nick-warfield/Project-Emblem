using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnit : RPGClass
{
    //Change the way flying units travel as compared to other units
    public override int AdjustTileMovementCost(Terrain Tile)
    {
        if (Tile.Type != Terrain.Tile.Wall)
        { return 1; }
        else
        { return Tile.MovementCost; }
    }
}
