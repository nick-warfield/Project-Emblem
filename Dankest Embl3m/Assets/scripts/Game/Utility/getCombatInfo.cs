using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getCombatInfo : MonoBehaviour
{
    public GameObject EnabledView, DisabledView;
    public Text[] Info1, Info2;
    public Image Bar1, Bar2;
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
            Info1[0].text = Stats[0].UnitReference.CharacterName;
            Info1[1].text = Stats[0].UnitReference.CombatParameters.EquipedWeapon.Name;
            Info1[2].text = Stats[0].HP.ToString();
            Info1[3].text = Stats[0].UnitReference.Stats[0].staticValue.ToString();
            Info1[4].text = Stats[0].Damage.ToString();
            Info1[5].text = Stats[0].HitChance.ToString();
            Info1[6].text = Stats[0].CritChance.ToString();
            if (Stats[0].AttackCount > 1) { Info1[7].text = "x" + Stats[0].AttackCount; } else { Info1[7].text = ""; }

            Info2[0].text = Stats[1].UnitReference.CharacterName;
            Info2[1].text = Stats[1].UnitReference.CombatParameters.EquipedWeapon.Name;
            Info2[2].text = Stats[1].HP.ToString();
            Info2[3].text = Stats[1].UnitReference.Stats[0].staticValue.ToString();
            Info2[4].text = Stats[1].Damage.ToString();
            Info2[5].text = Stats[1].HitChance.ToString();
            Info2[6].text = Stats[1].CritChance.ToString();
            if (Stats[1].AttackCount > 1) { Info2[7].text = "x" + Stats[1].AttackCount; } else { Info2[7].text = ""; }


            float percent = (float)Stats[0].HP / Stats[0].UnitReference.Stats[0].staticValue;
            Bar1.rectTransform.localScale = new Vector3(1, percent, 1);

            percent = (float)Stats[1].HP / Stats[1].UnitReference.Stats[0].staticValue;
            Bar2.rectTransform.localScale = new Vector3(1, percent, 1);


            /*
            Info[0].text =  Stats[0].UnitReference.CharacterName +
                            "\n" + Stats[0].UnitReference.CombatParameters.EquipedWeapon.Name +
                            "\n" +
                            "\n" + Stats[0].HP +
                            "\n" + Stats[0].Damage +
                            "\n" + Stats[0].HitChance +
                            "\n" + Stats[0].CritChance;
            if (Stats[0].AttackCount > 1) { Info[2].text = "x" + Stats[0].AttackCount; }
            else { Info[2].text = ""; }

            Info[1].text =  "\n\n" +
                            "\n" + Stats[1].HP +
                            "\n" + Stats[1].Damage +
                            "\n" + Stats[1].HitChance +
                            "\n" + Stats[1].CritChance +
                            "\n" +
                            "\n" + Stats[1].UnitReference.CombatParameters.EquipedWeapon.Name +
                            "\n" + Stats[1].UnitReference.CharacterName;
            if (Stats[1].AttackCount > 1) { Info[3].text = "x" + Stats[1].AttackCount; }
            else { Info[3].text = ""; }
            */
        }
    }
}
