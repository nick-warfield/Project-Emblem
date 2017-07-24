using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getUnitInfoAdvanced : MonoBehaviour
{
    RPGClass unitRef;
    public GameObject DisabledView;
    public GameObject EnabledView;

    public Text[] Columns;


    public void PassStats(RPGClass Character)
    {
        unitRef = Character;

        transform.parent = EnabledView.transform;
    }
    public void CloseMenu()
    { transform.parent = DisabledView.transform; }

    // Update is called once per frame
    void Update()
    {
        if (unitRef != null)
        {
            Columns[0].text =   unitRef.CharacterName +
                                "\n" + unitRef.Horoscope.ToString() +
                                "\n\n\n\n\n\n" +
                                "\n" + unitRef.className +
                                "\nLVL: " + unitRef.level + "\tXP: " + unitRef.exp +
                                "\n" +
                                "\nHP: " + unitRef.Stats[(int)RPGClass.Stat.HitPoints].dynamicValue + " / " + unitRef.Stats[(int)RPGClass.Stat.HitPoints].staticValue +
                                "\nSTRESS: " + unitRef.Stats[(int)RPGClass.Stat.Strength].dynamicValue + " / " + unitRef.Stats[(int)RPGClass.Stat.Strength].staticValue;

            Columns[1].text =   "STRENGTH: " + unitRef.Stats[(int)RPGClass.Stat.Strength].dynamicValue +
                                "\nMAGIC: " + unitRef.Stats[(int)RPGClass.Stat.Magic].dynamicValue +
                                "\nSPEED: " + unitRef.Stats[(int)RPGClass.Stat.Speed].dynamicValue +
                                "\nSKILL: " + unitRef.Stats[(int)RPGClass.Stat.Skill].dynamicValue +
                                "\nLUCK: " + unitRef.Stats[(int)RPGClass.Stat.Luck].dynamicValue +
                                "\nMOVE: " + unitRef.Stats[(int)RPGClass.Stat.Move].dynamicValue +
                                "\n" +
                                "\n[COMBAT STATS]" +
                                "\n" + unitRef.CombatParameters.EquipedWeapon.Name +
                                "\nATTACK: " + unitRef.CombatParameters.Attack +
                                "\nHIT: " + unitRef.CombatParameters.HitChance + "\nCRIT: " + unitRef.CombatParameters.CritChance +
                                "\nATTACK SPEED: " + unitRef.CombatParameters.AttackSpeed +
                                "\nDODGE: " + unitRef.CombatParameters.Dodge;

            //Get item names from inventory
            string[] items = new string[5];
            for (int i = 0; i < 5; i++)
            {
                if (unitRef.Inventory[i] != null) { items[i] = unitRef.Inventory[i].Name; }
                else { items[i] = "EMPTY"; }
            }

            Columns[2].text =   "DEFENSE: " + unitRef.Stats[(int)RPGClass.Stat.Defense].dynamicValue +
                                "\nRESISTANCE: " + unitRef.Stats[(int)RPGClass.Stat.Resistance].dynamicValue +
                                "\nWILLPOWER: " + unitRef.Stats[(int)RPGClass.Stat.Willpower].dynamicValue +
                                "\nCONSTITUTION: " + unitRef.Stats[(int)RPGClass.Stat.Constitution].dynamicValue +
                                "\nBULK: " + unitRef.Stats[(int)RPGClass.Stat.Bulk].dynamicValue +
                                "\nAID: " + unitRef.Stats[(int)RPGClass.Stat.Aid].dynamicValue +
                                "\n" +
                                "\n[INVENTORY]" +
                                "\n" + items[0] +
                                "\n" + items[1] +
                                "\n" + items[2] +
                                "\n" + items[3] +
                                "\n" + items[4];
        }
    }

    // Use this for initialization
    void Start ()
    {
        
	}
}
