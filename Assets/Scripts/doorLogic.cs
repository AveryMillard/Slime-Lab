using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class doorLogic : MonoBehaviour, IButtonReceiver
{

    private Renderer doorRenderer;
    private Color defaultColor;
    private GameObject leftDoor;
    private GameObject rightDoor;
    public enum direction { North = 1, East, South, West}
    [SerializeField] public direction Direction; //Assign from CAMERA view of the rooms

    public enum mode { Unlocked, Locked, Inverted}
    [SerializeField] public mode doorMode;

    public enum EntrancePref { NoPreference, Entrance};
    [SerializeField] public EntrancePref EntrancePreference;

    Vector3 ldOpenPos;
    Vector3 ldClosePos;
    Vector3 rdOpenPos;
    Vector3 rdClosePos;
    float openSpeed = 2f;
    [SerializeField] bool isOpening = false;
    [SerializeField] bool isClosing = false;

    // Start is called before the first frame update

    //for new door model, switch leftDoor & rightDoor, change transfomr.right to .up
    void Start()
    {
        doorRenderer = GetComponent<Renderer>();
        defaultColor = doorRenderer.material.color;
        rightDoor = transform.Find("Door_Left_001").gameObject;
        leftDoor = transform.Find("Door_Right_001").gameObject;

        if(doorMode == mode.Locked)
        {
            isClosing = true;
            ldClosePos = leftDoor.transform.position;
            rdClosePos = rightDoor.transform.position;
            ldOpenPos = ldClosePos - (leftDoor.transform.up * 2.5f);
            rdOpenPos = rdClosePos + (rightDoor.transform.up * 2.5f);
        }
        else
        {
            isOpening = true;
            ldClosePos = leftDoor.transform.position;
            rdClosePos = rightDoor.transform.position;
            ldOpenPos = ldClosePos - (leftDoor.transform.up * 2.5f);
            rdOpenPos = rdClosePos + (rightDoor.transform.up * 2.5f);
        }
       
    }

    // Update is called once per frame
    void Update()
    {

        if (isOpening)
        {
            rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, rdOpenPos, Time.deltaTime * openSpeed);
            leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, ldOpenPos, Time.deltaTime * openSpeed);
        }
        else if (isClosing)
        {

            rightDoor.transform.position = Vector3.Lerp(rightDoor.transform.position, rdClosePos, Time.deltaTime * openSpeed);
            leftDoor.transform.position = Vector3.Lerp(leftDoor.transform.position, ldClosePos, Time.deltaTime * openSpeed);
        }
    }


    public void doorToggle()
    {
        isOpening = !isOpening;
        isClosing = !isClosing;
    }


    //Implementing requirements for IButtonReciever
    public void OnButtonPressed()
    {
        doorRenderer.material.color = Color.green;
        doorToggle();
    }

    public void OnButtonReleased()
    {
        doorRenderer.material.color = defaultColor;
        doorToggle();
    }
}
