using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use requirecomponent to auto assign all the components needed to build a character
//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator2D))]
[RequireComponent(typeof(PixelSnap))]

public class Character : MonoBehaviour
{
    //Enums for managing the State and Status of the Unit.                          Rescued should be a component that sets state to waiting
    public enum _State { Idle, Selected, Walking, Waiting, InCombat, SelectingAction, Rescued };
    public _State CurrentState = _State.Idle;

    //Enum for checking which team a unit is on
    [HideInInspector] public TurnManager.TeamColor Team = TurnManager.TeamColor.Red;

    //I should probably use components for this stuff.
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


    //This is where I will add on support functions and stats and stuff. Just need a place holder for now since those are some pretty advanced features.
    public string support = "Support Struct Goes Here";


    //keeps track of how many times a unit has been traumatized. The more they have been traumatized, the worse the effects and the more easily it happens.
    //units get tramatized from hitting 0HP, maxing stress, and accumliating injuries.
    public int TraumaCounter = 0;

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
    public virtual void DeathsDoor(int Constitution, int Luck, int CurrentHP, int MaxHP)
    {
        int RolledNum = Random.Range(0, 100);
        //int trauma = TraumaCounter * 10;

        //get up if really lucky
        if (RolledNum < 1)
        { CurrentHP = MaxHP / 4; }

        //Die if really unlucky
        else if (RolledNum > 99)
        { Die(); }

        //Now start checking for different types of injury
        RolledNum = RolledNum + (TraumaCounter * 10) - (Constitution) - (Luck / 4);

        if (RolledNum < 0)
        { return; }
        else if (RolledNum < 40)
        { MinorInjury(RolledNum); }
        else if (RolledNum < 70)
        { SeriousInjury(RolledNum); }
        else if (RolledNum < 90)
        { PermanentInjury(RolledNum); }
        else if (RolledNum < 100)
        { FatalInjury(RolledNum); }
        else
        { Die(); }

        TraumaCounter += 1;
    }

    //Character Dies Permanently
    public void Die()
    { Destroy(gameObject); }

    //Character is fatally injured and will die
    void FatalInjury (float roll)
    {
        Injury newInjury = gameObject.AddComponent<Injury>();
        newInjury.Victim = GetComponent<RPGClass>();
        newInjury.Severity = Injury.InjuryLevel.Fatal;
        newInjury.Name = newInjury.Severity.ToString() + ": " + roll;

        if (newInjury.Victim != null)
        {
            newInjury.AffectedStats = new Injury.StatPenalty[5]
            {
                new Injury.StatPenalty(RPGClass.Stat.Strength, -2),
                new Injury.StatPenalty(RPGClass.Stat.Magic, -2),
                new Injury.StatPenalty(RPGClass.Stat.Speed, -2),
                new Injury.StatPenalty(RPGClass.Stat.Skill, -2),
                new Injury.StatPenalty(RPGClass.Stat.Move, -1)
            };
            newInjury.ApplyDebuff();
        }
    }

    //Character gains a permanent injury
    void PermanentInjury (float roll)
    {
        Injury newInjury = gameObject.AddComponent<Injury>();
        newInjury.Victim = GetComponent<RPGClass>();
        newInjury.Severity = Injury.InjuryLevel.Permanent;
        newInjury.Name = newInjury.Severity.ToString() + ": " + roll;

        if (newInjury.Victim != null)
        {
            newInjury.AffectedStats = new Injury.StatPenalty[3]
            {
                new Injury.StatPenalty(RPGClass.Stat.Speed, -2),
                new Injury.StatPenalty(RPGClass.Stat.Skill, -2),
                new Injury.StatPenalty(RPGClass.Stat.Move, -1)
            };
            newInjury.ApplyDebuff();
        }
    }

