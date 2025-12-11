using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class backgroundMusic : MonoBehaviour
{

    public AudioClip[] bgMusic;
    private AudioSource source;
    public Collider bossStartCollider; 
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void playBackgroundMusic()
    {

        GameObject roomPrefab = GameObject.FindGameObjectWithTag("Room");
        AudioClip clip;
    
        if (roomPrefab.name == "Boss Room") //player is not in boss room
        {
            clip = bgMusic[1];
            source.clip = clip;
            source.Play();
        }
        else //player is in boss room
        {
            clip = bgMusic[0];
            source.clip = clip;
            source.Play();
        }
    }
}
