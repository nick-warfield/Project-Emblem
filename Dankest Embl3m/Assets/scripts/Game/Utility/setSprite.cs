using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

class setSprite : MonoBehaviour
{
    public SpriteAtlas spr;
    public SpriteRenderer ren = null;
    public Animator2D.SpriteSheet[] sheets;

    Sprite[] sprits;
    float timeStamp;
    int current = 0;

	// Use this for initialization
	void Start ()
    {
        sprits = new Sprite[spr.spriteCount];
        spr.GetSprites(sprits);
        ren.sprite = sprits[current];
        timeStamp = Time.time + 1f;

        sheets = new Animator2D.SpriteSheet[1]
        {
            new Animator2D.SpriteSheet(CreateSpriteSheet("Blue", sprits), 1f)
        };
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


    //creates a sprite sheet in proper order from a larger list
    Sprite[] CreateSpriteSheet(string SearchTerm, Sprite[] Array)
    {
        List<Sprite> sheet = new List<Sprite> { };

        for (int i = 0; i < Array.Length; i++)
        {
            if (Array[i].name.Contains(SearchTerm))
            { sheet.Add(Array[i]); }
        }

        /*
        int[] sortingIndex = new int[sheet.Count];
        for (int i = 0; i < sortingIndex.Length; i++)
        {
            sortingIndex[i] = int.Parse(sheet[i].name);
        }
        

        for (int i = 0; i < sortingIndex.Length - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (sortingIndex[j - 1] < sortingIndex[j])
                {
                    int temp = sortingIndex[j - 1];
                    sortingIndex[j - 1] = sortingIndex[j];
                    sortingIndex[j] = temp;
                }
            }
        }
        */

        return sheet.ToArray();
    }
}
