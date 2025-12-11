using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAbility : AbilitySO{

    GameObject Bomb=null;
    
    public void OnEnable(){
        name=abilityNames.Bomb;
        cooldown=3f;
        icon=Resources.Load<Sprite>("AbilityIcons/Bomb");
    }

    public override void activate(AbilityHotbar.AbilityState[] states, float[] cooldowns, int id, Transform trans){
     //   Debug.Log("Boosting!");
        Bomb=new GameObject("Bomb");
        Bomb.tag = "Bomb";
        Bomb.AddComponent<Bomb>();
        Bomb.AddComponent<Rigidbody>();
        SphereCollider sphereCollider=Bomb.AddComponent<SphereCollider>();
        sphereCollider.radius = 1f;
        MeshFilter meshFilter = Bomb.AddComponent<MeshFilter>();

        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx"); // Example: Use a sphere mesh
        MeshRenderer meshRenderer = Bomb.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Locker_002"); // Example: Use a standard material

        // Set the bomb's position
        Bomb.transform.position = trans.position + new Vector3(0, 5, 0); // Spawn at the spawner's position
        Bomb.transform.localScale = new Vector3(1f, 1f, 1f);
        states[id]=AbilityHotbar.AbilityState.cooldown;
        cooldowns[id]=cooldown;
    }

}