    //Character gains an injury that will affect them for the rest of the encounter
    void SeriousInjury (float roll)
    {
        Injury newInjury = gameObject.AddComponent<Injury>();
        newInjury.Victim = GetComponent<RPGClass>();
        newInjury.Severity = Injury.InjuryLevel.Serious;
        newInjury.Name = newInjury.Severity.ToString() + ": " + roll;

        if (newInjury.Victim != null)
        {
            newInjury.AffectedStats = new Injury.StatPenalty[3]
            {
                new Injury.StatPenalty(RPGClass.Stat.Strength, -1),
                new Injury.StatPenalty(RPGClass.Stat.Magic, -1),
                new Injury.StatPenalty(RPGClass.Stat.Speed, -1)
            };
            newInjury.ApplyDebuff();
        }
    }

    //Character gains a light injury that will go away after some time
    void MinorInjury (float roll)
    {
        Injury newInjury = gameObject.AddComponent<Injury>();
        newInjury.Victim = GetComponent<RPGClass>();
        newInjury.Severity = Injury.InjuryLevel.Minor;
        newInjury.Name = newInjury.Severity.ToString() + ": " + roll;

        if (newInjury.Victim != null)
        {
            newInjury.AffectedStats = new Injury.StatPenalty[2]
            {
                new Injury.StatPenalty(RPGClass.Stat.Strength, -1),
                new Injury.StatPenalty(RPGClass.Stat.Magic, -1)
            };
            newInjury.ApplyDebuff();
        }
    }



    //used to instantiate default values in editor once a component is added/reset to a gameobject
    //set up sprites next
    protected virtual void Reset ()
    {
        tag = "Red Team";
        gameObject.layer = 13;
        //transform.position = new Vector3(0, 0, -1f);

        /*
        Rigidbody charRB = gameObject.GetComponent<Rigidbody>();
        BoxCollider charBOX = gameObject.GetComponent<BoxCollider>();

        charRB.useGravity = false;
        charRB.isKinematic = true;
        charRB.constraints = RigidbodyConstraints.FreezePositionZ;
        charRB.freezeRotation = true;

        charBOX.size = new Vector3(0.5f, 0.5f, 0.5f);
        */
    }
    private void Awake()
    {
        //Make sure Coordinates are initialized early so that they can be referenced by managers in their start functions
        UpdateCoordinatesWithTransformPosition();

        //Assign team based on gameobject tag
        switch (tag)
        {
            case    "Red Team": Team = TurnManager.TeamColor.Red;    break;
            case   "Blue Team": Team = TurnManager.TeamColor.Blue;   break;
            case  "Green Team": Team = TurnManager.TeamColor.Green;  break;
            case "Yellow Team": Team = TurnManager.TeamColor.Yellow; break;

            default: print("Error: " + gameObject.name + " is not assigned to a team. Object Will Be Destroyed."); break;
        }

        //Subscribe to Turn Events
        TurnManager tMan = FindObjectOfType<TurnManager>();
        tMan.PhaseStart += OnPhaseStart;
        tMan.PhaseEnd += OnPhaseEnd;
    }

    //Characters will update first, that way components (like injuries) can override and modify behaviour.
    protected virtual void OnPhaseStart(TurnManager.TeamColor CurrentTeamTurn, int TurnCount)
    {
        CurrentState = _State.Idle;
    }
    protected virtual void OnPhaseEnd(TurnManager.TeamColor CurrentTeamTurn, int TurnCount)
    {

    }

    SpriteRenderer Renderer;
    protected void Start()
    {
        //Assign a Horoscope Randomly
        if (Horoscope == Horoscopes.Random)
        { Horoscope = (Horoscopes)Random.Range(0, 11); }

        //Snap character into allignment
        transform.position = new Vector3(x, y, -1);

        //Assign the renderer component for reference
        Renderer = GetComponent<SpriteRenderer>();
    }
    protected void Update()
    {
        //float z = -1 + (y / 100f);
        //transform.position = new Vector3(x, y, z);

        //update character position and rendering order (to simulate depth)
        Renderer.sortingOrder = x - y;
        transform.position = new Vector3(x, y);
    }
}