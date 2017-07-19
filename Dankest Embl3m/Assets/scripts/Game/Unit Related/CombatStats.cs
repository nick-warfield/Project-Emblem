using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats : MonoBehaviour
{
    public int Attack = 0;
    public int HitChance = 0;
    public int CritChance = 0;
    public int DodgeChance = 0;
    public int AttackSpeed;
    public Weapons.Damage DamageType;


    private void Reset()
    {
        SetCombatStats();
    }

    //Sets the values of this class for reference later on
    public void SetCombatStats ()
    {
        RPGClass Unit = GetComponent<RPGClass>();
        int[] temp = CalculateCombatStats(Unit.Stats, Unit.WeaponStats, Unit.Inventory);

        Attack = temp[0];
        HitChance = temp[1];
        CritChance = temp[2];
        DodgeChance = temp[3];
        AttackSpeed = temp[4];
        DamageType = (Weapons.Damage)temp[5];
    }

    //calculate stats that will get passed onto the combat system
    int[] CalculateCombatStats(RPGClass.ClassStats[] UnitStats, RPGClass.ClassWeapons[] WeaponProficencies, Items[] Inventory)
    {
        //Declare some of the final stats that will get passed on.
        int attack = 0, hitChance = 0, critChance = 0, dodgeChance = 0, attackSpeed = 0;

        //Find the Equiped Weapon
        Weapons EquipedWeapon = null;// = new Weapons { WeaponCategory = Weapons.WeaponType.Unarmed };

        //loop through inventory
        for (int i = 0; i < 5; i++)
        {
            //check if slot contains a weapon
            if (Inventory[i].ItemCategory == Items.ItemType.Weapon)
            {
                print("weapon found at index " + i);
                Weapons temp = Inventory[i].GetComponent<Weapons>();

                //Check to see if the Unit is trained enough to weild the weapon
                for (int j = 0; j < 8; j++)
                {
                    if (WeaponProficencies[j].WeaponCategory == temp.WeaponCategory && WeaponProficencies[j].WeaponRank >= temp.WeaponRank)
                    { EquipedWeapon = temp; print(temp + "Equiped"); i = 10; break; }
                }
            }
        }

        //Return Unarmed Here if Applicable

        //Pull Stats from Character
        int Strength = UnitStats[(int)RPGClass.Stat.Strength].dynamicValue;
        int Magic = UnitStats[(int)RPGClass.Stat.Magic].dynamicValue;
        int Speed = UnitStats[(int)RPGClass.Stat.Speed].dynamicValue;
        int Skill = UnitStats[(int)RPGClass.Stat.Skill].dynamicValue;
        int Luck = UnitStats[(int)RPGClass.Stat.Luck].dynamicValue;
        int Bulk = UnitStats[(int)RPGClass.Stat.Bulk].dynamicValue;
        Weapons.Rank WeaponSkill = WeaponProficencies[(int)EquipedWeapon.WeaponCategory].WeaponRank;


        //Do calculations to determine the final stats
        //Determine attack Speed
        attackSpeed = Speed;
        if (Bulk >= EquipedWeapon.Weight) { attackSpeed += (Bulk - EquipedWeapon.Weight); }

        //Determine Weapon Bonuses
        int[] bonuses = GetWeaponBonuses(EquipedWeapon.WeaponCategory, WeaponSkill);

        //Determine Hit, Crit, and Dodge Chances
        hitChance = (Skill * 2) + (Luck / 2) + EquipedWeapon.HitChance;
        critChance = (Skill * 2) + (Luck / 4) + EquipedWeapon.CritChance;
        dodgeChance = (attackSpeed * 2) + Luck;

        //Determine Damage and Damage Type
        Weapons.Damage damageType = EquipedWeapon.DamageType;
        if (DamageType == Weapons.Damage.Physical)
        { attack = Strength + EquipedWeapon.Might; }
        else if (DamageType == Weapons.Damage.Magical)
        { attack = Magic + EquipedWeapon.Might; }

        //Apply Bonuses
        attack += bonuses[0];
        hitChance += bonuses[1];

        //Return Results
        return new int[6] { attack, hitChance, critChance, dodgeChance, attackSpeed, (int)damageType };
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
