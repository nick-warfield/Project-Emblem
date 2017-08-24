using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class versionNumber : MonoBehaviour
{
    // Use this for initialization
	void Start ()
    {
        UnityEngine.UI.Text txt = GetComponent<UnityEngine.UI.Text>();
        txt.text = Application.version;
	}
}
