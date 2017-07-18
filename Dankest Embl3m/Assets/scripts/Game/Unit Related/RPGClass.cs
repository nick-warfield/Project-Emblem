using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Character))]

public class RPGClass : MonoBehaviour
{
    //keeps track of how many times a unit has been traumatized. The more they have been traumatized, the worse the effects and the more easily it happens.
    //units get tramatized from hitting 0HP, maxing stress, and accumliating injuries.
    private int TraumaCounter = 0;

    //Holds all of the sprites/animations that this class uses during gameplay
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

        public SpriteRenderer render;
        public Color ogColor;
    }
    public spriteHolder classSprites;
    //SpriteRenderer sRender;

    //basic class stuff needed (The name of the class, the starting level, the starting amount of exp, the amount of exp dropped on death
    public string className;
    public int level = 1, exp;          //On mission start, enemy units are leveled up from 1 to their intended level so that their are slight variations in their stats
    public bool hasMoved = false;

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
        public string weaponType;   //the class of weapons that the rank is for
        public char weaponRank;     //the current rank of weapons that this unit can weild.
        public int weaponXP;        //the progress till the next rank for this weapon type.

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
        new ClassWeapons("Sword", '-', 0),      //Swords, fast physical weapons the deal the least damage. Beat axes.
        new ClassWeapons("Lance", '-', 0),      //Lances, slower but more damaging than swords. Beat swords.
        new ClassWeapons("Axe", '-', 0),        //Axes, slow but deal high damage. Beat lances.
        new ClassWeapons("Bow", '-', 0),        //Can only attack at a distance.
        new ClassWeapons("Arcane", '-', 0),     //Arcane magic, does good damage and can be used in many situations. Beats divine.
        new ClassWeapons("Divine", '-', 0),     //Divine magic, does the least damage but is incredibly effective against monsters. Reduces the stress of ony non-monster nearby.
        new ClassWeapons("Occult", '-', 0),     //Occult magic, very slow and highly damaging. Usually has an additional effect. Increases stress of the user.
        new ClassWeapons("Staff", '-', 0),      //Support magics, usually involves healing/buffing allies. Can also debuff enemies and provide added utility with teleports for example.
    };


    //the items currently in the character's inventory
    public GameObject[] inventory = new GameObject[5];

    //calculate stats that will get passed onto the combat system
    public int[] CombatStats()
    {
        int[] nums;
        bool unarmed = false;

        int Attk = 0;
        int Hit;
        int Crit;
        int Dodge;
        int AttkSpd;
        int type = 0;
        int weaponID = 0;

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

        //check weapon compatability, then add a bonus if applicable. Otherwise unit is unarmed
        char WeaponRank = inventory[0].GetComponent<Weapons>().Rank;
        string Type = inventory[0].GetComponent<Weapons>().WeaponType;

        //char[] AvailableWeapons = new char[7];
        //for (int i = 0; i < 7; i++) { AvailableWeapons[i] = useableWeapons[i].weaponRank; }

        for (int i = 0; i < 7; i++)
        {
            if (Type == useableWeapons[i].weaponType)
            {
                //WeaponRank = useableWeapons[i].weaponRank;
                if (useableWeapons[i].weaponRank == '-') { unarmed = true; }
                else if (useableWeapons[i].weaponRank == 'S') { WeaponRank = useableWeapons[i].weaponRank; }
                else if (useableWeapons[i].weaponRank <= WeaponRank) { WeaponRank = useableWeapons[i].weaponRank; }
                else { unarmed = true; }
            }
        }


        //pull weapon stats
        int Might = inventory[0].GetComponent<Weapons>().Might;
        //string Type = inventory[0].GetComponent<Weapons>().WeaponType;
        string dmgType = inventory[0].GetComponent<Weapons>().DamageType;
        int Weight = inventory[0].GetComponent<Weapons>().Weight;
        int HitW = inventory[0].GetComponent<Weapons>().Hit;
        int CritW = inventory[0].GetComponent<Weapons>().Crit;

        if (Blk - Weight < 0) { AttkSpd = Spd + (Blk - Weight); }
        else { AttkSpd = Spd; }

        switch (Type)
        {
            case ("Sword"):
                weaponID = 0;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might += 2; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 3; }

                break;

            case ("Lance"):
                weaponID = 1;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might++; HitW += 5; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 2; HitW += 5; }

                break;

            case ("Axe"):
                weaponID = 2;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { HitW += 5; }
                else if (WeaponRank == 'B') { HitW += 10; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { HitW += 15; }

                break;

            case ("Bow"):
                weaponID = 3;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might++; HitW += 5; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 2; HitW += 5; }

                break;


            case ("Arcane"):
                weaponID = 4;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might++; HitW += 5; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 2; HitW += 5; }

                break;

            case ("Divine"):
                weaponID = 5;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might++; HitW += 5; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 2; HitW += 5; }

                break;

            case ("Occult"):
                weaponID = 6;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might++; HitW += 5; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 2; HitW += 5; }

                break;

            case ("Staff"):
                weaponID = 7;

                //Weapon Rank Bonus
                if (WeaponRank == 'C') { Might++; }
                else if (WeaponRank == 'B') { Might += 2; }
                else if (WeaponRank == 'A' || WeaponRank == 'S') { Might += 3; }

                break;
        }

        switch (dmgType)
        {
            case ("Physical"):
                Attk = Str + Might;
                type = 0;
                break;

            case ("Magical"):
                Attk = Mag + Might;
                type = 1;
                break;
        }

        if (unarmed == true) { Attk = Hit = Crit = 0; AttkSpd = Spd; type = 7; print(inventory[0].GetComponent<Weapons>().Name + " is not compatable"); }
        else
        {
            Hit = (Skl * 2) + (Luk / 2) + (HitW);
            Crit = (Skl / 2) + (Luk / 4) + (CritW);
        }
        Dodge = (AttkSpd * 2) + (Luk);

        nums = new int[] { Attk, Hit, Crit, Dodge, AttkSpd, type, weaponID };
        return nums;
    }

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
                float charGrowth = GetComponent<Character>().growthRates[i].growthRate;     //the growth rate attached to the character, usually 0 for fodder enemies
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

    //calculate how far the unit can move in the map, and passes that value to the movement system
    //this will likely change depending on the type of unit. Flying units make passable tiles only cost 1 for example.
    public virtual int MovementLeft (int moveUsed, int moveCost, string tyle)
    {
        if (tyle == "Wall") { return 0; }   //walls are impassable, end check here. Return 0 tiles movable.

        int value;
        int maxMove = Stats[11].dynamicValue;   //the number of tiles this unit can travel

        moveUsed += moveCost;   //the number of tiles crossed + the cost of crossing the current tile.

        if (moveUsed <= maxMove) { value = moveCost; }      //if the movement used by this is less than the max move, then return the cost of crossing this tile.
        else { value = 0; }

        return value;
    }

    //checks a tile and adjusts the cost of traveling it so that units can have unique movement rules (eg: flying units can travel anything for 1 cost)
    public virtual int GetAdjustedF (Terrain Tile)
    {
        return Tile.MovementCost;
    }


    //bad things happen to units at max stress
    protected virtual void BreakingPoint ()
    {
        int will = Stats[9].dynamicValue;
        float RolledNum = Random.value * 100;
        int trauma = TraumaCounter * 10;

        //overcome mental stress. Either by rolling a '20' or by rolling under a doubled will stat.
        if (RolledNum <= 1 || (RolledNum + trauma) <= (will * 2))
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

    //checks to see if a unit gains an injury, then assigns it to them if they did
    public void GainInjury ()
    {
        //A table to roll on to see what kind of disability this unit will suffer, at high values this unit's death becomes certain. See Star Wars tabletop RPG for inspiration.
        //Could possibly avoid injury at a low value. Could also gain permanent disabilites, such as losing an eye or limb.
        //Trauma increases the value, making higher values more common.
    }

    //check to see if unit can move
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

    //keep hp beetween 0 and max value
    public void ClampHP ()
    {
        if (Stats[0].dynamicValue < 0) { Stats[0].dynamicValue = 0; }
        else if (Stats[0].dynamicValue > Stats[0].staticValue) { Stats[0].dynamicValue = Stats[0].staticValue; }
    }

    private void Start()
    {
        classSprites.ogColor = classSprites.render.color;
        if (tag != "Blue Team") { hasMoved = true; }
    }

    private void Update()
    {
        MoveState(hasMoved);
    }

}
