using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatStats))]

public class RPGClass : Character
{
    //Enum for accessing the correct stat from the Stats Structure
    public enum Stat { HitPoints, StressPoints, Strength, Magic, Speed, Skill, Defense, Resistance, Constitution, Willpower, Luck, Move, Bulk, Aid };

    //basic class stuff needed (The name of the class, the starting level, the starting amount of exp, the amount of exp dropped on death
    public string className;
    public int level = 1, exp;          //On mission start, enemy units are leveled up from 1 to their intended level so that their are slight variations in their stats

    //the structure to hold all of the data a stat needs
    [System.Serializable]
    public struct ClassStats
    {
        //stat variables
        public string statName;     //the name of the stat
        public int dynamicValue;    //the in game value that has been modified by other effects (eg: buffs, terrain bonus, etc)
        public int staticValue;     //the semi permanent value of the stat. This value rarely changes, usually on level up.
        public int baseValue;       //the starting value and minimum value of a stat. The static value can never drop below this but the dynamic value can.
        public int maxValue;        //the maximum value of a stat. The static value can never exceed this, but the dynamic value can.
        public int growthRate;      //determines how likely the stat is to increases upon level up. Non-fodder enemies add to this value via their 'Character' class(script).
        public string tooltip;      //a string of text describing the stat. Should help inform the player.

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
        new ClassStats("Hit Points", 15, 15, 15, 60, 50, "-"),      //0 - How many hits the character can take before becoming unresponsive and able to take injury/death.
        new ClassStats("Stress Points", 0, 15, 15, 30, 0, "-"),     //1 - How much stress the unit can acumulate before losing it. They then act unpredicatably.
        new ClassStats("Strength", 5, 5, 5, 20, 30, "-"),           //2 - Adds damage to strength based weapons.
        new ClassStats("Magic", 5, 5, 5, 20, 30, "-"),              //3 - Adds damage to magic based weapons.
        new ClassStats("Speed", 5, 5, 5, 20, 30, "-"),              //4 - Units can attack twice if they are much faster than the other unit. The faster a unit is, the more often they dodge.
        new ClassStats("Skill", 5, 5, 5, 20, 30, "-"),              //5 - Affects crit chance and hit chance, units with high skill crit and hit more often.
        new ClassStats("Defense", 5, 5, 5, 20, 30, "-"),            //6 - Flatly reduce physical damage taken.
        new ClassStats("Resistance", 5, 5, 5, 20, 30, "-"),         //7 - Flatly reduce magic damage taken.
        new ClassStats("Constitution", 5, 5, 5, 20, 30, "-"),       //8 - Reduces the likelyhood of injury/death.
        new ClassStats("Willpower", 5, 5, 5, 20, 30, "-"),          //9 - Reduces the likelyhood of losing it. Also reduces stress taken.
        new ClassStats("Luck", 5, 5, 5, 20, 30, "-"),               //10 - Affects a variety of things. Makes a unit more likely to hit, crit, dodge, survive death's door, and not lose it.
        new ClassStats("Move", 5, 5, 5, 20, 0, "-"),                //11 - How many spaces the unit can move.
        new ClassStats("Bulk", 5, 5, 5, 20, 0, "-"),                //12 - How big the unit is. Bigger units can weild heavier weapons without a penalty and can rescue smaller units (For mounted units, having less bulk raises aid instead.).
        new ClassStats("Aid", 4, 4, 4, 20, 0, "-"),                 //13 - How big of a unit this unit can rescue.
    };

    //determines what kind of weapons the unit can use
    [System.Serializable]
    public struct ClassWeapons
    {
        public string name;
        [HideInInspector] public Weapons.WeaponType WeaponCategory;
        public Weapons.Rank WeaponRank;    //the current rank of weapons that this unit can weild.
        public int weaponXP;        //the progress till the next rank for this weapon type.

        //constructor
        public ClassWeapons(Weapons.WeaponType a, Weapons.Rank b, int c)
        {
            WeaponCategory = a;
            WeaponRank = b;
            weaponXP = c;

            name = WeaponCategory.ToString();
        }
    }

