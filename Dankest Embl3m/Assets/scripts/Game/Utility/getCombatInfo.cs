using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getCombatInfo : MonoBehaviour
{
    public GameObject EnabledView, DisabledView;
    public Text[] Info;
    CombatManager.DamageRelated[] Stats = new CombatManager.DamageRelated[2];

    public void PassStats(CombatManager.DamageRelated Attacker, CombatManager.DamageRelated Defender)
    {
        Stats[0] = Attacker; Stats[1] = Defender;
        transform.SetParent(EnabledView.transform);
        //transform.parent = EnabledView.transform;
    }
    public void CloseMenu()
    {
        transform.SetParent(DisabledView.transform);
        //transform.parent = DisabledView.transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Stats[0].UnitReference != null && Stats[1].UnitReference != null)
        {
            Info[0].text =  Stats[0].UnitReference.CharacterName +
                            "\n" + Stats[0].UnitReference.CombatParameters.EquipedWeapon.Name +
                            "\n" +
                            "\n" + Stats[0].HP +
                            "\n" + Stats[0].Damage +
                            "\n" + Stats[0].HitChance +
                            "\n" + Stats[0].CritChance;

            Info[1].text =  "\n\n" +
                            "\n" +
                            "\n" + Stats[1].HP +
                            "\n" + Stats[1].Damage +
                            "\n" + Stats[1].HitChance +
                            "\n" + Stats[1].CritChance +
                            "\n" + Stats[1].UnitReference.CombatParameters.EquipedWeapon.Name +
                            "\n" + Stats[1].UnitReference.CharacterName;
        }
    }
}
