using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats2 : MonoBehaviour
{
    public int Health = 0;
    public int Stress = 0;

    public int Attack = 0;
    public int HitChance = 0;
    public int CritChance = 0;

    public int Dodge = 0;
    public int CriticalDodge = 0;
    public int AttackSpeed = 0;

    public Weapons.Damage DamageType;

    public Weapons EquipedWeapon;

    public int Defense;
    public int Resistance;



    //Sets the values of this class for reference later on
    public void SetCombatStats (RPGClass Unit)
    {
        //RPGClass Unit = GetComponent<RPGClass>();

        EquipedWeapon = EquipWeapon(Unit.Inventory, Unit.WeaponStats);
        Health = Unit.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue;
        Stress = Unit.Stats[(int)RPGClass.Stat.StressPoints].dynamicValue;

        int[] temp = CalculateCombatStats(Unit.Stats, Unit.WeaponStats, EquipedWeapon);
        Attack = temp[0];
        HitChance = temp[1];
        CritChance = temp[2];
        Dodge = temp[3];
        AttackSpeed = temp[4];
        DamageType = (Weapons.Damage)temp[5];
        Defense = temp[6];
        Resistance = temp[7];

        CriticalDodge = Unit.Stats[(int)RPGClass.Stat.Luck].dynamicValue;
    }

    //Equip the first elligible weapon
    Weapons EquipWeapon(Items[] Inventory, RPGClass.ClassWeapons[] WeaponProficencies)
    {
        //loop through inventory
        for (int i = 0; i < 5; i++)
        {
            //check if slot contains a weapon
            if (Inventory[i] != null && Inventory[i].ItemCategory == Items.ItemType.Weapon)
            {
                print("weapon found at index " + i);
                Weapons temp = Inventory[i].GetComponent<Weapons>();

                //Check to see if the Unit is trained enough to weild the weapon
                for (int j = 0; j < 8; j++)
                {
                    if (WeaponProficencies[j].WeaponCategory == temp.WeaponCategory && WeaponProficencies[j].WeaponRank >= temp.WeaponRank)
                    { print(temp + "Equiped"); i = 10; return temp; }
                }
            }
        }

        return null;
    }

    //calculate stats that will get passed onto the combat system
    int[] CalculateCombatStats(RPGClass.ClassStats[] UnitStats, RPGClass.ClassWeapons[] WeaponProficencies, Weapons Weapon)
    {
        //Declare some of the final stats that will get passed on.
        int attack = 0, hitChance = 0, critChance = 0, dodgeChance = 0, attackSpeed = 0;

        //Find the Equiped Weapon
        //Weapons EquipedWeapon = EquipWeapon(Inventory, WeaponProficencies);

        //Return Unarmed Here if Applicable

        //Pull Stats from Character
        int Strength = UnitStats[(int)RPGClass.Stat.Strength].dynamicValue;
        int Magic = UnitStats[(int)RPGClass.Stat.Magic].dynamicValue;
        int Speed = UnitStats[(int)RPGClass.Stat.Speed].dynamicValue;
        int Skill = UnitStats[(int)RPGClass.Stat.Skill].dynamicValue;
        int Luck = UnitStats[(int)RPGClass.Stat.Luck].dynamicValue;
        int Bulk = UnitStats[(int)RPGClass.Stat.Bulk].dynamicValue;
        Weapons.Rank WeaponSkill = WeaponProficencies[(int)Weapon.WeaponCategory].WeaponRank;


        //Do calculations to determine the final stats
        //Determine attack Speed
        attackSpeed = Speed;
        if (Bulk < Weapon.Weight) { attackSpeed += (Bulk - Weapon.Weight); }

        //Determine Weapon Bonuses
        int[] bonuses = GetWeaponBonuses(Weapon.WeaponCategory, WeaponSkill);

        //Determine Hit, Crit, and Dodge Chances
        hitChance = (Skill * 2) + (Luck / 2) + Weapon.HitChance;
        critChance = (Skill * 2) + (Luck / 4) + Weapon.CritChance;
        dodgeChance = (attackSpeed * 2) + Luck;

        //Determine Damage and Damage Type
        Weapons.Damage damageType = Weapon.DamageType;
        if (DamageType == Weapons.Damage.Physical)
        { attack = Strength + Weapon.Might; }
        else if (DamageType == Weapons.Damage.Magical)
        { attack = Magic + Weapon.Might; }

        //Apply Bonuses
        attack += bonuses[0];
        hitChance += bonuses[1];

        //Grab Defenses
        int defense = UnitStats[(int)RPGClass.Stat.Defense].dynamicValue;
        int resistance = UnitStats[(int)RPGClass.Stat.Resistance].dynamicValue;

        //Return Results
        return new int[8] { attack, hitChance, critChance, dodgeChance, attackSpeed, (int)damageType, defense, resistance};
    }

    //Determine Weapon Bonuses
    int[] GetWeaponBonuses(Weapons.WeaponType EquipedWeaponType, Weapons.Rank CharacterWeaponRank)
    {
        //Reward some combination of extra damage or hit damage
        int attackBonus = 0, HitBonus = 0;

        //Check Weapon type first as different bonuses are rewarded to different weapons. Then Check weapon rank, higher rank = better bonus
        switch (EquipedWeaponType)
        {
            case (Weapons.WeaponType.Sword):
            case (Weapons.WeaponType.Staff):
                switch (CharacterWeaponRank)
                {
                    case (Weapons.Rank.C):
                        attackBonus = 1;
                        break;

                    case (Weapons.Rank.B):
                        attackBonus = 2;
                        break;

                    case (Weapons.Rank.A):
                        attackBonus = 3;
                        break;

                    case (Weapons.Rank.S):
                        attackBonus = 3;
                        break;
                }
                break;

            case (Weapons.WeaponType.Lance):
            case (Weapons.WeaponType.Bow):
            case (Weapons.WeaponType.Arcane):
            case (Weapons.WeaponType.Divine):
            case (Weapons.WeaponType.Occult):
                switch (CharacterWeaponRank)
                {
                    case (Weapons.Rank.C):
                        attackBonus = 1;
                        break;

                    case (Weapons.Rank.B):
                        attackBonus = 1;
                        HitBonus = 5;
                        break;

                    case (Weapons.Rank.A):
                        attackBonus = 2;
                        HitBonus = 5;
                        break;

                    case (Weapons.Rank.S):
                        attackBonus = 2;
                        HitBonus = 5;
                        break;
                }
                break;

            case (Weapons.WeaponType.Axe):
                switch (CharacterWeaponRank)
                {
                    case (Weapons.Rank.C):
                        HitBonus = 5;
                        break;

                    case (Weapons.Rank.B):
                        HitBonus = 10;
                        break;

                    case (Weapons.Rank.A):
                        HitBonus = 15;
                        break;

                    case (Weapons.Rank.S):
                        HitBonus = 15;
                        break;
                }
                break;
        }

        //return the bonuses
        return new int[2] { attackBonus, HitBonus };
    }
}
