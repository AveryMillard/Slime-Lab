using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManage : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;

    [SerializeField] public bool isOnIce;

    private void Awake()
    {
        inputManager =GetComponent<InputManager>(); 
        playerLocomotion = GetComponent<PlayerLocomotion>();
        isOnIce = false;
    }

    private void Update()
    {
       inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        if (!isOnIce)
            playerLocomotion.HandleAllMovement();

    }

}
