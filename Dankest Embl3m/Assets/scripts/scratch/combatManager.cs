using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class combatManager : MonoBehaviour
{
    //GameObject[,,] mapRef;
    public Camera cam1;
    public Camera cam2;

    public GameObject LeftUnit;
    public GameObject RightUnit;

    public Text LeftText;
    public Text RightText;

    public GameObject attackingUnit;
    public GameObject defendingUnit;

    public GameObject attackerTerrain;
    public GameObject defenderTerrain;

    public bool runCombat = false;
    bool playAnime = false;
    float[] timeStamp;

    struct CombatResults
    {
        public GameObject AttackingUnit;
        public GameObject DefendingUnit;

        public int AttackerStartHP;
        public int DefenderStartHP;

        public bool AttackPhaseHit;
        public bool AttackPhaseCrit;
        public int AttackPhaseDamage;

        public bool DefendPhaseHit;
        public bool DefendPhaseCrit;
        public int DefendPhaseDamage;

        public GameObject BonusPhaseAttacker;
        public bool BonusPhaseHit;
        public bool BonusPhaseCrit;
        public int BonusPhaseDamage;

        //constructor
        public CombatResults (GameObject AU, GameObject DU, int aHP, int dHP, bool AH, bool AC, int AD, bool DH, bool DC, int DD, GameObject BU, bool BH, bool BC, int BD)
        {
            AttackingUnit = AU;
            DefendingUnit = DU;
            AttackerStartHP = aHP;
            DefenderStartHP = dHP;
            AttackPhaseHit = AH;
            AttackPhaseCrit = AC;
            AttackPhaseDamage = AD;
            DefendPhaseHit = DH;
            DefendPhaseCrit = DC;
            DefendPhaseDamage = DD;
            BonusPhaseAttacker = BU;
            BonusPhaseHit = BH;
            BonusPhaseCrit = BC;
            BonusPhaseDamage = BD;
        }
    }
    CombatResults CombatStack;


	// Use this for initialization
	void Start ()
    {
        //mapRef = GameObject.Find("Director").GetComponent<GridMap>().MAP;
        timeStamp = new float[] { 0, 0, 0, 0 };
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (runCombat == true)
        {
            cam1.enabled = false;
            cam2.enabled = true;


            CombatStack = combatCalc(attackingUnit, attackerTerrain, defendingUnit, defenderTerrain);
            attackingUnit = attackerTerrain = defendingUnit = defenderTerrain = null;


            playAnime = true;
            runCombat = false;
        }


        if (playAnime)
        {
            playAnimation(CombatStack);
        }


        if (Time.time >= timeStamp[3] && timeStamp[3] != 0)
        {

            cam1.enabled = true;
            cam2.enabled = false;

            timeStamp[0] = timeStamp[1] = timeStamp[2] = timeStamp[3] = 0;
            playAnime = false;
        }
	}

    CombatResults combatCalc (GameObject AU, GameObject AT, GameObject DU, GameObject DT)
    {
        int aHP = AU.GetComponent<RPGClass>().Stats[0].dynamicValue;
        int dHP = DU.GetComponent<RPGClass>().Stats[0].dynamicValue;

        bool phase1H = false;       //hit flags for each combat phase, used in animation
        bool phase2H = false;
        bool phase3H = false;

        bool phase1C = false;       //crit flags for each combat phase, used in animation
        bool phase2C = false;
        bool phase3C = false;

        int phase1D = 0;            //final damage for each combat phase, used in animation
        int phase2D = 0;
        int phase3D = 0;

        GameObject BU = null;       //the unit that attacks twice, if any


        //int[attack, hit, crit, dodge, spd, damageType(0=physical/1=magical), weapondID(0sword/1lance/2axe/3bow/4arcane/5light/6dark/7staff]
        int[] AUStats = new int[7]; //AU.GetComponent<RPGClass>().CombatStats();
        int[] DUStats = new int[7]; //DU.GetComponent<RPGClass>().CombatStats();

        int spdDiff = AUStats[4] - DUStats[4];                                                          //compares the speed between two units, allows for a unit to attack twice
        int AUHit = AUStats[1] - DUStats[3] - defenderTerrain.GetComponent<Terrain>().DodgeBonus;       //determines how likely the attacker is going to hit
        int DUHit = DUStats[1] - AUStats[3] - attackerTerrain.GetComponent<Terrain>().DodgeBonus;       //determines how likely the defender is going to hit
        int AUDef = attackerTerrain.GetComponent<Terrain>().DefenseBonus;                               //adds any defense bonus from terrain to the attacker
        int DUDef = defenderTerrain.GetComponent<Terrain>().DefenseBonus;                               //adds any defense bonus from terrain to the defender

        if (AUStats[5] == 0) { DUDef += DU.GetComponent<RPGClass>().Stats[6].dynamicValue; }            //grabs defense for physical attacks
        else { DUDef += DU.GetComponent<RPGClass>().Stats[7].dynamicValue; }                            //grabs resistance for magical attacks

        if (DUStats[5] == 0) { AUDef += AU.GetComponent<RPGClass>().Stats[6].dynamicValue; }            //grabs defense for physical attacks
        else { AUDef += AU.GetComponent<RPGClass>().Stats[7].dynamicValue; }                            //grabs resistance for magical attacks

        //weapon triangle effects
        if (AUStats[6] != 3 && AUStats[6] != 7)
        {
            int adv = AUStats[6] - 1;
            int disadv = AUStats[6] + 1;
            int otherW = DUStats[6];

            if (adv == -1) { adv = 2; } else if (adv == 3) { adv = 6; }
            if (disadv == 3) { disadv = 0; } else if (disadv == 7) { disadv = 4; }

            if (adv == otherW)
            {
                AUDef++;
                AUHit += 15;

                DUDef--;
                DUHit -= 15;
                print("Advantage");
            }
            else if (disadv == otherW)
            {
                AUDef--;
                AUHit -= 15;

                DUDef++;
                DUHit += 15;
                print("Disadvantage");
            }
        }

        int AUDmg = AUStats[0] - DUDef;
        int DUDmg = DUStats[0] - AUDef;

        //Healing
        if (AUStats[6] == 7)
        {
            if (AU.tag == DU.tag) { AUDmg = -AUStats[0]; AUHit = 100; DUDmg = 0; spdDiff = 0; print(AUDmg); }
            else { AUDmg = 0; }
        }

        //Attacker Phase
        if (Random.Range(0, 100) <= AUHit)
        {
            phase1H = true;

            if (Random.Range(0, 100) <= AUStats[2])
            {
                phase1C = true;
                phase1D = AUDmg * 3;
                DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= phase1D;
            }
            else
            {
                phase1D = AUDmg;
                DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= AUDmg;
            }

            //print("ATTACKER HIT");
        }

        //Defender Phase
        if (Random.Range(0, 100) <= DUHit)
        {
            phase2H = true;

            if (Random.Range(0, 100) <= DUStats[2])
            {
                phase2C = true;
                phase2D = DUDmg * 3;
                AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= phase2D;
            }
            else
            {
                phase2D = DUDmg;
                AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= DUDmg;
            }

            //print("DEFENDER HIT");
        }

        //Bonus Phase
        if (spdDiff > 5)
        {
            BU = AU;

            //Attacker Bonus
            if (Random.Range(0, 100) <= AUHit)
            {
                phase3H = true;

                if (Random.Range(0, 100) <= AUStats[2])
                {
                    phase3C = true;
                    phase3D = AUDmg * 3;
                    DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= phase3D;
                }
                else
                {
                    phase3D = AUDmg;
                    DU.GetComponent<RPGClass>().Stats[0].dynamicValue -= AUDmg;
                }

                //print("ATTACKER BONUS");
            }
        }
        else if (spdDiff < -5)
        {
            BU = DU;

            //Defender Bonus
            if (Random.Range(0, 100) <= DUHit)
            {
                phase3H = true;

                if (Random.Range(0, 100) <= DUStats[2])
                {
                    phase3C = true;
                    phase3D = DUDmg * 3;
                    AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= (DUDmg * 3);
                }
                else
                {
                    phase3D = DUDmg;
                    AU.GetComponent<RPGClass>().Stats[0].dynamicValue -= DUDmg;
                }

                //print("DEFENDER BONUS");
            }
        }

        CombatResults hey = new CombatResults(AU, DU, aHP, dHP, phase1H, phase1C, phase1D, phase2H, phase2C, phase2D, BU, phase3H, phase3C, phase3D);
        return hey;
    }

    void playAnimation(CombatResults cr)
    {
        //GameObject cmb = GameObject.Find("Combat Stuff");
        //SpriteRenderer rend;
        if (timeStamp[0] == 0 && timeStamp[1] == 0 && timeStamp[2] == 0 && timeStamp[3] == 0)
        {
            Sprite aSpr = cr.AttackingUnit.GetComponent<RPGClass>().SpriteSheets.Attack[0];
            Sprite dSpr = cr.DefendingUnit.GetComponent<RPGClass>().SpriteSheets.Attack[0];

            LeftUnit.GetComponent<SpriteRenderer>().sprite = aSpr;
            RightUnit.GetComponent<SpriteRenderer>().sprite = dSpr;

            LeftText.text = "HP: " + cr.AttackerStartHP + "/" + cr.AttackingUnit.GetComponent<RPGClass>().Stats[0].staticValue;
            RightText.text = "HP: " + cr.DefenderStartHP+ "/" + cr.DefendingUnit.GetComponent<RPGClass>().Stats[0].staticValue;

            timeStamp[0] = Time.time + 1f;
        }

        //first animation
        if (Time.time >= timeStamp[0] && timeStamp[0] != 0)
        {
            LeftUnit.GetComponent<Animator>().SetTrigger("Start");

            if (cr.AttackPhaseHit)
            {
                if (cr.AttackPhaseCrit) { print(cr.AttackingUnit + " CRIT"); }
                else { print(cr.AttackingUnit + " Hit"); }

                cr.DefenderStartHP -= cr.AttackPhaseDamage;
                RightText.text = "HP: " + cr.DefenderStartHP + "/" + cr.DefendingUnit.GetComponent<RPGClass>().Stats[0].staticValue;
                print("and did " + cr.AttackPhaseDamage + " damage");
            }
            else { print(cr.AttackingUnit + " Missed"); }

            timeStamp[1] = timeStamp[0] + 1f;
            timeStamp[0] = 0;
        }

        //second animation
        if (Time.time >= timeStamp[1] && timeStamp[1] != 0)
        {
            RightUnit.GetComponent<Animator>().SetTrigger("Start");

            if (cr.DefendPhaseHit)
            {
                if (cr.DefendPhaseCrit) { print(cr.DefendingUnit + " CRIT"); }
                else { print(cr.DefendingUnit + " Hit"); }

                cr.AttackerStartHP -= cr.DefendPhaseDamage;
                LeftText.text = "HP: " + cr.AttackerStartHP + "/" + cr.AttackingUnit.GetComponent<RPGClass>().Stats[0].staticValue;
                print("and did " + cr.DefendPhaseDamage + " damage");
            }
            else { print(cr.DefendingUnit + " Missed"); }

            timeStamp[2] = timeStamp[1] + 1f;
            timeStamp[1] = 0;
        }

        //possible third animation
        if (Time.time >= timeStamp[2] && timeStamp[2] != 0)
        {
            if (cr.BonusPhaseAttacker != null)
            {
                if (cr.BonusPhaseAttacker == cr.AttackingUnit)
                { LeftUnit.GetComponent<Animator>().SetTrigger("Start"); }
                else
                { RightUnit.GetComponent<Animator>().SetTrigger("Start"); }

                if (cr.BonusPhaseHit)
                {
                    if (cr.BonusPhaseCrit) { print(cr.BonusPhaseAttacker + " CRIT"); }
                    else { print(cr.BonusPhaseAttacker + " Hit"); }

                    if (cr.BonusPhaseAttacker == cr.DefendingUnit)
                    {
                        cr.AttackerStartHP -= cr.BonusPhaseDamage + cr.AttackPhaseDamage;
                        LeftText.text = "HP: " + cr.AttackerStartHP + "/" + cr.AttackingUnit.GetComponent<RPGClass>().Stats[0].staticValue;
                    }
                    else
                    {
                        cr.DefenderStartHP -= cr.BonusPhaseDamage + cr.DefendPhaseDamage;
                        RightText.text = "HP: " + cr.DefenderStartHP + "/" + cr.DefendingUnit.GetComponent<RPGClass>().Stats[0].staticValue;
                    }
                    print("and did " + cr.BonusPhaseDamage + " damage");
                }
                else { print(cr.BonusPhaseAttacker + " Missed"); }

                timeStamp[3] = timeStamp[2] + 1f;
                timeStamp[2] = 0;
            }
            else { timeStamp[3] = timeStamp[2] + 0.2f; timeStamp[2] = 0; }
        }

        //cr.AttackingUnit.GetComponent<RPGClass>().ClampHP();
        //cr.DefendingUnit.GetComponent<RPGClass>().ClampHP();
    }
}
