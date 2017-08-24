using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    List<RPGClass>[] Team = new List<RPGClass>[4]       //an array of lists that holds the Units on each team.
        {
            new List<RPGClass> { },
            new List<RPGClass> { },
            new List<RPGClass> { },
            new List<RPGClass> { }
        };

    int TeamActions = 1;        //an int to keep track of how many actions are left for a team. When it hits 0, the active team is swapped and the variable is reset
    int TurnCount = 1;          //keeps track of the current turn, allows for triggering stuff based on the number of turns that have passed.

    public enum TeamColor { Blue, Green, Red, Yellow};
    public TeamColor CurrentTeam = TeamColor.Blue;


    //delegate and events to let stuff know that a turn has begun and so on. still figuring this stuff out.
    public delegate void TurnEventHandler(TeamColor CurrentTeam, int TurnCount);
    public event TurnEventHandler PhaseStart;
    public event TurnEventHandler PhaseStartLate;
    public event TurnEventHandler PhaseEnd;
    public event TurnEventHandler PhaseEndLate;


    //initialize the team lists. There will usually only be 2, but this will allow up to 4 different teams with a predetermined order.
    void InitializeTeamLists ()
    {
        //find all of the units in play
        RPGClass[] AllUnits = FindObjectsOfType<RPGClass>();

        //fill out the lists with the correct team
        for (int i = 0; i < AllUnits.Length; i++)
        {
            switch (AllUnits[i].tag)
            {
                case   "Blue Team": Team[(int)TeamColor.Blue].Add(AllUnits[i]);   break;
                case  "Green Team": Team[(int)TeamColor.Green].Add(AllUnits[i]);  break;
                case    "Red Team": Team[(int)TeamColor.Red].Add(AllUnits[i]);    break;
                case "Yellow Team": Team[(int)TeamColor.Yellow].Add(AllUnits[i]); break;

                default: print("Error: " + AllUnits[i].name + " is not assigned to a team. Object Destroyed."); Destroy(AllUnits[i]); break;
            }
        }

        //initialize the starting actions to the correct value
        TeamActions = Team[0].Count;
    }
    void PrintTeams ()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0: print("Blue Team"); break;
                case 1: print("Green Team"); break;
                case 2: print("Yellow Team"); break;
                case 3: print("Red Team"); break;
            }

            if (Team[i].Count == 0)
            { print("No Units."); }
            else
            {
                for (int j = 0; j < Team[i].Count; j++)
                { print((j + 1) + ": " + Team[i][j].CharacterName + ", " + Team[i][j].className); }
            }
        }
    }

    //keeps track of how many actions are left in a turn. It actually generates a new number from scratch, based on which units are not waiting.
    int TrackActions()
    {
        int num = 0;

        for (int i = 0; i < Team[(int)CurrentTeam].Count; i++)
        { if (Team[(int)CurrentTeam][i].CurrentState != Character._State.Waiting) { num++; }  }

        return num;
    }
    //passes control to the next team that has units. does not invoke anything. Will update turn count on the start of every blue turn.
    void PassTurnControl ()
    {
        //use a do while loop to guarantee that the same turn won't get selected by default.
        do
        {
            //if the team is yellow, reset the enum. otherwise increment.
            if (CurrentTeam == TeamColor.Yellow) { CurrentTeam = TeamColor.Blue; }
            else { CurrentTeam++; }
        }
        //end the loop if a team has more than 1 unit alive.
        while (Team[(int)CurrentTeam].Count <= 0);

        //most importantly, sets TeamActions to a non-zero number to prevent a turn from instantly ending.
        TeamActions = Team[(int)CurrentTeam].Count;
        //if the current turn is blue, a full round has passed and the turn count needs to be updated
        if (CurrentTeam == TeamColor.Blue) { TurnCount++; }

        print("Turn: " + TurnCount);
        print(CurrentTeam + ", " + TeamActions);
    }


    private void Reset()
    {
        if (FindObjectsOfType<TurnManager>().Length > 1)
        {
            print("ERROR: Multiple Turn Managers Found. Ensure only one turn manager is in scene.");
            Destroy(this);
        }
    }
    void Awake()
    {
        InitializeTeamLists();
        //PrintTeams();
    }
    private void Start()
    {
        PhaseStart.Invoke(CurrentTeam, TurnCount);
    }

    // Update is called once per frame
    void Update ()
    {
        TeamActions = TrackActions();

        if (Input.GetKeyDown(KeyCode.P) || TeamActions <= 0)
        {
            //Trigger End Turn Events (eg: spawn units)
            if (PhaseEnd != null) { PhaseEnd.Invoke(CurrentTeam, TurnCount); }
            if (PhaseEndLate != null) { PhaseEndLate.Invoke(CurrentTeam, TurnCount); }

            //Pass Turn Control over to the next valid team.
            PassTurnControl();

            //Trigger Start Turn Events (eg: heal from fort)
            if (PhaseStart != null) { PhaseStart.Invoke(CurrentTeam, TurnCount); }
            if (PhaseStartLate != null) { PhaseStartLate.Invoke(CurrentTeam, TurnCount); }
        }
    }
}
