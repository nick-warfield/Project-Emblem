using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : TacticsBehaviour
{
    Terrain[,] MapReference;
    RPGClass SelectedUnit;
    public Terrain[] AvailableTilesForTravel;
    public Terrain[] AvailableTilesForAttack;
    public Terrain[] Path;


    //Updates the path a unit will travel
    Terrain[] UpdateCurrentPath(Terrain NewTile, RPGClass Unit, Terrain[] CurrentPath, Terrain[] AvailableTiles)
    {
        //If the unit passed doesn't exsist, return a null
        if (Unit == null)
        { return null; }
        //Check if the new tile is a tile that could be traveled to
        if (!PathContains(NewTile, AvailableTiles) )
        { return CurrentPath; }
        //Check if the new tile is already on the current path
        else if (PathContains(NewTile, CurrentPath) )
        { return ShortenPath(NewTile, CurrentPath); }
        //Check if the new tile is adjacent to the last tile on the path
        else if (AdjacentTiles(NewTile, CurrentPath[CurrentPath.Length-1]) )
        {
            //Calculate the movement cost needed to add the tile
            int MoveCost = Unit.AdjustTileMovementCost(NewTile);
            for (int i = 0; i < CurrentPath.Length; i++)
            { MoveCost += Unit.AdjustTileMovementCost(CurrentPath[i]); }
            
            //Add the tile if it there is enough movement for it
            if (MoveCost <= Unit.Stats[(int)RPGClass.Stat.Move].dynamicValue)
            {
                Terrain[] NewPath = new Terrain[CurrentPath.Length + 1];

                for (int j = 0; j < NewPath.Length; j++)
                { NewPath[j] = CurrentPath[j]; }
                NewPath[CurrentPath.Length] = NewTile;

                return NewPath;
            }
            //if there is not enough movement to pay for the tile, return a new shorter path
            else
            { return ShortenPath(NewTile, CurrentPath[0]); }
        }
        //If all other checks fail, return the shortest path
        else
        { return ShortenPath(NewTile, CurrentPath[0]); }
    }

    //Adds the Range of the weapon to the movelist
    Terrain[] ExapandMoveListWithWeaponRange(Weapons Weapon, Terrain[] MoveList, Terrain[,] Map)
    {
        List<Terrain> inRange = new List<Terrain> { };
        Vector2 Coordinates1 = Vector2.zero;
        Vector2 Coordinates2 = Vector2.zero;

        for (int i = 0; i < MoveList.Length; i++)
        {
            for (int j = Weapon.minRange; j < Weapon.maxRange; j++)
            {
                for (int k = -j; k < j; k++)
                {
                    Coordinates1 = new Vector2(MoveList[i].x - k, MoveList[i].y + Mathf.Abs(k) - j);
                    Coordinates2 = new Vector2(MoveList[i].x + k, MoveList[i].y - Mathf.Abs(k) + j);
                }
            }

            if (BoundsCheck(Coordinates1, Map) )
            {
                Terrain Tile = Map[Mathf.RoundToInt(Coordinates1.x), Mathf.RoundToInt(Coordinates1.y)];

                if (!PathContains(Tile, inRange.ToArray()) )
                { inRange.Add(Tile); }
            }

            if (BoundsCheck(Coordinates2, Map))
            {
                Terrain Tile = Map[Mathf.RoundToInt(Coordinates2.x), Mathf.RoundToInt(Coordinates2.y)];

                if (!PathContains(Tile, inRange.ToArray()))
                { inRange.Add(Tile); }
            }
        }

        return null;
    }

    public void SetSelectedUnit(RPGClass Unit, Terrain[,] Map)
    {
        SelectedUnit = Unit;
        AvailableTilesForTravel = DijkstraAlgorithm(Unit, Map);
        AvailableTilesForAttack = ExapandMoveListWithWeaponRange((Weapons)Unit.Inventory[0], AvailableTilesForTravel, Map);
        Path = new Terrain[1] { Map[Unit.x, Unit.y] };
    }

    public void DeselectUnit()
    {
        SelectedUnit = null;
        AvailableTilesForTravel = new Terrain[0];
        AvailableTilesForAttack = new Terrain[0];
        Path = new Terrain[0];
    }


	// Use this for initialization
	void Start ()
    {
        MapReference = FindObjectOfType<Map>().TerrainMap;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetButton("Fire1") )
        { print("FIRE"); }

        //Placeholder, Input Manager will generate events or something that will get recieved here
        bool movedetected = false;

        //Basically, I only want to run these calculations when a new tile is suggested to be added. Not every frame as would happen with update
        if (movedetected)
        {
            UpdateCurrentPath(MapReference[SelectedUnit.x, SelectedUnit.y], SelectedUnit, Path, AvailableTilesForTravel);
        }
	}
}
