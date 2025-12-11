using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePositionSync : MonoBehaviour
{
    public Transform target;      // Reference to the target object
    private Vector3 offset;       // Initial offset between this object and the target

    void Start()
    {
        target = GameObject.Find("Player").GetComponent<Transform>();
        if (target != null)
        {
            // Calculate and store the initial offset
            offset = transform.position - target.position;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Maintain the initial relative position by adding the offset to the target's position
            transform.position = target.position + offset;
        }
    }
}
