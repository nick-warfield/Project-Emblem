using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountedUnit : RPGClass
{
    public override int AdjustTileMovementCost(Terrain Tile)
    {
        switch (Tile.Type)
        {
            case Terrain.Tile.Forrest:
            case Terrain.Tile.Fort:
                return 3;

            default:
                return Tile.MovementCost;
        }
    }
}
