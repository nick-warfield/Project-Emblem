using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : TacticsBehaviour
{
    //CombatStats Attacker, Defender;
    //Terrain AttackerTerrain, DefenderTerrain;


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
            if (Defender.HP <= 0) { return AttackList.ToArray(); }
        }

        //Defender Phase
        if (Defender.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Defender));
            Defender.AttackCount--;
            Attacker.HP -= AttackList[AttackList.Count - 1].DamageDealt;
            if (Attacker.HP <= 0) { return AttackList.ToArray(); }
        }

        //Bonus Phase
        if (Attacker.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Attacker));
            Attacker.AttackCount--;
            Defender.HP -= AttackList[AttackList.Count - 1].DamageDealt;
        }
        else if (Defender.AttackCount > 0)
        {
            AttackList.Add(AttackRoll(Defender));
            Defender.AttackCount--;
            Attacker.HP -= AttackList[AttackList.Count - 1].DamageDealt;
        }

        return AttackList.ToArray();
    }


    //Contains the stats relevant to dealing damage
    struct DamageRelated
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
    DamageRelated ProbableDamage(CombatStats Attacker, CombatStats Defender, Terrain AttackerTerrain, Terrain DefenderTerrain)
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
        
        //Calculate weapon triangle effect, then apply them
        int[] Modifiers = WeaponTriangleEffect(Attacker.EquipedWeapon, Defender.EquipedWeapon);
        Damage += Modifiers[0];
        Hit += Modifiers[1];

        //Finally, clamp final values so that they make sense to the player
        if (Damage < 0) { Damage = 0; }
        if (Crit < 0) { Crit = 0; } else if (Crit > 100) { Crit = 100; }
        if (Hit < 0) { Hit = 0; } else if (Hit > 100) { Hit = 100; }

        return new DamageRelated(Attacker.GetComponent<RPGClass>(), Attacker.Health, Damage, Hit, Crit, AttackCount, Modifiers[2]);
    }
    //Heals are bit different but similar enough, most notable is that you can't heal enemies and you can never heal more than once at a time
    DamageRelated Heal(CombatStats Healer, CombatStats Other)
    {
        int Hit = 100, Crit = 0, HealAmount = Healer.Attack, HealCount;

        if (Other.tag != Healer.tag) { HealCount = 0; }
        else { HealCount = 1; }

        return new DamageRelated(Healer.GetComponent<RPGClass>(), Healer.Health, HealAmount, Hit, Crit, HealCount);
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
