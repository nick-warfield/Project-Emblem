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
        //Check if tile is the last tile on the path
        else if (NewTile == CurrentPath[CurrentPath.Length - 1])
        { return CurrentPath; }
        //Check if the new tile is already on the current path
        else if (PathContains(NewTile, CurrentPath) )
        { return ShortenPath(NewTile, CurrentPath); }
        //Check if the new tile is adjacent to the last tile on the path
        else if (AdjacentTiles(NewTile, CurrentPath[CurrentPath.Length-1]) )
        {
            //Calculate the movement cost needed to add the tile
            int MoveCost = Unit.AdjustTileMovementCost(NewTile);
            for (int i = 1; i < CurrentPath.Length; i++)
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
        {
            //print("all checks failed");
            Terrain[] temp = ShortenPath(NewTile, CurrentPath[0]);

            if (temp == null) { return CurrentPath; }
            else { return temp; }
        }
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
    Terrain[] GetRangeAtPoint(Weapons Weapon, Terrain Point, Terrain[,] Map)
    {
        List<Terrain> inRange = new List<Terrain> { };
        Vector2 Coordinates1 = Vector2.zero;
        Vector2 Coordinates2 = Vector2.zero;

        for (int j = Weapon.minRange; j <= Weapon.maxRange; j++)
        {
            for (int k = -j; k < j; k++)
            {
                Coordinates1 = new Vector2(Point.x - k, Point.y + Mathf.Abs(k) - j);
                Coordinates2 = new Vector2(Point.x + k, Point.y - Mathf.Abs(k) + j);

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

        return inRange.ToArray();
    }

    //Spawns ui tiles to show where i can travel to
    //Need to do these another way, they are very performance intensive because they are spawning a bunch of things every frame
    private void DisplayIndicator(GameObject Indicator, Terrain[] Tiles)
    {
        for (int i = 0; i < Tiles.Length; i++)
        {
            GameObject ind = Instantiate(Indicator, new Vector3(Tiles[i].x, Tiles[i].y, -0.125f), transform.rotation);
            ind.GetComponent<DestroyOnBoolNotReset>().flagChecked = true;
        }
    }
    private void DisplayIndicator(GameObject Indicator, Terrain Tile)
    {
        GameObject ind = Instantiate(Indicator, new Vector3(Tile.x, Tile.y, -0.125f), transform.rotation);
        ind.GetComponent<DestroyOnBoolNotReset>().flagChecked = true;
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

            //on right click, bring up stats
            if (Input.GetButtonDown("Fire2"))
            {
                RPGClass temp = Cursor.GetUnitAtCursorPosition();

                if (temp != null)
                {
                    getUnitInfoAdvanced menu = FindObjectOfType<getUnitInfoAdvanced>();
                    menu.PassStats(temp);
                }
                else
                { getUnitInfoAdvanced menu = FindObjectOfType<getUnitInfoAdvanced>(); menu.CloseMenu(); }

            }
        }

        //If a unit is selected, these actions are available
        else
        { StateMachine(SelectedUnit); }
	}


    //A little state machine to handle different actions depending on what is being done with the selected unit
    private void StateMachine(RPGClass Unit)
    {
        switch (Unit.CurrentState)
        {
            case Character._State.Selected:
                //Put up some indicators for movement, current pathm and attackable tiles
                Path = UpdateCurrentPath(LevelMap[Cursor.x, Cursor.y], Unit, Path, AvailableTilesForTravel);

                DisplayIndicator(Indicators[1], AvailableTilesForTravel);
                DisplayIndicator(Indicators[0], Path);
                DisplayIndicator(Indicators[2], RedTiles);

                //If the player wants to confirm movement, move the unit to the last tile on the path
                if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit"))
                {
                    RPGClass temp = Cursor.GetUnitAtCursorPosition();
                    if (temp == null || temp == SelectedUnit)
                    {
                        Unit.CurrentState = Character._State.SelectingAction;

                        Unit.x = Path[Path.Length - 1].x; Unit.y = Path[Path.Length - 1].y;
                        AvailableTilesForAttack = GetRangeAtPoint(Unit.CombatParameters.EquipedWeapon, Path[Path.Length - 1], LevelMap);
                    }
                }

                //If the player wants to cancel the selection, deselect the unit
                else if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Cancel"))
                { DeselectUnit(Character._State.Idle); }

                break;

            case Character._State.SelectingAction:
                //Put up some indicators for what is in range
                DisplayIndicator(Indicators[2], AvailableTilesForAttack);


                //If the player wants to confirm an action
                if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit"))
                {
                    RPGClass TempUnit = Cursor.GetUnitAtCursorPosition();

                    if (TempUnit != null)
                    {
                        //If the player clicks the selected unit they will begin waiting
                        if (TempUnit == Unit) { DeselectUnit(Character._State.Waiting); }

                        //If the player clicks an enemy unit, combat will begin
                        else if (!TempUnit.CompareTag(SelectedUnit.tag) && PathContains(LevelMap[TempUnit.x, TempUnit.y], AvailableTilesForAttack))
                        {
                            SelectedUnit.CurrentState = Character._State.InCombat;

                            //Set up the combat Manager with new stats
                            CombatManager cManager = gameObject.GetComponent<CombatManager>();
                            cManager.InitializeCombatParameters(Unit, TempUnit, LevelMap[Unit.x, Unit.y], LevelMap[TempUnit.x, TempUnit.y]);
                            //cManager.StartCombat();
                        }
                    }
                }


                //If the player wants to cancel their move and put the unit back at the starting tile
                else if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Cancel"))
                {
                    Unit.x = Path[0].x; Unit.y = Path[0].y;
                    SetSelectedUnit(Unit, LevelMap);    //If I don't recalculate all of the pathfinding, the game crashes
                }

                break;

            case Character._State.InCombat:
                getCombatInfo menu = FindObjectOfType<getCombatInfo>();
                CombatManager cMan = GetComponent<CombatManager>();

                RPGClass tempUnit;
                if (Unit != cMan.LeftSide.Unit) { tempUnit = cMan.LeftSide.Unit; }
                else { tempUnit = cMan.RightSide.Unit; }
                DisplayIndicator(Indicators[2], LevelMap[tempUnit.x, tempUnit.y]);

                //End combat after it has been run all the way
                if (!cMan.SimulateCombat)
                {
                    if (menu.transform.parent.name == menu.DisabledView.name)
                    { Unit.CurrentState = Character._State.Waiting; }

                    //on submit action, start combat (only while not already in combat)
                    else if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit"))
                    { cMan.StartCombat(); }

                    //on cancel action, go back 1 state
                    else if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Cancel"))
                    { Unit.CurrentState = Character._State.SelectingAction; menu.CloseMenu(); }
                }

                break;

            default:
                //If the selected unit gets put in an unintended state, deselect them and make them wait
                print("Defaulted. Previous State: " + Unit.CurrentState.ToString());
                DeselectUnit(Character._State.Waiting);
                break;
        }
    }
}
