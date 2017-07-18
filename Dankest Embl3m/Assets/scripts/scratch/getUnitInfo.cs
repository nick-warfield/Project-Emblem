using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getUnitInfo : MonoBehaviour {

    Text txt;
    GameObject[,,] mapRef;
    int x;
    int y;

	// Use this for initialization
	void Start ()
    {
        mapRef = GameObject.Find("Director").GetComponent<GridMap>().MAP;
        txt = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        x = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.x);
        y = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.y);

        if (mapRef[x, y, 1] != null)
        {
            RPGClass unit = mapRef[x, y, 1].GetComponent<RPGClass>();
            string name = mapRef[x, y, 1].GetComponent<Character>().CharacterName;
            txt.text = name + "\nHP: " + unit.Stats[0].dynamicValue + "/" + unit.Stats[0].staticValue + "\nSP: " + unit.Stats[1].dynamicValue + "/" + unit.Stats[1].staticValue;
        }
    }
}