using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMovement : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private Vector3 lockedVelocity;
    [SerializeField] public bool isOnIce = false;
    [SerializeField] private bool isTouchingWall = false;
 

    public float minimumSpeed = 5f;  


    private PlayerLocomotion playerLocomotion;
    private PlayerManage playerManage;


    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerManage = GetComponent<PlayerManage>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with an object tagged as "Ice"
        if (collision.gameObject.CompareTag("IceFloor"))
        {
            Debug.Log("On ice");
            LockMomentum();
        }
        else if (isOnIce && collision.gameObject.CompareTag("WallObject"))
        {
            // Player is touching the wall while on ice, give them control
            EnableWallControl();
        }
        else
        {
            // If the player is on ice and collides with a non-ice, non-wall object, restore control
            RestoreControl();
        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (isOnIce && collision.gameObject.CompareTag("WallObject"))
        {
            // The player has stopped touching the wall, lock the new momentum and continue sliding
            LockMomentum();
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.3f);
            bool nearIce = false;

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("IceFloor"))

                {
                    nearIce = true;
                    break;
                }
            }
            if (!nearIce) RestoreControl();
        }
    }
    
    void EnableWallControl()
    {
        // Allow movement while touching a wall
        isTouchingWall = true;

        if (playerLocomotion != null)
        {
            playerManage.isOnIce = false;
        }
    }

    void LockMomentum()
    {
        Debug.Log("momentum locked");
        Vector3 currentVelocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
        float currentSpeed = currentVelocity.magnitude;

        // If speed is too slow or zero, set a minimum speed
        if (currentSpeed < minimumSpeed)
        {
            // Normalize the velocity and scale to the minimum speed
            currentVelocity = currentVelocity.normalized * minimumSpeed;
        }

        lockedVelocity = currentVelocity;
        isOnIce = true;
        isTouchingWall = false;


        // Disable player movement script so they can't change direction
        if (playerLocomotion != null)
        {
            playerManage.isOnIce = isOnIce;
        }
    }

    void RestoreControl()
    {
        // Restore normal movement control when player touches non-ice object
        isOnIce = false;
        isTouchingWall = false;

        if (playerLocomotion != null)
        {
            playerManage.isOnIce = isOnIce;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isOnIce && !isTouchingWall)
        {
            // Maintain the locked horizontal velocity while allowing vertical velocity (gravity)
            playerRigidbody.velocity = new Vector3(lockedVelocity.x, playerRigidbody.velocity.y, lockedVelocity.z);
        }
        // If the player is touching a wall, they can move freely (handled by PlayerMovement script)
    }
}
