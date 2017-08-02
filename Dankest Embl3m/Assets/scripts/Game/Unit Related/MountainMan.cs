using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainMan : RPGClass
{
    //Let the mountain guys walk on mountain tiles
    public override int AdjustTileMovementCost(Terrain Tile)
    {
        if (Tile.Type == Terrain.Tile.Mountain)
        { return 3; }
        else
        { return Tile.MovementCost; }
    }
}
