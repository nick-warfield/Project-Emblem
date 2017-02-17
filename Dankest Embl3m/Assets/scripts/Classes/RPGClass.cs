using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Character))]

public class RPGClass : MonoBehaviour
{
    //private variables
    private int TraumaCounter = 0;

    [System.Serializable]
    public struct spriteHolder
    {
        public Sprite classPortrait;
        public Sprite[] idle;
        public Sprite[] selected;
        public Sprite[] walkSide;
        public Sprite[] walkFront;
        public Sprite[] walkBack;
        public Sprite[] combatSprite;
        public Sprite[] criticalHit;
    }
    public spriteHolder classSprites;

    //basic class stuff needed
    public string className;
    public int level = 1, exp;

    //the numberic stats of a character
    [System.Serializable]
    public struct ClassStats
    {
        //stat variables
        public string statName;
        public int dynamicValue;
        public int staticValue;
        public int baseValue;
        public int maxValue;
        public int growthRate;
        public string tooltip;

        //constructor
        public ClassStats(string a, int b, int c, int d, int e, int f, string g)
        {
            statName = a;
            dynamicValue = b;
            staticValue = c;
            baseValue = d;
            maxValue = e;
            growthRate = f;
            tooltip = g;
        }
    }

    //set up an array to hold each stat, then initialize some default values
    public ClassStats[] Stats = new ClassStats[14]
    {
        new ClassStats("Hit Points", 15, 15, 15, 60, 50, "-"),      //0
        new ClassStats("Stress Points", 0, 15, 15, 30, 0, "-"),     //1
        new ClassStats("Strength", 5, 5, 5, 20, 30, "-"),           //2
        new ClassStats("Magic", 5, 5, 5, 20, 30, "-"),              //3
        new ClassStats("Speed", 5, 5, 5, 20, 30, "-"),              //4
        new ClassStats("Skill", 5, 5, 5, 20, 30, "-"),              //5
        new ClassStats("Defense", 5, 5, 5, 20, 30, "-"),            //6
        new ClassStats("Resistance", 5, 5, 5, 20, 30, "-"),         //7
        new ClassStats("Constitution", 5, 5, 5, 20, 30, "-"),       //8
        new ClassStats("Willpower", 5, 5, 5, 20, 30, "-"),          //9
        new ClassStats("Luck", 5, 5, 5, 20, 30, "-"),               //10
        new ClassStats("Move", 5, 5, 5, 20, 0, "-"),                //11
        new ClassStats("Bulk", 5, 5, 5, 20, 0, "-"),                //12
        new ClassStats("Aid", 4, 4, 4, 20, 0, "-"),                 //13
    };


    //determines what kind of weapons the unit can use
    [System.Serializable]
    public struct ClassWeapons
    {
        public string weaponType;
        public char weaponRank;
        public int weaponXP;

        //constructor
        public ClassWeapons(string a, char b, int c)
        {
            weaponType = a;
            weaponRank = b;
            weaponXP = c;
        }
    }

    public ClassWeapons[] useableWeapons = new ClassWeapons[8]
    {
        new ClassWeapons("Sword", '-', 0),
        new ClassWeapons("Lance", '-', 0),
        new ClassWeapons("Axe", '-', 0),
        new ClassWeapons("Bow", '-', 0),
        new ClassWeapons("Arcane", '-', 0),
        new ClassWeapons("Light", '-', 0),
        new ClassWeapons("Dark", '-', 0),
        new ClassWeapons("Staff", '-', 0),
    };


    //the items currently in the character's inventory
    public GameObject[] inventory = new GameObject[5];

    //calculate stats that will get passed onto the combat system
    protected int[] CambatStats()
    {
        int[] nums;

        int Attk = 0;
        int Hit;
        int Crit;
        int Dodge;

        //pull character stats
        int HP = Stats[0].dynamicValue;
        int Str = Stats[2].dynamicValue;
        int Mag = Stats[3].dynamicValue;
        int Skl = Stats[5].dynamicValue;
        int Spd = Stats[4].dynamicValue;
        int Def = Stats[6].dynamicValue;
        int Res = Stats[7].dynamicValue;
        int Luk = Stats[10].dynamicValue;
        int Blk = Stats[12].dynamicValue;

        //pull weapon stats
        int Might = 0;      //inventory[0].Weapon.Might;
        string Type = "Sword";
        int Weight = 0;
        int HitW = 0;
        int CritW = 0;

        if (Blk - Weight < 0) { Spd = Spd + (Blk - Weight); }

        switch (Type)
        {
            case ("Sword"):
            case ("Lance"):
            case ("Axe"):
            case ("Bow"):
                Attk = Str + Might;
                break;

            case ("Arcane"):
            case ("Light"):
            case ("Dark"):
            case ("Staff"):
                Attk = Mag + Might;
                break;
        }

        Hit = (Skl * 2) + (Luk / 2) + (HitW);
        Crit = (Skl / 2) + (Luk / 4) + (CritW);
        Dodge = (Spd) + (Luk);

        nums = new int[] { Attk, Hit, Crit, Dodge };
        return nums;
    }

    //when character gains 100 exp, increase level and randomly assign stat upgrades
    protected void LevelUp()
    {
        exp -= 100;
        level += 1;

        //roll stats upgrades
        for (int i = 0; i < Stats.Length; i++)
        {
            if (Stats[i].staticValue < Stats[i].maxValue)
            {
                float numRolled = Random.value * 100;
                float charGrowth = GetComponent<Character>().growthRates[i].growthRate;
                float GrowthNum = Stats[i].growthRate + charGrowth;

                int statIncrease = 0;

                while (GrowthNum >= 100)
                {
                    statIncrease += 1;
                    GrowthNum -= 100;
                }

                if (numRolled < GrowthNum)
                {
                    statIncrease += 1;
                }

                Stats[i].staticValue += statIncrease;
                Stats[i].dynamicValue += statIncrease;
            }
        }
    }

    //bad things happen to units at max stress
    protected virtual void BreakingPoint ()
    {
        int will = Stats[9].dynamicValue;
        float RolledNum = Random.value * 100;
        int trauma = TraumaCounter * 10;

        //overcome mental stress
        if (RolledNum <= 1 || (RolledNum + trauma) <= (will * 2))
        {
            //bunch of stat boosts
        }
        else
        {
            TraumaCounter += 1;
            //player loses total control over this unit
        }
    }

    //bad things happen to units that hit 0HP
    protected virtual void DeathsDoor ()
    {
        int con = Stats[8].dynamicValue;
        float RolledNum = Random.value * 100;
        int trauma = TraumaCounter * 10;

        //get up if really lucky
        if (RolledNum <= 1)
        {
            Stats[0].dynamicValue = Stats[0].staticValue / 2;
        }

        //kill unit if they are real unlucky
        if (RolledNum + trauma > (con * 4) || RolledNum >= 99)
        {
            Destroy(gameObject);
        }

        //a chance to remain unscathed
        if (RolledNum + trauma > (con * 2))
        {
            TraumaCounter += 1;
            GainInjury();
        }
    }

    //
    public void GainInjury ()
    {

    }
}
