using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator2D : MonoBehaviour
{
    public Shader PalleteSwap;

    //Holds all of the sprites/shaders that a Character uses during gameplay. To animate I'll create a script that assigns sprites to the sprite renderer.
    //[System.Serializable]
    public struct SpriteAnimation
    {
        
        public Sprite[] SpriteSheet;
        public float FrameSpeed;
    }
    public SpriteAnimation[] Animations;

    [System.Serializable]
    public struct CharacterArt
    {
        public Sprite Portrait;
        public Sprite[] Idle;
        public Sprite[] Selected;
        public Sprite[] WalkSide;
        public Sprite[] WalkFront;
        public Sprite[] WalkBack;
        public Sprite[] Attack;
        public Sprite[] CriticalHit;
        public Sprite[] Dodge;
    }
    public CharacterArt SpriteSheets;

    SpriteRenderer Renderer;
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        Animation(SpriteSheets.Idle, 1f);
    }


    //Grab the Renderer attatched to this object
    private void OnValidate()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
    }
}
