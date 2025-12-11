using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntimedStealthMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerManager pManagerRef;

    Timer timerRef;
    Tip tipRef;
    GameObject playerObjectRef;
    GameObject tipObjectRef;
    void Start()
    {

        playerObjectRef = GameObject.Find("Player");
        tipObjectRef = GameObject.Find("Tip");
        tipRef = tipObjectRef.GetComponent<Tip>();
        pManagerRef = playerObjectRef.GetComponent<PlayerManager>();

    }

    public void Die()
    {
        pManagerRef.TakeLives(1);
        pManagerRef.Respawn();
    }
    //Location of enemies should reset here // TO DO    
}
