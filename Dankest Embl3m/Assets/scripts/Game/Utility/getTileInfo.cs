using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getTileInfo : MonoBehaviour
{
    public Text[] txt;
    Terrain[,] mapRef;
    Selector cursor;

	// Use this for initialization
	void Start ()
    {
        //txt = GetComponent<Text>();
        cursor = FindObjectOfType<Selector>();
        mapRef = FindObjectOfType<Map>().LevelMap;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Terrain tile = mapRef[cursor.x, cursor.y];

        if (tile != null)
        {
            txt[0].text = tile.Type.ToString();
            txt[1].text = tile.DodgeBonus.ToString();
            txt[2].text = tile.DefenseBonus.ToString();

            //txt.text = tile.Type.ToString() + "\nDodge: " + tile.DodgeBonus + "\nDef: " + tile.DefenseBonus;
        }
	}
}
