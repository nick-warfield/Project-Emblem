using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Uncomment this to generate a new map in editor.
//[ExecuteInEditMode]

public class GridMap : MonoBehaviour
{
    public int width;   //how many tiles wide is the map
    public int height;  //how many tiles tall is the map
    public int layer;   //how many layers are there to map (tile layer, unit layer, etc)

    public LayerMask hi;

    //for adjusting lerp values
    float increment = 0;
    public float lerpSpeed = 7;

    GameObject backgroundLayer;     //the terrain layer of the map
    public GameObject terrainFill;  //the tiles to fill out the terrain layer with by default
    //public GameObject[] UnitsOnMap;

    //the array that holds all of the gameobjects in use for the level
    public GameObject[,,] MAP;


    //vvv functions below vvv\\


    //fill out the MAP array with in game data
    void InitializeMAP ()
    {
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);     //set camera

        //set the length, width, and number of layers in the map
        MAP = new GameObject[width, height, layer];

        //Creates Missing Layers
        //turn on Execute in Edit Mode to have the created layers made permanent
        CreateUnitLayer();
        CreateTerrainLayer(terrainFill);

        //Reads every position in the world, and loads that object into the corresponding array value
        ReadMAP_Data();
    }

    //if the terrain layer has not been made, make one and then fill out with default tiles. This will also place them in the game world
    void CreateTerrainLayer (GameObject tile)
    {
        GameObject terrainLayer = GameObject.Find("Terrain");
        
        //make terrain if hasn't been made yet
        if (terrainLayer == null)
        {
            backgroundLayer = new GameObject();
            backgroundLayer.name = "Terrain";

            //loop through each 'width' value
            for (int i = 0; i < MAP.GetLength(0); i++)
            {
                //loop through each 'height' value
                for (int j = 0; j < MAP.GetLength(1); j++)
                {
                    tile.transform.position = new Vector3(i, j, 0);
                    tile.name = ("Tile (X:" + i + " Y:" + j + ") ");
                    Instantiate(tile, backgroundLayer.transform);
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
    void CreateUnitLayer ()
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


    //WIP. Supposed to streamline my movement. So that other scripts only need to call one function and provide only the start and end place.
    void ChangeMapPosition (GameObject StartArrayPosition, GameObject EndArrayPosition)
    {
        bool updateLocation = true;

        //this is where the magic happens, validation needs to be done before this point
        //updateLocation = MoveLocation();

        //shut off debugging inputs, reset update location so that it can accept new changes
        if (updateLocation == false)
        {
            //for (int i = 0; i < yep.GetLength(0); i++)
            //{ yep[i] = false; }

            updateLocation = true;
        }
    }

    //returnining false means the function should not be run anymore
    bool MoveLocation (int StartX, int StartY, int EndX, int EndY, int Layer)
    {
        //gaurantee that the layer value is 0 or positive
        Layer = Mathf.Abs(Layer);

        //Initialize 
        GameObject StartingObject = MAP[StartX, StartY, Layer];
        GameObject Destination = MAP[EndX, EndY, Layer];
        Vector3 StartPlace = new Vector3(StartX, StartY, -Layer);
        Vector3 EndPlace = new Vector3(EndX, EndY, -Layer);
        float theDistance = Vector3.Distance(StartPlace, EndPlace);
        print(theDistance);

        //Check path, end function if destination is occupied or if starting object is null
        if (Destination != null) { return false; }
        if (StartingObject == null) { return false; }

        //Different move if complex movement detected
        if (StartX != EndX && StartY != EndY)
        {
            //Maybe doing this from within the function is not the most elegant solution
            //I think that creating a larger helper function to hold this one is the way to go
            //it'll take two array locations as input, convert them, then move things in gird
            //it could also handle shutting this function off on it's own

            //MoveLocation(StartX, StartY, EndX, StartY, layer);

            //EndPlace = new Vector3(StartX, EndY, -layer);
            //theDistance = Vector3.Distance(StartPlace, EndPlace);
            //print(theDistance);

            return false;
        }


        //Move if approved
        if (increment <= 1)
        {
            increment += ((lerpSpeed / theDistance) / 100f);
            StartingObject.transform.position = Vector3.Lerp(StartPlace, EndPlace, increment);

            return true;
        }

        //Update array only once when done
        if (increment > 1)
        {
            increment = 0;

            MAP[EndX, EndY, Layer] = StartingObject;
            MAP[StartX, StartY, Layer] = null;

            StartingObject.transform.position = new Vector3(EndX, EndY, -Layer);

            return false;
        }

        return false;
    }

    //make the game turn based. This will handle switching between turns
    string TurnTracker (string currentPhase, bool turnEnd)
    {
        GameObject[] WorkingUnits = GameObject.FindGameObjectsWithTag(currentPhase);

        //if the user has not asked for their turn to end, check to see if they can still do anything
        if (!turnEnd)
        {
            turnEnd = true;
            for (int i = 0; i < WorkingUnits.Length; i++)
            {
                if (!WorkingUnits[i].GetComponent<RPGClass>().hasMoved)
                { turnEnd = false; break; }
            }
        }

        //if the user want their turn to end
        if (turnEnd)
        {
            do
            {
                turnEnd = false;

                if (currentPhase == "Blue Team")
                { currentPhase = "Green Team"; }

                else if (currentPhase == "Green Team")
                { currentPhase = "Red Team"; }

                else if (currentPhase == "Red Team")
                { currentPhase = "Blue Team"; }


                WorkingUnits = GameObject.FindGameObjectsWithTag(currentPhase);
                if (WorkingUnits.Length > 0)
                {
                    turnEnd = true;

                    for (int i = 0; i < WorkingUnits.Length; i++)
                    { WorkingUnits[i].GetComponent<RPGClass>().hasMoved = false; }
                }


            } while (!turnEnd);

            print(currentPhase);
        }


        return currentPhase;
    }
    public string currentTurn = "Blue Team";


    //initialize MAP array here, i use Awake to make sure I have the array ready if another thing needs to reference it in thier Start loop.
    void Awake ()
    {
        InitializeMAP();
    }


    private void Update()
    {
        currentTurn = TurnTracker(currentTurn, false);

        //MovementScratch();
    }





    /*
    //using for debugging
    bool hola = true;
    bool[] yep = new bool[10];
    void MovementScratch()
    {
        if (hola == false)
        {
            for (int i = 0; i < yep.GetLength(0); i++)
            { yep[i] = false; }

            hola = true;
        }

        //reset indiana bones
        if (Input.GetKey("r"))
        {
            GameObject thing = GameObject.Find("Indiana Bones");
            thing.transform.position = new Vector3(11, 3, -1);

            ReadMAP_Data();
            print(MAP[11, 3, 1]);

            for (int i = 0; i < yep.GetLength(0); i++)
            { yep[i] = false; }

            hola = true;
        }

        if (Input.GetKey("b")) { yep[0] = true; }
        if (Input.GetKey("v")) { yep[2] = true; }
        if (Input.GetKey("z")) { yep[6] = true; }
        if (Input.GetKey("m")) { yep[7] = true; }
        if (Input.GetKey("c")) { yep[8] = true; }

        //this set up stops the location from being called once it is done moving something, should probably switch to a bool to simplify
        //will have to reset hola before being able to move something again, nevermind fixed with if statement at the start of update
        if (yep[6])
        {
            if (hola == true)
            {
                hola = MoveLocation(11, 3, 9, 3, 1);
                print(hola);
            }
        }

        //this set up allows for units to travel through one another, but is not grid locked
        if (yep[7])
        {
            if (hola == true) { hola = MoveLocation(11, 3, 3, 3, 1); print(hola); }
        }

        if (yep[8])
        {
            if (hola == true) { hola = MoveLocation(9, 3, 9, 5, 1); print(hola); }
        }

        if (yep[2])
        {
            if (hola == true) { hola = MoveLocation(11, 3, 10, 4, 1); print(hola); }
        }

        //this kind of set up won't allow for travelling through allied units
        if (yep[0])
        {
            MoveLocation(11, 3, 11, 5, 1);
            yep[1] = true;

            if (yep[1])
            {
                MoveLocation(11, 5, 7, 5, 1);
            }
        }
    }
    */
}
