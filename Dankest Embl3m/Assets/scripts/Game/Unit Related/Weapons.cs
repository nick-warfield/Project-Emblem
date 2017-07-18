using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    //These stats identify the weapon and what are the requirements to use it
    public string Name;
    public enum WeaponType { Sword, Axe, Lance, Arcane, Divine, Occult, Bow, Staff };
    public WeaponType WeaponCategory;
    public enum Damage { Physical, Magical };
    public Damage DamageType;
    public enum Rank { Untrained, E, D, C, B, A, S };
    public Rank WeaponRank = Rank.E;

    //Theses stats determines the damage, range, weight, etc of the weapon
    public int Might;
    public int minRange = 1;
    public int maxRange = 1;
    public int HitChance = 90;
    public int CritChance;
    public int Weight;
    public int Durability;

    //How much it costs to buy/sell this weapon
    public int Buy;
    public int Sell;
}
