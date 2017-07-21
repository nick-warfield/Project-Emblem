using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Map
{
    RPGClass SelectedUnit;
    public Selector Cursor;
    public GameObject[] Indicators;

    [HideInInspector] public Terrain[] AvailableTilesForTravel;
    [HideInInspector] public Terrain[] AvailableTilesForAttack;
    [HideInInspector] public Terrain[] Path;
    [HideInInspector] public Terrain[] RedTiles;


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

                for (int j = 0; j < CurrentPath.Length; j++)
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
            for (int j = Weapon.minRange; j <= Weapon.maxRange; j++)
            {
                for (int k = -j; k < j; k++)
                {
                    Coordinates1 = new Vector2(MoveList[i].x - k, MoveList[i].y + Mathf.Abs(k) - j);
                    Coordinates2 = new Vector2(MoveList[i].x + k, MoveList[i].y - Mathf.Abs(k) + j);

                    if (BoundsCheck(Coordinates1, Map))
                    {
                        Terrain Tile = Map[Mathf.RoundToInt(Coordinates1.x), Mathf.RoundToInt(Coordinates1.y)];

                        if (!PathContains(Tile, inRange.ToArray()))
                        { inRange.Add(Tile); }
                    }

                    if (BoundsCheck(Coordinates2, Map))
                    {
                        Terrain Tile = Map[Mathf.RoundToInt(Coordinates2.x), Mathf.RoundToInt(Coordinates2.y)];

                        if (!PathContains(Tile, inRange.ToArray()))
                        { inRange.Add(Tile); }
                    }

                }
            }
        }

        return inRange.ToArray();
    }

    //Spawns ui tiles to show where i can travel to
    private void DisplayIndicator(GameObject Indicator, Terrain[] Tiles)
    {
        for (int i = 0; i < Tiles.Length; i++)
        {
            GameObject ind = Instantiate(Indicator, new Vector3(Tiles[i].x, Tiles[i].y), transform.rotation);
            ind.GetComponent<DestroyOnBoolNotReset>().flagChecked = true;
        }
    }


    public void SetSelectedUnit(RPGClass Unit, Terrain[,] Map)
    {
        if (Unit == null) { return; }

        SelectedUnit = Unit;
        SelectedUnit.CurrentState = Character._State.Selected;

        AvailableTilesForTravel = DijkstraAlgorithm(Unit, Map);
        AvailableTilesForAttack = ExapandMoveListWithWeaponRange((Weapons)Unit.Inventory[0], AvailableTilesForTravel, Map);
        Path = new Terrain[1] { Map[Unit.x, Unit.y] };
        
        List<Terrain> tempRed = new List<Terrain> { };
        for (int i = 0; i < AvailableTilesForAttack.Length; i++)
        {
            if (!PathContains(AvailableTilesForAttack[i], AvailableTilesForTravel) )
            { tempRed.Add(AvailableTilesForAttack[i]); }
        }
        RedTiles = tempRed.ToArray();
        
    }
    public void DeselectUnit(Character._State NewState)
    {
        SelectedUnit.CurrentState = NewState;
        SelectedUnit = null;

        AvailableTilesForTravel = new Terrain[0];
        AvailableTilesForAttack = new Terrain[0];
        Path = new Terrain[0];
        RedTiles = new Terrain[0];
    }


    // Update is called once per frame
    void Update ()
    {
        //I've pretty much started up a state machine here, I'll hold off on fleshing it out until I make my own Input manager and figure out the turn stuff

        //If no unit is selected, these actions become available
        if (SelectedUnit == null)
        {
            //On input, check to see if a unit can be selected
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit") )
            { SetSelectedUnit(Cursor.GetUnitAtCursorPosition(), LevelMap); }
        }

        //If a unit is selected, these actions are available
        else
        {
            //On deselect action, put unit back at the start of the path and then deselect it
            if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Cancel"))
            {
                SelectedUnit.x = Path[0].x; SelectedUnit.y = Path[0].y;
                DeselectUnit(Character._State.Idle);
            }

            //If a deselect action is not requested
            else
            {
                //Update the path only while the character has not been moved
                if (SelectedUnit.CurrentState == Character._State.Selected)
                { Path = UpdateCurrentPath(LevelMap[Cursor.x, Cursor.y], SelectedUnit, Path, AvailableTilesForTravel); }

                //Put up some indicators for movement, current pathm and attackable tiles
                DisplayIndicator(Indicators[1], AvailableTilesForTravel);
                DisplayIndicator(Indicators[0], Path);
                DisplayIndicator(Indicators[2], RedTiles);

                //If a select input is requested do one of the following
                if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit"))
                {
                    //If the unit is in the selected state, set it to the walking state and update it's location
                    if (SelectedUnit.CurrentState == Character._State.Selected)
                    {
                        SelectedUnit.CurrentState = Character._State.Walking;

                        SelectedUnit.x = Path[Path.Length - 1].x;
                        SelectedUnit.y = Path[Path.Length - 1].y;
                    }

                    //If a unit is in the walking state, deselect it and put it in the waiting state since it can no longer be moved this turn
                    else if (SelectedUnit.CurrentState == Character._State.Walking)
                    {
                        DeselectUnit(Character._State.Waiting);
                    }
                }
            }
        }
	}
}
