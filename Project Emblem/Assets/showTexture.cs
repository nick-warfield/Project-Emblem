using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showTexture : MonoBehaviour {
    public Texture2D tex;
	// Use this for initialization
	void Start ()
    {
        //Texture tex = FindObjectOfType<PaletteSwapLookup>().LookupTexture;
        Sprite spr = Sprite.Create(tex, new Rect(transform.position.x, transform.position.y, 9, 2), new Vector2(0, 0), 5);
        GetComponent<SpriteRenderer>().sprite = spr;
	}
}
