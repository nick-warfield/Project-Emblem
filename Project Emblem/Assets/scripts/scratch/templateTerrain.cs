using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class templateTerrain : MonoBehaviour
{
    public string tyleName;

    public int defaultSlow;
    public float dodgeBonus;
    public int defenseBonus;
    //public float p_heal;

    public GameObject unitPresent = null;


	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        //unitPresent.GetComponent<UnitStats>().
	}

    private void OnTriggerEnter(Collider other)
    {
        if (unitPresent == null)
        {
            unitPresent = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        unitPresent = null;
    }
}
