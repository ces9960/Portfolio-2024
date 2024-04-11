using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundObject : MonoBehaviour
{
    //audio source and pitch
    AudioSource source;
    public float soundPitch;

    // Start is called before the first frame update
    void Start()
    {
        //audio setup
        source = GetComponent<AudioSource>();
        source.pitch = soundPitch;
        source.mute = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSound()
    {
        //unmutes and plays the sound
        source.mute = false;
        source.Play();
    }
}
