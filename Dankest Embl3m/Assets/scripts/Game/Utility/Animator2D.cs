using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator2D : MonoBehaviour
{
    SpriteRenderer Renderer;
    Character.CharacterArt SpriteSheets;
    float TimeStamp = 1;
    int CurrentFrame = 0;

    //So other functions can read the current frame
    public int GetCurrentFrame()
    { return CurrentFrame; }

    //Loops through an array of sprites at a certain speed
    public void Animation(Sprite[] Animation, float FrameSpeed)
    {
        if (Time.time >= TimeStamp)
        {
            TimeStamp = Time.time + FrameSpeed;
            CurrentFrame++;

            if (CurrentFrame >= Animation.Length)
            { CurrentFrame = 0; }

            Renderer.sprite = Animation[CurrentFrame];

            /*
            for (int i = 0; i < Animation.Length; i++)
            {
                if (Animation[i] == Renderer.sprite)
                {
                    int nextFrame = i + 1;
                    if (nextFrame >= Animation.Length) { nextFrame = 0; }

                    //print(nextFrame);
                    Renderer.sprite = Animation[nextFrame];
                    break;
                }
            }*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        Animation(SpriteSheets.Idle, 1f);
    }

    private void Start()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        SpriteSheets = gameObject.GetComponent<RPGClass>().SpriteSheets;
    }

    private void Reset()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
        SpriteSheets = gameObject.GetComponent<RPGClass>().SpriteSheets;
    }
}
