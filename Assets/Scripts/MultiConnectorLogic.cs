using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiConnectorLogic : MonoBehaviour, IButtonReceiver
{
    private IButtonReceiver buttonReceiver1, buttonReceiver2, buttonReceiver3, buttonReceiver4, buttonReceiver5;
    [SerializeField] GameObject connection1;
    [SerializeField] GameObject connection2;
    [SerializeField] GameObject connection3;
    [SerializeField] GameObject connection4;
    [SerializeField] GameObject connection5;


    void Start()
    {
        buttonReceiver1 = connection1.GetComponent<IButtonReceiver>();
        buttonReceiver2 = connection2.GetComponent<IButtonReceiver>();
        buttonReceiver3 = connection3.GetComponent<IButtonReceiver>();
        buttonReceiver4 = connection4.GetComponent<IButtonReceiver>();
        buttonReceiver5 = connection5.GetComponent<IButtonReceiver>();
    }

    public void OnButtonPressed()
    {
        buttonReceiver1?.OnButtonPressed();
        buttonReceiver2?.OnButtonPressed();
        buttonReceiver3?.OnButtonPressed();
        buttonReceiver4?.OnButtonPressed();
        buttonReceiver5?.OnButtonPressed();
    }

    public void OnButtonReleased()
    {
        buttonReceiver1?.OnButtonReleased();
        buttonReceiver2?.OnButtonReleased();
        buttonReceiver3?.OnButtonReleased();
        buttonReceiver4?.OnButtonReleased();
        buttonReceiver5?.OnButtonReleased();
    }

}
