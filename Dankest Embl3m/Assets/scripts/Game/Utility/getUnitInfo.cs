using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getUnitInfo : MonoBehaviour
{

    Text txt;
    //Terrain[,] mapRef;
    RPGClass unitRef;
    Selector cursor;
    //int x;
    //int y;

	// Use this for initialization
	void Start ()
    {
        //mapRef = FindObjectOfType<Map>().LevelMap;
        cursor = FindObjectOfType<Selector>();
        txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //x = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.x);
        //y = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.y);

        unitRef = cursor.GetUnitAtCursorPosition();
        if (unitRef != null)
        {
            //RPGClass unit = mapRef[x, y, ].GetComponent<RPGClass>();
            //string name = mapRef[x, y, 1].GetComponent<Character>().CharacterName;
            string name = unitRef.CharacterName; //print(name);
            txt.text = name + "\nHP: " + unitRef.Stats[0].dynamicValue + "/" + unitRef.Stats[0].staticValue + "\nSP: " + unitRef.Stats[1].dynamicValue + "/" + unitRef.Stats[1].staticValue;
        }
    }
}