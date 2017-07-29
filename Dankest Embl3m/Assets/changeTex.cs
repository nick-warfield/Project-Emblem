using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeTex : MonoBehaviour
{
    public SpriteRenderer rend;
    public Sprite oldSprite, newSprite;

	// Use this for initialization
	void Start ()
    {
        //oldSprite = rend.sprite;
        newSprite = oldSprite;

        Color[] col = new Color[oldSprite.texture.width * oldSprite.texture.height];
        
        for (int i = 0; i < col.Length; i++) { col[i] = new Color(i, i, i); }
        print(col.Length);

        newSprite.texture.SetPixels(col);
        newSprite.texture.Apply();

        rend.sprite = newSprite;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
