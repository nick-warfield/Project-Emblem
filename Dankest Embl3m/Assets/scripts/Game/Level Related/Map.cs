﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MAPS MUST BE RECTANGLES WITH NO HOLES
//Creates and holds the map data so that it can be accessed and altered as needed.
public class Map : MonoBehaviour
{
    public Terrain[,] TerrainMap;   //Holds the terrain data. Formatted so that indexes correspond to transforms.

    //collides a sphere against the node of a tile, then stores the gameobject collided with.
    void StoreTilesIntoArray()
    {
        int x = 0;
        int y = 0;
        bool didYIncrementTwiceInARow = false;      //exit condition for do-while loop
        List<Terrain> ter = new List<Terrain> { };  //used to add tiles as they are read

        do
        {
            //Checks to see if a tile exsists at a specific location
            Collider[] Reader = Physics.OverlapSphere(new Vector3(x+0.5f, y+0.5f, 0), 0.25f);

            if (Reader.Length == 1)     //if only 1 tile has been detected, add it to the list. Then increment x.
            {
                ter.Add(Reader[0].GetComponent<Terrain>());
                x++;
            }
            else if (Reader.Length == 0 && x != 0)  //if no tile was detected and x has not been reset, then increment y and reset x.
            {
                x = 0;
                y++;
            }
            else if (Reader.Length > 1)     //if more than one tile was detected, end search and throw an error message. This code has no way to handle this.
            {
                print("Two Tiles Occupying (" + x + ", " + y + ") Detected.");
                didYIncrementTwiceInARow = true;
            }
            else    //if all the other checks failed, then we have incremented y twice and there are no more tiles to be loaded in
            {
                y--;    //decrement y so we have the correct number of rows
                x = ter.Count / y;  //get x by dividing the total number of tiles found by the number of rows. This works if the map is a perfect rectangle with no holes
                TerrainMap = new Terrain[x, y];     //allocate my array

                int k = 0;  //k is used to cycle through the 1 dimensonal list
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        TerrainMap[i, j] = ter[k];  //load list data into array in a correctly formated manner
                        k++;
                    }
                }

                didYIncrementTwiceInARow = true;        //turn on my exit condition
                print("tiles loaded into array");       //message to confirm tiles were loaded
            }
        }
        while (!didYIncrementTwiceInARow);
    }

    private void Awake()
    {StoreTilesIntoArray(); }
}