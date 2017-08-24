using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnBoolNotReset : MonoBehaviour {

    public bool flagChecked = true;
	
	// Update is called once per frame
	void Update ()
    {
		if (!flagChecked) { Destroy(gameObject); }

        flagChecked = false;
	}
}
