using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    //Item Name and Description
    public string Name, Description;

    public enum ItemType { Weapon, Consumable }
    public ItemType ItemCategory;

    //How many times the item can be used before it breaks
    public int Uses;

    //How much it costs to buy/sell this weapon
    public int Buy;
    public int Sell;
}
