using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple scalable interface to deal with connections in button logic. 
public interface IButtonReceiver
{
    void OnButtonPressed();
    void OnButtonReleased();
}
