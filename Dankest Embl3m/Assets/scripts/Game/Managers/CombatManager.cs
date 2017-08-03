using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : TacticsBehaviour
{
    public AnimationParameters LeftSide, RightSide;
    //public UnityEngine.UI.Text LeftText, RightText;

    AttackResults[] TheStack;
    public bool SimulateCombat = false;
    float TimeStamp = 0;
    public Camera mainCam, combatCam;

    DamageRelated AttackerDamageParameters, DefenderDamageParameters;


    //Sets up all the variables/structs needed locally, but can be triggered from another class
    public void InitializeCombatParameters (RPGClass Attacker, RPGClass Defender, Terrain AttackerTerrain, Terrain DefenderTerrain)
    {
        //Set up my probabilities and damage including resistances
        AttackerDamageParameters = ProbableDamage(Attacker.CombatParameters, Defender.CombatParameters, AttackerTerrain, DefenderTerrain);
        DefenderDamageParameters = ProbableDamage(Defender.CombatParameters, Attacker.CombatParameters, DefenderTerrain, AttackerTerrain);

        //Set up the 'sides', that way world space can be somewhat reflected in the animations and attacker/defender only get to be on one specific side
        if (Attacker.x > Defender.x || Attacker.y > Defender.y)
        {
            RightSide.Unit = Attacker; RightSide.Probabilites = AttackerDamageParameters;
            LeftSide.Unit = Defender; LeftSide.Probabilites = DefenderDamageParameters;
        }
        else
        {
            RightSide.Unit = Defender; RightSide.Probabilites = DefenderDamageParameters;
            LeftSide.Unit = Attacker; LeftSide.Probabilites = AttackerDamageParameters;
        }

        //Display a window showing combat information
        getCombatInfo menu = FindObjectOfType<getCombatInfo>();
        menu.PassStats(AttackerDamageParameters, DefenderDamageParameters);
    }

    public void StartCombat()
    {
        //Initialize Stack
        TheStack = GetCombatResults(AttackerDamageParameters, DefenderDamageParameters);

        //Assign Sprites to each side
        LeftSide.AnimationObject.GetComponent<SpriteRenderer>().sprite = LeftSide.Unit.GetComponent<SpriteRenderer>().sprite;
        RightSide.AnimationObject.GetComponent<SpriteRenderer>().sprite = RightSide.Unit.GetComponent<SpriteRenderer>().sprite;

        //Swap Cameras
        combatCam.enabled = true;
        mainCam.enabled = false;

        //Set Flag to start animating
        TimeStamp = Time.time + 1.75f;
        SimulateCombat = true;

        //Disable the menu
        getCombatInfo menu = FindObjectOfType<getCombatInfo>();
        menu.CloseMenu();
    }
    void EndCombat()
    {
        //Swap Cameras
        mainCam.enabled = true;
        combatCam.enabled = false;

        //Set flag to stop animating
        TimeStamp = 0;
        SimulateCombat = false;
    }

    private void Update()
    {
        if (SimulateCombat)
        {
            if (Time.time >= TimeStamp)
            {
                if (TheStack.Length > 0)
                {
                    TheStack = StartAnimations(TheStack, LeftSide, RightSide);
                    TimeStamp = Time.time + 1.25f;
                }
                else
                { EndCombat(); }
            }

            LeftSide.HPUI.text = "HP: " + LeftSide.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue + "/" + LeftSide.Unit.Stats[(int)RPGClass.Stat.HitPoints].staticValue;
            LeftSide.StatsUI.text = "Hit: " + LeftSide.Probabilites.HitChance + "\nCrit: " + LeftSide.Probabilites.CritChance + "\nDamage: " + LeftSide.Probabilites.Damage;

            RightSide.HPUI.text = "HP: " + RightSide.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue + "/" + RightSide.Unit.Stats[(int)RPGClass.Stat.HitPoints].staticValue;
            RightSide.StatsUI.text = "Hit: " + RightSide.Probabilites.HitChance + "\nCrit: " + RightSide.Probabilites.CritChance + "\nDamage: " + RightSide.Probabilites.Damage;
        }
    }


    [System.Serializable]
    public struct AnimationParameters
    {
        public RPGClass Unit;
        public GameObject AnimationObject;

        public UnityEngine.UI.Text HPUI, StatsUI, NameUI;

        public GameObject TerrainFloor;
        public GameObject TerrainBackground;

        public DamageRelated Probabilites;
    }

    //Runs an attack and then returns the combat left so that it can be fed back in with correct timings
    AttackResults[] StartAnimations (AttackResults[] Attacks, AnimationParameters Left, AnimationParameters Right)
    {
        if (Attacks[0].Unit == Left.Unit)
        {
            Left.AnimationObject.GetComponent<Animator>().SetTrigger("Start");
            Right.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue -= Attacks[0].DamageDealt;
            Right.Unit.Stats[(int)RPGClass.Stat.StressPoints].dynamicValue += (Attacks[0].DamageDealt - (Right.Unit.Stats[(int)RPGClass.Stat.Willpower].dynamicValue / 2) );
            //print(Right.Unit.Stats[(int)RPGClass.Stat.StressPoints].dynamicValue);

            Right.Unit.ClampHP();
            Right.Unit.ClampStress();

            if (Right.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue <= 0)
            {
                if (Right.Unit.FodderCharacter)
                { Right.Unit.Die(); }
                else
                {
                    Right.Unit.DeathsDoor(Right.Unit.Stats[(int)RPGClass.Stat.Constitution].dynamicValue,
                                          Right.Unit.Stats[(int)RPGClass.Stat.Luck].dynamicValue,
                                          Right.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue,
                                          Right.Unit.Stats[(int)RPGClass.Stat.HitPoints].staticValue);
                }
            }
        }
        else
        {
            Right.AnimationObject.GetComponent<Animator>().SetTrigger("Start");
            Left.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue -= Attacks[0].DamageDealt;
            Left.Unit.Stats[(int)RPGClass.Stat.StressPoints].dynamicValue += (Attacks[0].DamageDealt - (Left.Unit.Stats[(int)RPGClass.Stat.Willpower].dynamicValue / 2) );
            //print(Left.Unit.Stats[(int)RPGClass.Stat.StressPoints].dynamicValue);

            Left.Unit.ClampHP();
            Left.Unit.ClampStress();

            if (Left.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue <= 0)
            {
                if (Left.Unit.FodderCharacter)
                { Left.Unit.Die(); }
                else
                {
                    Left.Unit.DeathsDoor(Left.Unit.Stats[(int)RPGClass.Stat.Constitution].dynamicValue,
                                            Left.Unit.Stats[(int)RPGClass.Stat.Luck].dynamicValue,
                                            Left.Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue,
                                            Left.Unit.Stats[(int)RPGClass.Stat.HitPoints].staticValue);
                }
            }
        }

        List<AttackResults> Reduced = new List<AttackResults> { };
        for (int i = 1; i < Attacks.Length; i++)
        { Reduced.Add(Attacks[i]); }

        return Reduced.ToArray();
    }

    //struct to store the results of an attack
    struct AttackResults
    {
        public RPGClass Unit;
        public int DamageDealt;
        public bool Miss;

        public AttackResults(int dmg, bool isMiss, DamageRelated AttackingUnit)
        {
            DamageDealt = dmg;
            Miss = isMiss;

            Unit = AttackingUnit.UnitReference;
        }
    }
    //Simulates an attack, then returns the damage
    AttackResults AttackRoll (DamageRelated Attacker)
    {
        int Roll = Random.Range(0, 100), Damage = 0;
        bool Missed = false;

        if (Roll <= Attacker.CritChance)
        { Damage = Attacker.Damage * 3; }
        else if (Roll <= Attacker.HitChance)
        { Damage = Attacker.Damage; }
        else { Missed = true; }

        return new AttackResults(Damage, Missed, Attacker);
    }
    //Actually calculate the combat and it's results
    AttackResults[] GetCombatResults(DamageRelated Attacker, DamageRelated Defender)
    {
        List<AttackResults> AttackList = new List<AttackResults> { };

        //Attacker Phase
        if (Attacker.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Attacker));
            Attacker.AttackCount--;
            Defender.HP -= AttackList[AttackList.Count - 1].DamageDealt;

            print(Attacker.UnitReference.name + " Attacked: Dealt " + AttackList[AttackList.Count - 1].DamageDealt);
            if (Defender.HP <= 0) { return AttackList.ToArray(); }
        }

        //Defender Phase
        if (Defender.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Defender));
            Defender.AttackCount--;
            Attacker.HP -= AttackList[AttackList.Count - 1].DamageDealt;

            print(Defender.UnitReference.name + " Attacked. Dealt: " + AttackList[AttackList.Count - 1].DamageDealt);

            if (Attacker.HP <= 0) { return AttackList.ToArray(); }
        }

        //Bonus Phase
        if (Attacker.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Attacker));
            Attacker.AttackCount--;
            Defender.HP -= AttackList[AttackList.Count - 1].DamageDealt;

            print(Attacker.UnitReference.name + " Attacked: Dealt " + AttackList[AttackList.Count - 1].DamageDealt);
        }
        else if (Defender.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Defender));
            Defender.AttackCount--;
            Attacker.HP -= AttackList[AttackList.Count - 1].DamageDealt;

            print(Defender.UnitReference.name + " Attacked: Dealt " + AttackList[AttackList.Count - 1].DamageDealt);
        }

        return AttackList.ToArray();
    }

    //Contains the stats relevant to dealing damage
    public struct DamageRelated
    {
        public RPGClass UnitReference;

        public int HP, Damage, HitChance, CritChance, AttackCount;
        public bool Advantage, Disadvantage;

        public DamageRelated (RPGClass Unit, int hp, int damage, int hit, int crit, int attacks, int advantage)
        {
            UnitReference = Unit;
            HP = hp;

            Damage = damage;
            HitChance = hit;
            CritChance = crit;
            AttackCount = attacks;

            Advantage = Disadvantage = false;

            if (advantage > 0) { Advantage = true; }
            else if (advantage < 0) { Disadvantage = true; }
        }
        public DamageRelated(RPGClass Unit, int hp, int damage, int hit, int crit, int attacks)
        {
            UnitReference = Unit;
            HP = hp;

            Damage = damage;
            HitChance = hit;
            CritChance = crit;
            AttackCount = attacks;

            Advantage = Disadvantage = false;
        }
    };
    //figure out attacker parameters relative to the defender.
    DamageRelated ProbableDamage(RPGClass.CombatStats Attacker, RPGClass.CombatStats Defender, Terrain AttackerTerrain, Terrain DefenderTerrain)
    {
        //Don't attack with a staff, it's for healing not hurting! (This only basic staves)
        if (Attacker.EquipedWeapon.WeaponCategory == Weapons.WeaponType.Staff && Attacker.EquipedWeapon.Effect == Weapons.WeaponEffect.Basic)
        { return Heal(Attacker, Defender); }

        int Hit = Attacker.HitChance - Defender.Dodge - DefenderTerrain.DodgeBonus;   //Hit chance = hit - dodge - dodge bonus
        int Crit = Attacker.CritChance - Defender.CriticalDodge;    //Crit chance = crit - crit dodge
        int Damage = Attacker.Attack;       //Damage = strength/magic + might

        //Reduce damage by defenders defense/resistance according to attacker damage type
        if (Attacker.DamageType == Weapons.Damage.Physical)
        { Damage -= (Defender.Defense + DefenderTerrain.DefenseBonus); }
        else
        { Damage -= (Defender.Resistance + DefenderTerrain.DefenseBonus); }

        //Determine the number of attacks a unit has, by default 1
        int AttackCount = 1;

        //If the attacker is signifigantly faster than the defender
        if (Attacker.AttackSpeed > Defender.AttackSpeed + 5)
        { AttackCount = 2; }

        //If the attacker has a special weapon
        if (Attacker.EquipedWeapon.Effect == Weapons.WeaponEffect.DoubleStrike)
        { AttackCount *= 2; }

        //Get Distance
        int distance = Mathf.Abs(AttackerTerrain.x - DefenderTerrain.x) + Mathf.Abs(AttackerTerrain.y - DefenderTerrain.y);
        //Check Range
        if (distance > Attacker.EquipedWeapon.maxRange || distance < Attacker.EquipedWeapon.minRange)
        { AttackCount = 0; }
        
        //Calculate weapon triangle effect, then apply them
        int[] Modifiers = WeaponTriangleEffect(Attacker.EquipedWeapon, Defender.EquipedWeapon);
        Damage += Modifiers[0];
        Hit += Modifiers[1];

        //Finally, clamp final values so that they make sense to the player
        if (Damage < 0) { Damage = 0; }
        if (Crit < 0) { Crit = 0; } else if (Crit > 100) { Crit = 100; }
        if (Hit < 0) { Hit = 0; } else if (Hit > 100) { Hit = 100; }

        return new DamageRelated(Attacker.UnitReference, Attacker.UnitReference.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue, Damage, Hit, Crit, AttackCount, Modifiers[2]);
    }
    //Heals are bit different but similar enough, most notable is that you can't heal enemies and you can never heal more than once at a time
    DamageRelated Heal(RPGClass.CombatStats Healer, RPGClass.CombatStats Other)
    {
        int Hit = 100, Crit = 0, HealAmount = Healer.Attack, HealCount;

        if (Other.UnitReference.tag != Healer.UnitReference.tag) { HealCount = 0; }
        else { HealCount = 1; }

        return new DamageRelated(Healer.UnitReference, Healer.Health, HealAmount, Hit, Crit, HealCount);
    }

    //Calculate the weapon triangle effects
    int[] WeaponTriangleEffect(Weapons AttackerWeapon, Weapons DefenderWeapon)
    {
        Weapons.WeaponType dType = DefenderWeapon.WeaponCategory;

        int DamageModifier = 0, HitModifier = 0;
        int Advantage = 0;  //since bools can't be null: -1 = Disadvantage, 0 = neutral, +1 Advantage

        //Switch based on the attackers weapon type
        switch (AttackerWeapon.WeaponCategory)
        {
            //Then switch on defender's weapon type. Grant an advantage/disadvantage as appropriate
            case Weapons.WeaponType.Sword:
                if (dType == Weapons.WeaponType.Axe) { DamageModifier = 1; HitModifier = 15; Advantage = 1; }
                else if (dType == Weapons.WeaponType.Lance) { DamageModifier = -1; HitModifier = -15; Advantage = -1; }
                break;
            case Weapons.WeaponType.Lance:
                if (dType == Weapons.WeaponType.Sword) { DamageModifier = 1; HitModifier = 15; Advantage = 1; }
                else if (dType == Weapons.WeaponType.Axe) { DamageModifier = -1; HitModifier = -15; Advantage = -1; }
                break;
            case Weapons.WeaponType.Axe:
                if (dType == Weapons.WeaponType.Lance) { DamageModifier = 1; HitModifier = 15; Advantage = 1; }
                else if (dType == Weapons.WeaponType.Sword) { DamageModifier = -1; HitModifier = -15; Advantage = -1; }
                break;

            case Weapons.WeaponType.Arcane:
                if (dType == Weapons.WeaponType.Divine) { DamageModifier = 1; HitModifier = 15; Advantage = 1; }
                else if (dType == Weapons.WeaponType.Occult) { DamageModifier = -1; HitModifier = -15; Advantage = -1; }
                break;
            case Weapons.WeaponType.Divine:
                if (dType == Weapons.WeaponType.Occult) { DamageModifier = 1; HitModifier = 15; Advantage = 1; }
                else if (dType == Weapons.WeaponType.Arcane) { DamageModifier = -1; HitModifier = -15; Advantage = -1; }
                break;
            case Weapons.WeaponType.Occult:
                if (dType == Weapons.WeaponType.Arcane) { DamageModifier = 1; HitModifier = 15; Advantage = 1; }
                else if (dType == Weapons.WeaponType.Divine) { DamageModifier = -1; HitModifier = -15; Advantage = -1; }
                break;
        }

        return new int[3] { DamageModifier, HitModifier, Advantage };
    }
}
