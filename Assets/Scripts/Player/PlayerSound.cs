using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    private playerMovement playerMovement;

    private AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        playerMovement = GetComponent<playerMovement>();
    }

    
    void PlayFootstep()
    {
        if(playerMovement.iceToggle == false)
        {
            AudioClip clip = footstepSounds[(int)Random.Range(0, footstepSounds.Length)];
            source.clip = clip;

            source.Play();
            source.volume = Random.Range(0.02f, 0.05f);
            source.pitch = Random.Range(0.8f, 1.2f);
        }
    }
}
