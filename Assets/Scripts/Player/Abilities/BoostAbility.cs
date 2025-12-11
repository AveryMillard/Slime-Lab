using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostAbility : AbilitySO{

    GameObject playerObject;
    Rigidbody rb;
    GameObject ActualBoost=null;
    bool initialized = false;
    public override void Initialize(GameObject player)
    {
        playerObject = player;
        Debug.Log("BoostAbility initialized with player: " + player.name);
        initialized = true;
    }

    public void OnEnable(){
        name=abilityNames.Boost;
        cooldown=3f;
        icon=Resources.Load<Sprite>("AbilityIcons/Boost");
        Debug.Log("Assigned references!"); 
    }

    public override void activate(AbilityHotbar.AbilityState[] states, float[] cooldowns, int id, Transform trans){
        //   Debug.Log("Boosting!");
        if (initialized)
        {
            playerObject.AddComponent<BoostCoroutiner>();
            states[id] = AbilityHotbar.AbilityState.cooldown;
            cooldowns[id] = cooldown;
        }
        else Debug.Log("Error: Boost hasn't been initialized yet");
    }

}
