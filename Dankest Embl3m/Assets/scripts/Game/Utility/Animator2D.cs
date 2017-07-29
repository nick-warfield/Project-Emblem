using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator2D : MonoBehaviour
{
    [System.Serializable]
    public struct SpriteSheet
    {
        [HideInInspector] public string name;        //the name of the sprite sheet
        public Sprite[] frame;              //the individual frames of the sprite sheet
        public float speed;                 //how fast the sprite sheet is animated

        //Constructor
        public SpriteSheet(Sprite FirstFrame, float FrameSpeed)
        {
            name = FirstFrame.name;

            //FirstFrame.
            frame = new Sprite[1] { FirstFrame };
            speed = FrameSpeed;
        }
        public SpriteSheet(Sprite[] Frames, float FrameSpeed)
        {
            name = Frames[0].name;

            frame = new Sprite[Frames.Length];
            for (int i = 0; i < Frames.Length; i++)
            { frame[i] = Frames[i]; }

            speed = FrameSpeed;
        }
        /*
        public SpriteSheet(string SpriteName, float FrameSpeed)
        {
            name = SpriteName;

            List<Sprite> spr = new List<Sprite> { };
            spr.Add(DataB)

            speed = FrameSpeed;
        }
        */
    }
    //public SpriteSheet Animation = new SpriteSheet()

    //public Shader PalleteSwap;

    //Holds all of the sprites/shaders that a Character uses during gameplay. To animate I'll create a script that assigns sprites to the sprite renderer.
    //[System.Serializable]
    /*public struct SpriteAnimation
    {
        
        public Sprite[] SpriteSheet;
        public float FrameSpeed;
    }
    public SpriteAnimation[] Animations;
    */
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
    public void Animate(Sprite[] Animation, float FrameSpeed)
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
        Animate(SpriteSheets.Idle, 1f);
    }


    //Grab the Renderer attatched to this object
    private void Start()
    {
        Renderer = gameObject.GetComponent<SpriteRenderer>();
    }
}
