using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionEnd : MonoBehaviour
{
    bool enabled;
    Boss bossObj;
    // Start is called before the first frame update
    void Start()
    {
        enabled = true;
        bossObj = GameObject.Find("Boss").GetComponent<Boss>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null && enabled)
        {
            enabled = false;
            bossObj.EndIntermission();
        }
    }
}
