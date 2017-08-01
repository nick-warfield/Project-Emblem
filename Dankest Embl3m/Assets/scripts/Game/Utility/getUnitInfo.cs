using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getUnitInfo : MonoBehaviour
{

    public Text[] txt;
    public Image[] bar;
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
	}
	
	// Update is called once per frame
	void Update ()
    {
        //x = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.x);
        //y = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.y);

        unitRef = cursor.GetUnitAtCursorPosition();
        if (unitRef != null)
        {

            txt[0].text = unitRef.CharacterName;
            txt[1].text = unitRef.Stats[0].dynamicValue.ToString();
            txt[2].text = unitRef.Stats[0].staticValue.ToString();
            txt[3].text = unitRef.Stats[1].dynamicValue.ToString();
            txt[4].text = unitRef.Stats[1].staticValue.ToString();

            float percent = (float)unitRef.Stats[0].dynamicValue / unitRef.Stats[0].staticValue;
            bar[0].rectTransform.localScale = new Vector3(percent, 1, 1);

            percent = (float)unitRef.Stats[1].dynamicValue / unitRef.Stats[1].staticValue;
            if (percent <= 1)
            {
                bar[1].rectTransform.localScale = new Vector3(percent, 1, 1);
                bar[2].rectTransform.localScale = new Vector3(0, 1, 1);
            }
            else
            {
                bar[1].rectTransform.localScale = new Vector3(1, 1, 1);
                bar[2].rectTransform.localScale = new Vector3(percent - 1, 1, 1);
            }

            //RPGClass unit = mapRef[x, y, ].GetComponent<RPGClass>();
            //string name = mapRef[x, y, 1].GetComponent<Character>().CharacterName;
            //string name = unitRef.CharacterName; //print(name);
            //txt.text = name + "\nHP: " + unitRef.Stats[0].dynamicValue + "/" + unitRef.Stats[0].staticValue + "\nSP: " + unitRef.Stats[1].dynamicValue + "/" + unitRef.Stats[1].staticValue;
        }
    }
}