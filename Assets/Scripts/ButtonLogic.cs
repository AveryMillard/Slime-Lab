using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic : MonoBehaviour
{

    private Renderer buttonRenderer;
    [SerializeField] private GameObject connection;
    private enum buttonMode { Press, PressAndHold};

    private bool Activated;
    [SerializeField] private buttonMode mode;

    private IButtonReceiver buttonReceiver;

    // Start is called before the first frame update
    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();   
        buttonReceiver = connection.GetComponent<IButtonReceiver>();
        Activated = false;
    }

 

    //Using OnTriggerEnter instead of Update to avoid unneccecary checks, increases performance
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //Works for anything with player tag, can be modified later to fit a more generalized tag
        {
            if(Activated == false)
            {
                buttonRenderer.material.color = Color.green;
                buttonReceiver?.OnButtonPressed();
                Activated = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(mode == buttonMode.PressAndHold)
            {
                buttonRenderer.material.color = Color.red;
                buttonReceiver?.OnButtonReleased();
                Activated = false;
            }
            
        }
    }
}
