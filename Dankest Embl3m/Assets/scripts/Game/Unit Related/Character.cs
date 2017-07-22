using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use requirecomponent to auto assign all the components needed to build a character
//[RequireComponent(typeof(RPGClass))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]

public class Character : MonoBehaviour
{
    //Enums for managing the State and Status of the Unit
    public enum _State { Idle, Selected, Walking, Waiting, InCombat, SelectingAction, Rescued };
    public _State CurrentState = _State.Idle;
    public enum StatusEffects { None, Tramatized, Downed, Dead };
    public StatusEffects Condition;

    //The Character's X and Y coordinates, hidden in inspector. Plus a little function to update them using position
    [HideInInspector] public int x, y;
    public void UpdateCoordinatesWithTransformPosition ()
    {
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
    }

    //The Character's Name and Description, defaults are provided.
    public string CharacterName = "Soldier", CharacterDescription = "A grunt of the Empire's Military.";
    public bool FodderCharacter = true;

    public enum Horoscopes { Aries, Taurus, Gemini, Cancer, Leo, Virgo, Libra, Scorpio, Sagittarius, Capricorn, Aquarius, Pisces, Random };
    public Horoscopes Horoscope = Horoscopes.Random;

    //Holds all of the sprites/shaders that a Character uses during gameplay. To animate I'll create a script that assigns sprites to the sprite renderer.
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

        public Shader PalleteSwap;
        public SpriteRenderer Renderer;
    }
    public CharacterArt SpriteSheets;

    //This is where I will add on support functions and stats and stuff. Just need a place holder for now since those are some pretty advanced features.
    public string support = "Support Struct Goes Here";


    //keeps track of how many times a unit has been traumatized. The more they have been traumatized, the worse the effects and the more easily it happens.
    //units get tramatized from hitting 0HP, maxing stress, and accumliating injuries.
    [HideInInspector] public int TraumaCounter = 0;

    //bad things happen to units at max stress
    protected virtual void BreakingPoint(int Will, int Luck)
    {
        float RolledNum = Random.value * 100;
        int trauma = TraumaCounter * 10;

        //overcome mental stress. Either by rolling a '20' or by rolling under a doubled will stat.
        if (RolledNum <= 1 || (RolledNum + trauma) <= (Will * 2))
        {
            //bunch of stat boosts
        }
        //otherwise, the check is failed and the unit suffers
        else
        {
            TraumaCounter += 1;
            //player loses total control over this unit, maybe roll on a table that has a variety of effects.
        }
    }

    //bad things happen to units that hit 0HP
    protected virtual void DeathsDoor(int Constitution, int Luck, int CurrentHP, int MaxHP)
    {
        float RolledNum = Random.value * 100;
        int trauma = TraumaCounter * 10;

        //get up if really lucky
        if (RolledNum <= 1)
        {
            CurrentHP = MaxHP / 2;
        }

        //kill unit if they are real unlucky
        if (RolledNum + trauma > (Constitution * 4) || RolledNum >= 99)
        {
            Destroy(gameObject);
        }

        //a chance to remain unscathed
        if (RolledNum + trauma > (Constitution * 2))
        {
            TraumaCounter += 1;
            GainInjury();
        }
    }

    //checks to see if a unit gains an injury, then assigns it to them if they did
    public void GainInjury()
    {
        //A table to roll on to see what kind of disability this unit will suffer, at high values this unit's death becomes certain. See Star Wars tabletop RPG for inspiration.
        //Could possibly avoid injury at a low value. Could also gain permanent disabilites, such as losing an eye or limb.
        //Trauma increases the value, making higher values more common.
    }


    //used to instantiate default values in editor once a component is added/reset to a gameobject
    //set up sprites next
    void Reset ()
    {
        tag = "Red Team";
        gameObject.layer = 13;
        transform.position = new Vector3(0, 0, -1f);

        Rigidbody charRB = gameObject.GetComponent<Rigidbody>();
        BoxCollider charBOX = gameObject.GetComponent<BoxCollider>();

        charRB.useGravity = false;
        charRB.isKinematic = true;
        charRB.constraints = RigidbodyConstraints.FreezePositionZ;
        charRB.freezeRotation = true;

        charBOX.center = new Vector3(0.5f, -0.5f, 0);
        charBOX.size = new Vector3(0.5f, 0.5f, 2);
    }

    private void Awake()
    {
        //Make sure Coordinates are initialized early so that they can be referenced by managers in their start functions
        UpdateCoordinatesWithTransformPosition();
    }

    private void Start()
    {
        //Assign a Horoscope Randomly
        if (Horoscope == Horoscopes.Random)
        { Horoscope = (Horoscopes)Random.Range(0, 11); }

        //Snap character into allignment
        transform.position = new Vector3(x, y, -1);
    }

    private void Update()
    { transform.position = new Vector3(x, y, transform.position.z); }
}