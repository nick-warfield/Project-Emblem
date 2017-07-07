using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class movementManager : MonoBehaviour
{
    GameObject[,,] mapRef;
    //enum axis { x, y };

    
    //Return all possible selections for moving a unit
    protected Terrain[] PossibleDestinations (Terrain Start, int Movement, GameObject[,,] map)
    {
        //I'll probably reference these a lot, so i'll store them locally
        int xStart = Start.x;
        int yStart = Start.y;
        int F = 0;

        //start a list to store my tiles, i'll be checking this as i go along to prevent duplicates
        List<Terrain> tiles = new List<Terrain>() { Start };


        for (int i = -Movement; i < Movement; i++)
        {
            int k = Movement - Mathf.Abs(i);
            for (int j = -k; j < k; j++)
            {
                tiles.Add(map[xStart+i, yStart+j, 0].GetComponent<Terrain>() );
            }
        }
        
        for (int i = 0; i < tiles.Count; i++) { print(tiles[i].name); }

        /*
        //keep adding new valid neighbors to the list. Then send stuff that has already added neighbors to the begininning
        do
        {
            if (xStart < map.GetLength(0) && map[xStart, yStart, 0].GetComponent<Terrain>().Tag != Terrain.tileTag.Wall && tiles. )
            { tiles.Add(map[xStart, yStart, 0].GetComponent<Terrain>() ); }
        }
        while (tiles.Last<Terrain>() != Start);



        for (int i = 0; i < Movement; i++)
        {
            if (xStart + i < map.GetLength(0) && map[xStart + i, yStart, 0].GetComponent<Terrain>().Tag != Terrain.tileTag.Wall)
            {
                tiles.Add(map[xStart + i, yStart, 0].GetComponent<Terrain>() );
            }


            if (xStart - i > 0)
            {
                tiles.Add(map[xStart - i, yStart, 0].GetComponent<Terrain>() );
            }
        }
        */
        return null;
    }

    //colours and returns abialable tiles a selected unit can move to
    protected List<Terrain> UnitMove (Terrain Start, RPGClass Unit, GameObject[,,] Map)
    {
        //create the list I will eventually return, add the starting tile because of course a unit can go there
        List<Terrain> available = new List<Terrain> { Start };
        int movement = Unit.Stats[11].dynamicValue + 1;

        //temp. Get all the tiles surrounding the start without adjusting for movement cost or obstructions
        for (int i = 0; i < movement; i++)
        {
            for (int j = -i; j < i; j++)
            {
                int k = Mathf.Abs(j) - i;
                int a = Start.x - j;
                int b = Start.y + k;

                for (int doTwice = 0; doTwice < 2; doTwice++)
                {
                    if (a >= 0 && a < Map.GetLength(0) && b >= 0 && b < Map.GetLength(1))
                    {
                        Terrain t = Map[a, b, 0].GetComponent<Terrain>();

                        //if (!available.Contains(t) && t.Tag != Terrain.tileTag.Wall && aStarPathfinding(Start, t, Unit, Map) )
                        //{
                        //    available.Add(Map[a, b, 0].GetComponent<Terrain>());
                        //}
                    }

                    if (doTwice == 0)
                    {
                        k = -Mathf.Abs(j) + i;
                        a = Start.x + j;
                        b = Start.y + k;
                    }
                }

            }
        }


        return available;

        /*
        List<int> coordinates = new List<int> { };

        int x = Location.x;
        int y = Location.y;
        GameObject aUI;
        maxRange++;

        //for unit movement, minrange will always equal 0 since a unit can always at least travel to the tile they are on
        for (int i = minRange; i < maxRange; i++)
        {
            for (int j = -i; j < i; j++)
            {
                //this algorithm does not acount for varying costs of moving across a tile or tiles that can not be crossed
                int k = Mathf.Abs(j) - i;
                aUI = Instantiate(AttackUI, new Vector3(x - j, y + k), transform.rotation);
                aUI.GetComponent<DestroyOnBoolNotReset>().flagChecked = true;

                k = ((x - j) * 1000) + (y + k);
                coordinates.Add(k);


                k = -Mathf.Abs(j) + i;
                aUI = Instantiate(AttackUI, new Vector3(x + j, y + k), transform.rotation);
                aUI.GetComponent<DestroyOnBoolNotReset>().flagChecked = true;

                k = ((x - j) * 1000) + (y + k);
                coordinates.Add(k);
            }
        }

        return coordinates;
        */
    }

    //find the shortest path between two known points
    Terrain[] badDijkstra (Terrain[] OpenTiles, Terrain Start, Terrain End)
    {
        List<Terrain> closedList = new List<Terrain> { Start };     //holds the current shortest path
        List<Terrain> openList = new List<Terrain> { };             //holds the tiles being considered
        Terrain current = Start;                                    //the current tile being referenced
        Terrain temp;

        //keep on looping until a path has been found
        do
        {
            //grab neighbors
            for (int i = 0; i < 4; i++)
            {
                Vector3 tempVec = Vector3.one;
                switch (i)
                {
                    case 0: tempVec = new Vector3(current.x + 1, current.y, 0); break;
                    case 1: tempVec = new Vector3(current.x - 1, current.y, 0); break;
                    case 2: tempVec = new Vector3(current.x, current.y + 1, 0); break;
                    case 3: tempVec = new Vector3(current.x, current.y - 1, 0); break;
                }
                Collider[] c = Physics.OverlapSphere(tempVec, 0.5f);
                if (c[0] != null)
                {
                    temp = c[0].GetComponent<Terrain>();
                    
                    //if the tile is on the possible tile and it is not on the closed list
                    if (OpenTiles.Contains(temp) && !closedList.Contains(temp) )
                    {
                        //if it is already on the open list... do something
                        if (openList.Contains(temp) )
                        {

                        }
                        //otherwise, add it to the open list and update its values for future reference
                        else
                        { openList.Add(temp); temp.parent = current; temp.H = current.H + temp.F; }
                    }
                }
            }
        }
        while (closedList.Last<Terrain>() != End);

        //convert and return array
        return closedList.ToArray();
    }

    //find the shortest path between two know points... (WORK IN PROGRESS)
    Terrain[] DijkstraPath2 (Terrain Start, Terrain End, GameObject[,,] map)
    {
        List<Terrain> visited = new List<Terrain> { Start };    //this list contains the tiles that form the shortest path
        List<Terrain> unvisited = new List<Terrain> { };        //this list contains the tiles being considered to add to the closed list

        Terrain mostRecent = Start;
        Terrain temp;

        Start.H = 0;

        do
        {
            //Add adjacent tiles to the list
            if (mostRecent.x + 1 < map.GetLength(0))   //right tile
            {
                temp = map[mostRecent.x + 1, mostRecent.y, 0].GetComponent<Terrain>();  //so i dont have to use that ugly mapref

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                    }
                }
            }

            if (mostRecent.x - 1 >= 0)      //left tile
            {
                temp = map[mostRecent.x - 1, mostRecent.y, 0].GetComponent<Terrain>();

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                    }
                }
            }

            if (mostRecent.y + 1 < map.GetLength(1))   //above tile
            {
                temp = map[mostRecent.x, mostRecent.y + 1, 0].GetComponent<Terrain>();

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                    }
                }
            }

            if (mostRecent.y - 1 >= 0)      //below tile
            {
                temp = map[mostRecent.x, mostRecent.y - 1, 0].GetComponent<Terrain>();

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                    }
                }
            }

            //sort list so that the first result has the smallest H value
            for (int i = 1; i < unvisited.Count; i++)
            {
                if (unvisited[i].H < unvisited[0].H)
                {
                    temp = unvisited[0];
                    unvisited[0] = unvisited[i];
                    unvisited[i] = temp;
                }
            }

            visited.Add(unvisited[0]);              //add the unvisted tile with the smallest F value to the visited list
            unvisited.Remove(unvisited[0]);         //remove the added tile from the unvisited list
            mostRecent = visited.Last<Terrain>();   //update the mostrecent tile



        } while (unvisited.Count > 0);   //if the cost of moving becomes greater than the avaialable movement, stop looping

        //reset the h values for future pathfindings
        for (int i = 0; i < visited.Count; i++) { visited[i].H = Mathf.Infinity; }

        return visited.ToArray();
    }

    //find the shortest path to every tile in range
    protected Terrain[] DijkstraPath (Terrain Start, RPGClass Unit, GameObject[,,] map)
    {
        List<Terrain> visited = new List<Terrain> { Start };    //this list contains the tiles that form the shortest path
        List<Terrain> unvisited = new List<Terrain> { };        //this list contains the tiles being considered to add to the closed list
        
        int move = Unit.Stats[11].dynamicValue;
        Terrain mostRecent = Start;
        Terrain temp;

        Start.H = 0;

        do
        {
            //Add adjacent tiles to the list
            if (mostRecent.x + 1 < map.GetLength(0) )   //right tile
            {
                temp = map[mostRecent.x + 1, mostRecent.y, 0].GetComponent<Terrain>();  //so i dont have to use that ugly mapref
                temp.F = Unit.GetAdjustedF(temp);

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall && mostRecent.H + temp.F <= move)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp) )
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; temp.parent = mostRecent; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                        temp.parent = mostRecent;
                    }
                }
            }

            if (mostRecent.x - 1 >= 0)      //left tile
            {
                temp = map[mostRecent.x - 1, mostRecent.y, 0].GetComponent<Terrain>();
                temp.F = Unit.GetAdjustedF(temp);

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall && mostRecent.H + temp.F <= move)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; temp.parent = mostRecent; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                        temp.parent = mostRecent;
                    }
                }
            }

            if (mostRecent.y + 1 < map.GetLength(1) )   //above tile
            {
                temp = map[mostRecent.x, mostRecent.y + 1, 0].GetComponent<Terrain>();
                temp.F = Unit.GetAdjustedF(temp);

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall && mostRecent.H + temp.F <= move)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; temp.parent = mostRecent; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                        temp.parent = mostRecent;
                    }
                }
            }

            if (mostRecent.y - 1 >= 0)      //below tile
            {
                temp = map[mostRecent.x, mostRecent.y - 1, 0].GetComponent<Terrain>();
                temp.F = Unit.GetAdjustedF(temp);

                //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                if (!visited.Contains(temp) && temp.Tag != Terrain.tileTag.Wall && mostRecent.H + temp.F <= move)
                {
                    //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                    if (unvisited.Contains(temp))
                    {
                        if (mostRecent.H + temp.F < temp.H)
                        { temp.H = mostRecent.H + temp.F; temp.parent = mostRecent; }
                    }
                    else    //otherwise, add it to the unvisited list
                    {
                        unvisited.Add(temp);
                        temp.H = mostRecent.H + temp.F;
                        temp.parent = mostRecent;
                    }
                }
            }

            //sort list so that the first result has the smallest H value
            for (int i = 1; i < unvisited.Count; i++)
            {
                if (unvisited[i].H < unvisited[0].H)
                {
                    temp = unvisited[0];
                    unvisited[0] = unvisited[i];
                    unvisited[i] = temp;
                }
            }

            visited.Add(unvisited[0]);              //add the unvisted tile with the smallest F value to the visited list
            unvisited.Remove(unvisited[0]);         //remove the added tile from the unvisited list
            mostRecent = visited.Last<Terrain>();   //update the mostrecent tile

        } while (unvisited.Count > 0);   //if the cost of moving becomes greater than the avaialable movement, stop looping

        //reset the h values for future pathfindings
        for (int i = 0; i < visited.Count; i++) { visited[i].H = Mathf.Infinity; }

        return visited.ToArray();
    }

    //adjust current path if the desired tile is open but out of reach... (WORK IN PROGRESS)
    protected Terrain[] UpdatePath (Terrain[] OpenTiles, Terrain[] currentPath, Terrain nextTile, int move, GameObject[,,] map)
    {
        //if the desired tile is not an open tile, ignore it and return the current path
        if (!OpenTiles.Contains<Terrain>(nextTile))
        { return currentPath; }
        else if (currentPath.Contains<Terrain>(nextTile))  //if the current path already has the new tile, shorten it to match
        {
            //create a list that will get returned
            List<Terrain> p = new List<Terrain> { };

            int i = 0;
            do
            {
                //fill out list with tiles from the current path
                p.Add(currentPath[i]);
                i++;
            }
            while (p.Last<Terrain>() != nextTile);  //end the loop once the desired tile has been added

            //convert and return shortened path
            return currentPath; //p.ToArray();
        }
        else
        {
            //calculate the cost of the current path + the cost of adding the new tile to it
            float cost = 0;
            for (int i = 1; i < currentPath.Length; i++)
            { cost += currentPath[i].F; }
            cost += nextTile.F;

            //if the cost is too much, adjust the path to be as short as possible
            if (cost > move)
            {
                //here is where I use aStar pathfinding
                return DijkstraPath2(currentPath[0], nextTile, map);
            }
            //otherwise, just add the new tile to current path
            else
            {
                Terrain[] p = new Terrain[currentPath.Length + 1];
                p[currentPath.Length] = nextTile;
                return p;
            }

        }
    }

    //check new tile, return a valid path if possible
    protected Terrain[] gridPathfindingArray (Terrain[] openTiles, Terrain[] currentPath, Terrain newTile, int maxMove)
    {
        Terrain[] newPath = currentPath;

        //first, make sure the tile is on the open list. If it's not, just end the checks now
        if (!openTiles.Contains<Terrain>(newTile) ) { print("Tile not on openList"); return newPath; }

        //if the newTile is already on the path, shorten the path to that tile.
        //im doing a linear search starting at the end because the most recent tile is the most likely to be a repeat.
        for (int i = 0; i < currentPath.Length; i++)
        {
            int startFromBack = currentPath.Length - 1 - i;     //my reference for the end of the array

            //if the desired tile is already on the array...
            if (newTile == currentPath[startFromBack])
            {
                //print("tile already on path");

                startFromBack++;    //adjust end indicator so that it can set the length of a new array
                newPath = new Terrain[startFromBack];   //create a new array with the shortened length

                //fill out the new array with data from the current path
                for (int j = 0; j < startFromBack; j++)
                { newPath[j] = currentPath[j]; }

                //end the function call (since we have a valid path) by returning the shortened path.
                return newPath;
            }
        }

        //next i check to see if the tile is adjacent
        Terrain endTile = currentPath[currentPath.Length-1];
        int[] validNums = { endTile.x - 1, endTile.x + 1, endTile.y - 1, endTile.y + 1, endTile.x, endTile.y };
        float cost = 0;
        for (int i = 1; i < currentPath.Length; i++)
        { cost += currentPath[i].F; }
        cost += newTile.F;

        //if the next tile is a validNum and there is enough move to pay for it, then just add it to the current path
        if ((((newTile.x == validNums[0] || newTile.x == validNums[1]) && newTile.y == validNums[5]) ^ ((newTile.y == validNums[2] || newTile.y == validNums[3]) && newTile.x == validNums[4])) && cost <= maxMove)
        {
            newPath = new Terrain[currentPath.Length + 1];

            for (int i = 0; i < currentPath.Length; i++)
            {
                newPath[i] = currentPath[i];
            }
            newPath[newPath.Length - 1] = newTile;

            return newPath;
        }
        else    //if it makes here, then the desired tile is on the open list but there is currently no path to it. So now, find the shortest path to the new tile
        {
            List<Terrain> p = new List<Terrain> { newTile };
            do { p.Add(p.Last().parent); } while (p.Last() != currentPath[0]);
            p.Reverse();

            print("path adjusted");
            return p.ToArray();
        }
    }


    /*
    //i'll get back to this. not exactly sure what i'm trying to accomplish with this
    void moveUnit(GameObject[,,] map, int[,] moveCodes, int layer)
    {
        layer = Mathf.Abs(layer);       //layer is for sure a positive integer or zero
        int start = 0;                  //the first code, represents the starting position
        int end = moveCodes.GetLength(0);     //the last code, represents the ending position. Subtracted by 1 later on, use this value to reduce calculations.

        //moveCodes[start, (int)axis.x] = 0;

        /*
        int[] xcode = new int[end];
        int[] ycode = new int[end];

        //break up the codes into numbers that mean something
        for (int i = 0; i < end; i++)
        {
            xcode[i] = moveCodes[i] / 1000;     //the first 3 digits make up the x coordinate
            ycode[i] = moveCodes[i] % 1000;     //the last 3 digits make up the y coordinate
        }
        end--;  //adjust the end value so that it can be used to reference codes
        



    }

    */

	void Start ()
    {
        mapRef = GameObject.Find("Director").GetComponent<GridMap>().MAP;
	}
}
