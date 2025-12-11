using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="Ability", menuName ="Scriptable Objects/Ability", order =1)]
 public class AbilitySO : ScriptableObject
{
    [Header("Properties")]
    public abilityNames name;
    public float cooldown;
    public Sprite icon;

    public bool toggleable=false;
    public virtual void activate(AbilityHotbar.AbilityState[] states, float[] cooldowns, int id, Transform trans){
        
        Debug.Log("Missing ability code");
    }

    public virtual void Initialize(GameObject player)
    {
        Debug.Log("Initialize function hasn't been set up.");
    }

}

public enum abilityNames {Boost, Hook, Bomb}
