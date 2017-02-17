using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use requirecomponent to auto assign all the components needed to build a character
[RequireComponent(typeof(RPGClass))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]

public class Character : MonoBehaviour
{
    //the flavour and housekeeping stuff of the unit
    public Sprite portrait;
    public string characterName, description, horoscope;

    public string support = "Support Struct Goes Here";

    [System.Serializable]
    public struct CharGrowthRates
    {
        public string statName;
        public int growthRate;

        //constructor
        public CharGrowthRates(string a, int b)
        {
            statName = a;
            growthRate = b;
        }
    }

    public CharGrowthRates[] growthRates = new CharGrowthRates[14]
    {
        new CharGrowthRates("Hit Points", 0),
        new CharGrowthRates("Stress Points", 0),
        new CharGrowthRates("Strength", 0),
        new CharGrowthRates("Magic", 0),
        new CharGrowthRates("Speed", 0),
        new CharGrowthRates("Skill", 0),
        new CharGrowthRates("Defense", 0),
        new CharGrowthRates("Resistance", 0),
        new CharGrowthRates("Constitution", 0),
        new CharGrowthRates("Willpower", 0),
        new CharGrowthRates("Luck", 0),
        new CharGrowthRates("Move", 0),
        new CharGrowthRates("Bulk", 0),
        new CharGrowthRates("Aid", 0)
    };

    //used to instantiate default values in editor once a component is added/reset to a gameobject
    //set up sprites next
    void Reset ()
    {
        tag = "Red Team";
        gameObject.layer = 13;
        transform.position = new Vector3(0, 0, -0.5f);

        Rigidbody charRB = gameObject.GetComponent<Rigidbody>();
        BoxCollider charBOX = gameObject.GetComponent<BoxCollider>();

        charRB.useGravity = false;
        charRB.isKinematic = true;
        charRB.constraints = RigidbodyConstraints.FreezePositionZ;
        charRB.freezeRotation = true;

        charBOX.center = new Vector3(0.5f, -0.5f, 0);
        charBOX.size = new Vector3(0.5f, 0.5f, 2);
    }
}