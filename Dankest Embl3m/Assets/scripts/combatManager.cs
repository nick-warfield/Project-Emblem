using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatManager : MonoBehaviour
{
    GameObject[,,] mapRef;
    public Camera cam1;
    public Camera cam2;

    public GameObject attackingUnit;
    public GameObject defendingUnit;

    public GameObject attackerTerrain;
    public GameObject defenderTerrain;

    public bool runCombat = false;
    float timeStamp = 0;


	// Use this for initialization
	void Start ()
    {
        mapRef = GameObject.Find("Director").GetComponent<GridMap>().MAP;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (runCombat == true)
        {
            cam1.enabled = false;
            cam2.enabled = true;


            combatCalc(attackingUnit, attackerTerrain, defendingUnit, defenderTerrain);
            playAnimation();

            runCombat = false;
        }

        if (Time.time >= timeStamp)
        {
            cam1.enabled = true;
            cam2.enabled = false;
        }
	}

    void combatCalc (GameObject AU, GameObject AT, GameObject DU, GameObject DT)
    {
        //int[attack, hit, crit, dodge, spd, type(0=physical/1=magical)]
        int[] AUStats = AU.GetComponent<RPGClass>().CombatStats();
        int[] DUStats = DU.GetComponent<RPGClass>().CombatStats();

        int spdDiff = AUStats[4] - DUStats[4];
        int AUHit = AUStats[1] - DUStats[3];
        int DUHit = DUStats[1] - AUStats[3];
        int AUDef = 0;
        int DUDef = 0;

        if (AUStats[5] == 0) { DUDef = DU.GetComponent<RPGClass>().Stats[6].dynamicValue; }
        else { DUDef = DU.GetComponent<RPGClass>().Stats[7].dynamicValue; }

        if (DUStats[5] == 0) { AUDef = AU.GetComponent<RPGClass>().Stats[6].dynamicValue; }
        else { AUDef = AU.GetComponent<RPGClass>().Stats[7].dynamicValue; }

        int AUDmg = AUStats[0] - DUDef;
        int DUDmg = DUStats[0] - AUDef;

        //Attacker Phase
        if (Random.Range(0, 100) <= AUHit)
        {
            if (Random.Range(0, 100) <= AUStats[2])
            { DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= (AUDmg * 3); }
            else
            { DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= AUDmg; }

            print("ATTACKER HIT");
        }

        //Defender Phase
        if (Random.Range(0, 100) <= DUHit)
        {
            if (Random.Range(0, 100) <= DUStats[2])
            { AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= (DUDmg * 3); }
            else
            { AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= DUDmg; }

            print("DEFENDER HIT");
        }

        //Bonus Phase
        if (spdDiff > 5)
        {
            //Attacker Bonus
            if (Random.Range(0, 100) <= AUHit)
            {
                if (Random.Range(0, 100) <= AUStats[2])
                { DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= (AUDmg * 3); }
                else
                { DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= AUDmg; }
                print("ATTACKER BONUS");
            }
        }
        else if (spdDiff < -5)
        {
            //Defender Bonus
            if (Random.Range(0, 100) <= DUHit)
            {
                if (Random.Range(0, 100) <= DUStats[2])
                { AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= (DUDmg * 3); }
                else
                { AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= DUDmg; }
                print("DEFENDER BONUS");
            }
        }

    }

    void playAnimation()
    {
        timeStamp = Time.time + 3;
        attackingUnit = attackerTerrain = defendingUnit = defenderTerrain = null;
    }
}
