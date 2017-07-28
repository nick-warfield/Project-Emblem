using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

class setSprite : MonoBehaviour
{
    public SpriteAtlas spr;
    public SpriteRenderer ren = null;
    Sprite[] sprits;
    float timeStamp;
    int current = 0;

	// Use this for initialization
	void Start ()
    {
        sprits = new Sprite[spr.spriteCount];
        spr.GetSprites(sprits, "Phalanx-02-Blue_1");
        ren.sprite = sprits[current];
        timeStamp = Time.time + 1f;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Time.time >= timeStamp)
        {
            current++;
            if (current >= sprits.Length) { current = 0; }

            ren.sprite = sprits[current];

            timeStamp = Time.time + 1f;
        }
	}
}
