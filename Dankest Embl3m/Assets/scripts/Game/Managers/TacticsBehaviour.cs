using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I plan to use this script to derive all of my key functions from. Then I'll have other scripts inherit from here to access them.
public class TacticsBehaviour : MonoBehaviour
{
    

    /*
    //find the shortest path to every tile in range
    protected Terrain[] DijkstraPath(Vector2 Start, RPGClass Unit, Terrain[,] Map)
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
            for (int i = 0; i < 4; i++)
            {
                temp = null;
                switch (i)
                {
                    case 0: if (mostRecent.x + 1 < map.GetLength(0)) { temp = map[mostRecent.x + 1, mostRecent.y, 0].GetComponent<Terrain>(); } break;  //right tile
                    case 1: if (mostRecent.x - 1 >= 0) { temp = map[mostRecent.x - 1, mostRecent.y, 0].GetComponent<Terrain>(); } break;  //left tile
                    case 2: if (mostRecent.y + 1 < map.GetLength(1)) { temp = map[mostRecent.x, mostRecent.y + 1, 0].GetComponent<Terrain>(); } break;  //above tile
                    case 3: if (mostRecent.y - 1 >= 0) { temp = map[mostRecent.x, mostRecent.y - 1, 0].GetComponent<Terrain>(); } break;  //below tile
                }
                //if an existing tile was found
                if (temp != null)
                {
                    temp.F = Unit.GetAdjustedF(temp);

                    //ignore the tile if i have already visited it, or it is a wall, or there is not enough movement to pay for it
                    if (!visited.Contains(temp) && temp.Type != Terrain.Tile.Wall && mostRecent.H + temp.F <= move)
                    {
                        //if the checked tile is already on the unvisited list, check to see if this new path is shorter
                        if (unvisited.Contains(temp))
                        {
                            if (mostRecent.H + temp.F < temp.H)
                            { temp.H = mostRecent.H + temp.F; temp.parent = mostRecent; }
                        }
                        else    //otherwise, add it to the unvisited list
                        {
                            unvisited.Add(temp);                //add to unvisited
                            temp.H = mostRecent.H + temp.F;     //update H for future comparison
                            temp.parent = mostRecent;           //update parent so a path can be formed as needed
                        }
                    }
                }   //temp check
            }       //for loop


            //sort list (lazily) so that the first result has the smallest H value
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

        //reset the h values for future pathfindings... parent should not be reset so that it can be used in future.
        for (int i = 0; i < visited.Count; i++) { visited[i].H = Mathf.Infinity; }

        return visited.ToArray();
    }

    */
}
