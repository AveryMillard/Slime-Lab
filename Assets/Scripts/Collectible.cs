using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioSource collectSound;
    LevelGenManager lvlManagerScript;
    PlayerManager playermanager;
    Map map;
    GameObject mapObj;

    private void Start()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        GameObject lvlManager = GameObject.FindGameObjectWithTag("LevelGen");
        lvlManagerScript = lvlManager.GetComponent<LevelGenManager>();
        playermanager = Player.GetComponent<PlayerManager>();
        mapObj = GameObject.Find("miniMapRaw");
        map = mapObj.GetComponent<Map>();
        
        Room curRoom = lvlManagerScript.getRoom(lvlManagerScript.rooms, playermanager.playerCoords);

        if (!lvlManagerScript.collectibleCoords.Contains(playermanager.playerCoords))
        {
            gameObject.SetActive(false);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with Collectible");
        collectSound.Play();
        //Debug.Log(collectSound.name + " played");
        PlayerManager player = other.GetComponent<PlayerManager>();
        MeshRenderer mesh = GetComponent<MeshRenderer>();

        if (player != null)
        {
            player.IncreaseCollectibleCount();
            lvlManagerScript.collectibleCoords.Remove(playermanager.playerCoords);
            map.ReloadCollectible(playermanager.playerCoords);
            mesh.enabled = false;
            StartCoroutine(DeactivateAfterSound());
        }
    }
    private IEnumerator DeactivateAfterSound()
    {
        collectSound.Play();
        Debug.Log(collectSound.name + " played");
        yield return new WaitForSeconds(collectSound.clip.length/2);
        gameObject.SetActive(false);
    }
    
}