    public ClassWeapons[] WeaponStats = new ClassWeapons[8]
    {
        new ClassWeapons(Weapons.WeaponType.Sword, Weapons.Rank.Untrained, 0),      //Swords, fast physical weapons the deal the least damage. Beat axes.
        new ClassWeapons(Weapons.WeaponType.Lance, Weapons.Rank.Untrained, 0),      //Lances, slower but more damaging than swords. Beat swords.
        new ClassWeapons(Weapons.WeaponType.Axe, Weapons.Rank.Untrained, 0),        //Axes, slow but deal high damage. Beat lances.
        new ClassWeapons(Weapons.WeaponType.Bow, Weapons.Rank.Untrained, 0),        //Can only attack at a distance.
        new ClassWeapons(Weapons.WeaponType.Arcane, Weapons.Rank.Untrained, 0),     //Arcane magic, does good damage and can be used in many situations. Beats divine.
        new ClassWeapons(Weapons.WeaponType.Divine, Weapons.Rank.Untrained, 0),     //Divine magic, does the least damage but is incredibly effective against monsters. Reduces the stress of ony non-monster nearby.
        new ClassWeapons(Weapons.WeaponType.Occult, Weapons.Rank.Untrained, 0),     //Occult magic, very slow and highly damaging. Usually has an additional effect. Increases stress of the user.
        new ClassWeapons(Weapons.WeaponType.Staff, Weapons.Rank.Untrained, 0),      //Support magics, usually involves healing/buffing allies. Can also debuff enemies and provide added utility with teleports for example.
    };


    //the items currently in the character's inventory
    public Items[] Inventory = new Items[5];


    //Give the unit something to pass along to the combat manager
    public CombatStats CombatParameters;

    //when character gains 100 exp, increase level and randomly assign stat upgrades
    protected void LevelUp()
    {
        exp -= 100;     //reset exp counter
        level += 1;     //increment level

        //roll stats upgrades for each stat in the Stats array
        for (int i = 0; i < Stats.Length; i++)
        {
            //check to see if max level has been reached, roll if it hasn't been
            if (Stats[i].staticValue < Stats[i].maxValue)
            {
                float numRolled = Random.value * 100;                                       //the number rolled, each stat gets its own roll
                float charGrowth = 0f; // GetComponent<Character>().growthRates[i].growthRate;     //the growth rate attached to the character, usually 0 for fodder enemies
                float GrowthNum = Stats[i].growthRate + charGrowth;                         //class growth + character growth. The number to roll under in order to increase the stat. So the higher this number is, the more likely it is to go up.

                int statIncrease = 0;       //keep track of how much a stat has been rasied by

                //auto increase a stat if the growth is at least 100%, then reduce growth by 100% if so.
                while (GrowthNum >= 100)
                {
                    statIncrease += 1;
                    GrowthNum -= 100;
                }

                //check if the roll was less than the growth%
                if (numRolled < GrowthNum)
                {
                    statIncrease += 1;
                }

                //increase the stat in question by the statIncrease variable(never falls below 0)
                Stats[i].staticValue += statIncrease;
                Stats[i].dynamicValue += statIncrease;
            }
        }
    }



    //I'll change this when I want to adjust my pathfinding
    //checks a tile and adjusts the cost of traveling it so that units can have unique movement rules (eg: flying units can travel anything for 1 cost)
    public virtual int GetAdjustedF (Terrain Tile)
    {
        return Tile.MovementCost;
    }


    /*
    //check to see if unit can move. I'll use a state machine to handle behaviour, and an animator to handle color shifting
    public void MoveState (bool hasMoved)
    {
        if (GameObject.Find("Director").GetComponent<GridMap>().currentTurn != gameObject.tag)
        {
            classSprites.render.color = classSprites.ogColor;
        }
        else if (hasMoved)
        {
            classSprites.render.color = Color.gray;
        }

    }
    */

    //keep hp beetween 0 and max value
    public void ClampHP (int CurrentHP, int MaxHP)
    {
        if (CurrentHP < 0) { CurrentHP = 0; }
        else if (CurrentHP > MaxHP) { CurrentHP = MaxHP; }
    }


    private void Reset()
    {
        CombatParameters = GetComponent<CombatStats>();
    }
}
