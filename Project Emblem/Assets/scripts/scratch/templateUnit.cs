using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(templateClass))]

public class templateUnit : MonoBehaviour
{
    //the flavour and housekeeping stuff of the unit
    public Sprite unitPortrait;
    public string unitName, unitClass, description, Horoscope;
    //public templateClass unitClass;
    public int level, exp;

    //the numberic stats of a unit
    [System.Serializable]
    public struct UnitStats
    {  
        public string statName;
        public int dynamicValue;
        public int staticValue;
        public int baseValue;
        public int maxValue;
        public int growthRate;
        public string tooltip;
    }
    /* 
 *  0 = HP
 *  1 = Stress
 *  2 = Strength
 *  3 = Magic
 *  4 = Speed
 *  5 = Skill
 *  6 = Defense
 *  7 = Resistance
 *  8 = Constitition
 *  9 = Willpower
 * 10 = Luck
 * 11 = Move
 * 12 = Bulk
 * 13 = Aid
 */
    public UnitStats[] STATS;


    //determines what kind of weapons the unit can use
    [System.Serializable]
    public struct WeaponStats
    {
        public string weaponType;
        public char weaponRank;
        public int weaponXP;
    }
    /*
     * 0 - Sword
     * 1 - Lance
     * 2 - Axe
     * 3 - Bow
     * 4 - Arcane
     * 6 - Light
     * 7 - Dark
     * 8 - Staff
     */
    public WeaponStats[] WEAPONSTATS;


    //the items currently in the units inventory
    public GameObject[] inventory;


    [System.Serializable]
    public struct SupportStats
    {
        public string otherUnit;
        public GameObject otherUnitReference;
        public int supportValue;
        public char supportRank;
        public string[] thoughts;
    }
    public SupportStats[] UNITSUPPORTS;

    //private Vector2 startPoint;     //starting tile
    //private Vector2 endPoint;       //grabs start tile, does math, next start tile
    //private float speedLerp;        //physically moves unit tile from start tile to end tile

    //public bool isUsingMagic = false;

    //inventory
    //slot 1
    //slot 2
    //slot 3
    //slot 4
    //slot 5



    //public void batonPass ()
    //{
    //    int HP = currentHP;

    //    int attack;
    //    bool isMagAttack;

    //    if (isUsingMagic)
    //    {
    //        attack = magic;
    //        isMagAttack = true;
    //    }
    //    else
    //    {
    //        attack = strength;
    //        isMagAttack = false;
    //    }

    //    int def = defense;
    //    int res = resistance;
    //    int spd = speed;
    //    float crit = skill / 100;
    //}



    // Use this for initialization
    void Start ()
    {
        //STATS[13].staticValue = STATS[12].staticValue - 1;
        //unitClass = currentClass.ToString();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
