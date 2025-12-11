using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AbilityHotbar : MonoBehaviour
{
    static int abilityCount=3;
    public static AbilityHotbar Instance {get; set;}
    public AbilitySO[] hotbar= new AbilitySO[abilityCount];
    public AbilityState[] states= new AbilityState[abilityCount];
    public GameObject playerReference;

    public float[] cooldowns= {0,0,0};
    private bool isCasting=false;

    [SerializeField] public Image[]AbilityIcons= new Image[abilityCount];
    [SerializeField] public Image[] AbilityBackgrounds=new Image[abilityCount];

    public Transform trans;

    public Sprite emptySlotSprite;
    private void Awake(){
        Instance=this;
        playerReference = GameObject.Find("Player");
    }
    // Start is called before the first frame update
    void Start(){
        //Placeholder ability assignments
        hotbar[0] = ScriptableObject.CreateInstance<BoostAbility>();
        hotbar[0].Initialize(playerReference);
        hotbar[2]=ScriptableObject.CreateInstance<BombAbility>();

        for(int i=0; i<abilityCount; i++){
            states[i]=AbilityState.ready;
        }
           

        for(int i=0; i<abilityCount; i++){
            if(hotbar[i]!=null ) {
                AbilityIcons[i].sprite=hotbar[i].icon;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && hotbar[0]!=null && states[0]==AbilityState.ready)hotbar[0].activate(states, cooldowns, 0, playerReference.transform);
        if(Input.GetKeyDown(KeyCode.Q) && hotbar[1]!=null && states[1]==AbilityState.ready)hotbar[1].activate(states, cooldowns, 1, playerReference.transform);
        if(Input.GetKeyDown(KeyCode.R) && hotbar[2]!=null && states[2]==AbilityState.ready)hotbar[2].activate(states, cooldowns, 2, playerReference.transform);

        for(int i=0; i<abilityCount; i++){
            if(cooldowns[i]>0){
                cooldowns[i]-=Time.deltaTime;
            }
            else if(states[i]==AbilityState.cooldown) states[i]=AbilityState.ready;
        }
            
        
    }

    public enum AbilityState{
        ready,
        active,
        cooldown
    }
}
