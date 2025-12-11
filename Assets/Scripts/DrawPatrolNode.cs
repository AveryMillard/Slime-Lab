using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPatrolNode : MonoBehaviour
{
    void OnDrawGizmos()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();
        Gizmos.DrawSphere(sphere.transform.position, sphere.radius);
    }
}
