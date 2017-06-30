using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Uncomment this to generate a new map in editor.
//[ExecuteInEditMode]

public class LevelInfo : MonoBehaviour
{
    public int width;   //how many tiles wide is the map
    public int height;  //how many tiles tall is the map

    public GameObject terrainFill;  //the tiles to fill out the terrain layer with by default

    //the array that holds all of the gameobjects in use for the level
    public GameObject[,,] MAP;

    
    [System.Serializable]
    public struct TerrainInfo
    {
        public int w;
        //public Terrain[] Rows;
    };

    public TerrainInfo[] Terrain = new TerrainInfo[5];


    //fill out the MAP array with in game data
    void InitializeMAP()
    {
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);     //set camera

        //set the length, width, and number of layers in the map
        MAP = new GameObject[width, height, 3];

        //Creates Missing Layers
        //turn on Execute in Edit Mode to have the created layers made permanent
        CreateUnitLayer();
        CreateTerrainLayer(terrainFill);

        //Reads every position in the world, and loads that object into the corresponding array value
        ReadMAP_Data();
    }

    //if the terrain layer has not been made, make one and then fill out with default tiles. This will also place them in the game world
    void CreateTerrainLayer(GameObject tile)
    {
        GameObject terrainLayer = GameObject.Find("Terrain");

        //make terrain if hasn't been made yet
        if (terrainLayer == null)
        {
            terrainLayer = new GameObject();
            terrainLayer.name = "Terrain";

            //loop through each 'width' value
            for (int i = 0; i < MAP.GetLength(0); i++)
            {
                //loop through each 'height' value
                for (int j = 0; j < MAP.GetLength(1); j++)
                {
                    tile.transform.position = new Vector3(i, j, 0);
                    tile.name = ("Tile (X:" + i + " Y:" + j + ") ");
                    Instantiate(tile, terrainLayer.transform);
                }
            }
        }


        //for (int i = 0; i < MAP.GetLength(0); i++)
        //{
        //    for (int j = 0; j < MAP.GetLength(1); j++)
        //    {
        //        Collider[] thingTouching = Physics.OverlapSphere(new Vector3(i + .5f, j + .5f, 0), .25f);
        //        MAP[i, j, 0] = thingTouching[0].gameObject;
        //        //print(MAP[i, j, 0].ToString());
        //    }
        //}

    }

    //create the layer that holds units' information, if it does not exsist
    void CreateUnitLayer()
    {
        GameObject unitLayer = GameObject.Find("Units");

        if (unitLayer == null)
        {
            unitLayer = new GameObject("Units");
            unitLayer.transform.position = new Vector3(0, 0, -1);

            //figure out how to easily load a bunch of units into the map on start up

            //for (int i = 0; i < UnitsOnMap.GetLength(0); i++)
            //{
            //    //MAP[i, 0, 1] = UnitsOnMap[i];
            //    //UnitsOnMap[i].transform.position = new Vector3(i, 0, 0);
            //    //Instantiate(UnitsOnMap[i], unitLayer.transform);
            //}
        }

        //for (int i = 0; i < MAP.GetLength(0); i++)
        //{
        //    for (int j = 0; j < MAP.GetLength(1); j++)
        //    {
        //        Collider[] thingTouching = Physics.OverlapSphere(new Vector3(i + .5f, j + .5f, -1), .25f);
        //        if (thingTouching.GetLength(0) == 1)
        //        {
        //            MAP[i, j, 1] = thingTouching[0].gameObject;
        //            print(MAP[i, j, 1].ToString());
        //        }
        //        else
        //        { MAP[i, j, 1] = null; }
        //    }
        //}
    }

    //collides a sphere against the node of every point in the array, then stores the gameobject collided with (if any)
    void ReadMAP_Data()
    {
        for (int k = 0; k < MAP.GetLength(2); k++)
        {
            for (int i = 0; i < MAP.GetLength(0); i++)
            {
                for (int j = 0; j < MAP.GetLength(1); j++)
                {
                    //figure out how to use 2D Physics for some better performance

                    //use a sphere to check each possible location a game object could be
                    Collider[] thingTouching = Physics.OverlapSphere(new Vector3(i + 0.5f, j + 0.5f, k * -1), .25f);

                    if (thingTouching.GetLength(0) == 1)    //if only one game object was detected, store that object into the MAP array
                    {
                        MAP[i, j, k] = thingTouching[0].gameObject;
                        //print(MAP[i, j, k].ToString() + ' ' + i + ' ' + j + ' ' + k);
                    }
                    else            //otherwise, 0 object || more than 1 object has been detected. In either case, store a null value in the array.
                    { MAP[i, j, k] = null; }
                }
            }
        }
    }


}