using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    GameObject[,,] mapRef;
    combatManager comMan;
    int x, y, z;

    public GameObject selectedUnit = null;

    // Use this for initialization
    void Start()
    {
        mapRef = GameObject.Find("Director").GetComponent<GridMap>().MAP;
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        z = Mathf.RoundToInt(transform.position.z) * -1;

        comMan = GameObject.Find("Director").GetComponent<combatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnit != null)
        {
            selectedUnit.transform.position = new Vector3(x, y, (z-1)*(-1));
        }

        //manages inputs, lot of code so I made it a function to tidy up
        inputManager();
    }


    //detects an input, then does something depending on the input
    void inputManager()
    {
        //move up
        if (Input.GetKeyDown("w") || Input.GetKeyDown("up"))
        {
            mapRef[x, y, z] = null;
            y++;
            mapRef[x, y, z] = gameObject;
            transform.position = new Vector3(x, y, -z);
        }

        //move down
        else if (Input.GetKeyDown("s") || Input.GetKeyDown("down"))
        {
            mapRef[x, y, z] = null;
            y--;
            mapRef[x, y, z] = gameObject;
            transform.position = new Vector3(x, y, -z);
        }

        //move left
        else if (Input.GetKeyDown("a") || Input.GetKeyDown("left"))
        {
            mapRef[x, y, z] = null;
            x--;
            mapRef[x, y, z] = gameObject;
            transform.position = new Vector3(x, y, -z);
        }

        //move right
        else if (Input.GetKeyDown("d") || Input.GetKeyDown("right"))
        {
            mapRef[x, y, z] = null;
            x++;
            mapRef[x, y, z] = gameObject;
            transform.position = new Vector3(x, y, -z);
        }

        //select object, so that only a selected unit may be moved
        else if (Input.GetKeyDown("space"))
        {
            //select or deselect depending on current state
            if (selectedUnit == null)
            {
                selectedUnit = mapRef[x, y, z-1];
                mapRef[x, y, z - 1] = null;
            }
            else
            {
                mapRef[x, y, z - 1] = selectedUnit;
                selectedUnit = null;
            }
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift) && selectedUnit == null)
        {
            if (mapRef[x, y, z - 1] != null)
            {
                if (comMan.attackingUnit == null)
                {
                    comMan.attackingUnit = mapRef[x, y, z - 1];
                    comMan.attackerTerrain = mapRef[x, y, z - 2];
                }
                else
                {
                    comMan.defendingUnit = mapRef[x, y, z - 1];
                    comMan.defenderTerrain = mapRef[x, y, z - 2];
                }
            }
            else if (comMan.attackingUnit != null || comMan.defendingUnit != null)
            {
                comMan.attackingUnit = comMan.attackerTerrain = comMan.defendingUnit = comMan.defenderTerrain = null;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Return) && comMan.attackingUnit != null && comMan.defendingUnit != null)
        {
            comMan.runCombat = true;
        }

    }


}
