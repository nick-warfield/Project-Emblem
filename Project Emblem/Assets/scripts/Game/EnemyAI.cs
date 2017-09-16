using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    TurnManager tMan;
    List<RPGClass> UnitList;
    int Actions;

    //This is the Finite State Machine that serves as the AI. It checks for stuff in range and then attacks it maybe.
    void FSM(RPGClass Unit)
    {
        print("State Machine Ran");
    }


    //Set every unit to waiting in order to end the turn
    void EndTurn(List<RPGClass> team)
    {
        for (int i = 0; i < team.Count; i++)
        { team[i].CurrentState = Character._State.Waiting; }
    }

    private void Awake()
    {
        //Subscirbe to events
        tMan = FindObjectOfType<TurnManager>();
        tMan.PhaseStart += OnPhaseStart;
        tMan.PhaseStartLate += OnPhaseStartLate;
        tMan.PhaseEnd += OnPhaseEnd;
    }
    private void OnPhaseStart(TurnManager.TeamColor CurrentTeam, int TurnCount)
    {
        //If it is the enemy phase, do some stuff
        if (CurrentTeam == TurnManager.TeamColor.Red)
        {
            //set list and actions
            UnitList = tMan.Team[(int)CurrentTeam];
            Actions = UnitList.Count;

            //create and save seed here

            //sort and cull unit list here

            //loop through list and run state machine on each unit
            for (int i = 0; i < Actions; i++)
            { FSM(UnitList[i]); }
        }
    }
    private void OnPhaseStartLate(TurnManager.TeamColor CurrentTeam, int TurnCount)
    {
        if (CurrentTeam == TurnManager.TeamColor.Red)
        {
            //set the rest of the units to waiting to trigger turn swap. Do this late to get around units setting themselves active
            EndTurn(tMan.Team[(int)CurrentTeam]);
        }
    }
    private void OnPhaseEnd(TurnManager.TeamColor CurrentTeam, int TurnCount)
    {
        if (CurrentTeam == TurnManager.TeamColor.Red)
        {
            UnitList.Clear();

            //Delete Seed Here
        }
    }
}
