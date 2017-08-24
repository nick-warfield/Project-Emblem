using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipToLoop : MonoBehaviour
{
    public AudioClip LoopedSection;
    AudioSource Player;

	// Use this for initialization
	void Start ()
    { Player = GetComponent<AudioSource>(); }
	
	// Update is called once per frame
	void Update ()
    {
		if (!Player.isPlaying)
        {
            Player.clip = LoopedSection;
            Player.loop = true;
            Player.Play();

            Destroy(this);
        }
	}
}
