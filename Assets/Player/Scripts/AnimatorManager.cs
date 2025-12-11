using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator animator;
    private IceMovement playerIceMovement;
    [SerializeField] public float animationSpeed;
    [SerializeField] public float speedMultiplier;

    int horizontal;
    int vertical;
    private void Awake()
    {
        playerIceMovement = GetComponent<IceMovement>();

        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        animator.SetFloat("speedMulitplier", speedMultiplier);


    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement)
    {
        //idle if player is on ice
        if (playerIceMovement != null && playerIceMovement.isOnIce)
        {
            animator.SetFloat(horizontal, 0, 0.1f, Time.deltaTime);
            animator.SetFloat(vertical, 0, 0.1f, Time.deltaTime);
        }

        //normal movement animation
        else
        {
            animator.SetFloat(horizontal, horizontalMovement, 0.1f * animationSpeed, Time.deltaTime);
            animator.SetFloat(vertical, verticalMovement, 0.1f * animationSpeed, Time.deltaTime);
        }
    }
}
