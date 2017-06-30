using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class preventOverlap : MonoBehaviour {

    RectTransform corner;
    Image panel;
    GameObject cursor;

	// Use this for initialization
	void Start ()
    {
        corner = GetComponent<RectTransform>();
        panel = GetComponent<Image>();
        cursor = GameObject.Find("Cursor");
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}
