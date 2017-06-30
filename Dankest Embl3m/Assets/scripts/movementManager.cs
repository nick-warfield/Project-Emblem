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


    //find the shortest path to a desired destination
    Terrain[] aStarPathfinding (Terrain Start, Terrain End, int maxDistance)
    {
        //I need to do some pathfinding here, because adjusting the path may make it short enough to work

        List<int> F = new List<int> { 0 };  //G + H, the total cost of using a tile. The lower the better
        List<int> G = new List<int> { 0 };  //the movement cost associatied with a tile eg: Forrest = 2
        List<int> H = new List<int> { 0 };  //the number of tiles (vertical + horizontal) to reach the destination

        int loopCount = 2;      //used for resizing my arrays
        int loopPosition = 0;   //used for accessing the last tile added into an array


        List<Terrain> closedList = new List<Terrain> { Start };     //the tiles that have been selected for the shortest path
        List<Terrain> openList = new List<Terrain>();               //the possible tiles to select from
        Terrain mostRecent;

        do
        {
            openList.Clear();   //reset open list
            mostRecent = closedList.Last<Terrain>();    //update most recent tile added

            //check left tile
            if (mapRef[mostRecent.x - 1, mostRecent.y, 0] != null)
            { openList.Add(mapRef[mostRecent.x - 1, mostRecent.y, 0].GetComponent<Terrain>() ); }
            //check right tile
            if (mapRef[mostRecent.x + 1, mostRecent.y, 0] != null)
            { openList.Add(mapRef[mostRecent.x + 1, mostRecent.y, 0].GetComponent<Terrain>()); }
            //check bottom tile
            if (mapRef[mostRecent.x, mostRecent.y - 1, 0] != null)
            { openList.Add(mapRef[mostRecent.x, mostRecent.y - 1, 0].GetComponent<Terrain>()); }
            //check top tile
            if (mapRef[mostRecent.x, mostRecent.y + 1, 0] != null)
            { openList.Add(mapRef[mostRecent.x, mostRecent.y + 1, 0].GetComponent<Terrain>()); }

            //remove wall tiles from list
            for (int i = 0; i < openList.Count; i++)
            { if (openList[i].Tag == Terrain.tileTag.Wall) { openList.Remove(openList[i]); } }



            loopCount++;        //increment my 'list tracking' variables
            loopPosition++;
        } while (F[0] == 0);   //i'll figure out the exit condition later

        return null;
    }

    //check new tile, return a valid path if possible
    protected Terrain[] gridPathfindingArray (Terrain[] currentPath, Terrain newTile, int maxMove)
    {
        Terrain[] newPath = currentPath;

        //first i check to see if the newTile is a wall, I can end the function right here if so
        if (newTile.Tag == Terrain.tileTag.Wall)
        { print("hit wall"); return newPath; }


        //if the newTile is already on the path, shorten the path to that tile.
        //im doing a linear search starting at the end because the most recent tile is the most likely to be a repeat.
        for (int i = 0; i < currentPath.Length; i++)
        {
            int startFromBack = currentPath.Length - 1 - i;     //my reference for the end of the array

            //if the desired tile is already on the array...
            if (newTile == currentPath[startFromBack])
            {
                print("tile already on path");

                startFromBack++;    //adjust end indicator so that it can set the length of a new array
                newPath = new Terrain[startFromBack];   //create a new array with the shortened length

                //fill out the new array with data from the current path
                for (int j = 0; j < startFromBack; j++)
                { newPath[j] = currentPath[j]; }

                //end the function call (since we have a valid path) by returning the shortened path.
                return newPath;
            }
        }

        //now we know that the newTile is new to the array, so we need to be careful with validation from here on out

        //next i check to see if the tile is adjacent
        Terrain endTile = currentPath[currentPath.Length-1];
        int[] validNums = { endTile.x - 1, endTile.x + 1, endTile.y - 1, endTile.y + 1, endTile.x, endTile.y };

        //if anything except 1 validNum hit is detected, return the current path because the newTile is non-adjacent
        if ( !((newTile.x == validNums[0] || newTile.x == validNums[1]) ^ (newTile.y == validNums[2] || newTile.y == validNums[3])) )
        { print("tile is diagonal"); return newPath; }

        else if ( !(newTile.x == validNums[4] || newTile.y == validNums[5]) )   //if neither of the numbers are the same value as the last tile, it's somewhere far out
        { print("tile is far out"); return newPath; }


        //finally, I check to see if I have the movement to pay for the new tile
        int currentMoveCost = 0;
        //don't include the cost of the starting tile
        for (int i = 1; i < currentPath.Length; i++)
        { currentMoveCost += currentPath[i].movementCost; }

        int newCost = currentMoveCost + newTile.movementCost;

        if (maxMove < newCost)
        {
            //insert shortest path pathfinding here, then check to see if the shortest path can reach the desired tile

            print("not enough movement");
            return newPath;
        }
        else
        {
            newPath = new Terrain[currentPath.Length + 1];

            for (int i = 0; i < currentPath.Length; i++)
            {
                newPath[i] = currentPath[i];
            }
            newPath[newPath.Length - 1] = newTile;
        }

        print("All checks passed");
        return newPath;
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
