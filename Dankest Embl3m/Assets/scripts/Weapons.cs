using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    //These stats identify the weapon and what are the requirements to use it
    public string Name;
    public string WeaponType;
    public string DamageType;
    public char Rank;

    //Theses stats determines the damage, range, weight, etc of the weapon
    public int Might;
    public int minRange;
    public int maxRange;
    public int Hit;
    public int Crit;
    public int Weight;
    public int Durability;

    //How much it costs to buy/sell this weapon
    public int Buy;
    public int Sell;
}
