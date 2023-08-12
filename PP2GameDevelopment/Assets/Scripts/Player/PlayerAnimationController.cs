using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        WalkingAnimation();
        RunningAnimation();
    }

    void WalkingAnimation()
    {
        // Forward
        bool isWalkingForward = animator.GetBool("isWalking");
        bool isWalkingForwardLeft = animator.GetBool("isWalkingForwardLeft");
        bool isWalkingForwardRight = animator.GetBool("isWalkingForwardRight");

        // Backward
        bool isWalkingBackward = animator.GetBool("isWalkingBackward");
        bool isWalkingBackwardLeft = animator.GetBool("isWalkingBackwardLeft");
        bool isWalkingBackwardRight = animator.GetBool("isWalkingBackwardRight");

        // Stafe Left && Right
        bool isStrafeLeft = animator.GetBool("isStrafeLeft");
        bool isStrafeRight = animator.GetBool("isStrafeRight");

        // Keys
        bool walkingForward = Input.GetKey(KeyCode.W);
        bool strafeLeft = Input.GetKey(KeyCode.A);
        bool strafeRight = Input.GetKey(KeyCode.D);
        bool walkingBackward = Input.GetKey(KeyCode.S);

        /////////////////////////////////////////////////////////
        // Forward Walk
        if (!isWalkingForward && walkingForward)
        {
            animator.SetBool("isWalking", true);
        }

        if (isWalkingForward && !walkingForward)
        {
            animator.SetBool("isWalking", false);
        }

        // Forward Left Walk
        if (!isWalkingForwardLeft && walkingForward && strafeLeft)
        {                     
            animator.SetBool("isWalkingForwardLeft", true);        
        }

        if (isWalkingForwardLeft && !walkingForward || !strafeLeft)
        {
            animator.SetBool("isWalkingForwardLeft", false);
        }

        // Forward Right Walk
        if (!isWalkingForwardRight && walkingForward && strafeRight)
        {
            animator.SetBool("isWalkingForwardRight", true);
        }

        if (isWalkingForwardRight && !walkingForward || !strafeRight)
        {
            animator.SetBool("isWalkingForwardRight", false);
        }

        /////////////////////////////////////////////////////////
        // Backward Walk
        if (!isWalkingBackward && walkingBackward)
        {
            animator.SetBool("isWalkingBackward", true);
        }

        if (isWalkingBackward && !walkingBackward)
        {
            animator.SetBool("isWalkingBackward", false);
        }

        // Backward Walk Left
        if (!isWalkingBackwardLeft && walkingBackward && strafeLeft)
        {
            animator.SetBool("isWalkingBackwardLeft", true);
        }

        if (isWalkingBackwardLeft && !walkingBackward || !strafeLeft)
        {
            animator.SetBool("isWalkingBackwardLeft", false);
        }

        // Backward Walk Right
        if (!isWalkingBackwardRight && walkingBackward && strafeRight)
        {
            animator.SetBool("isWalkingBackwardRight", true);
        }

        if (isWalkingBackwardRight && !walkingBackward || !strafeRight)
        {
            animator.SetBool("isWalkingBackwardRight", false);
        }

        /////////////////////////////////////////////////////////
        // Strafe Left
        if (!isStrafeLeft && strafeLeft)
        {
            animator.SetBool("isStrafeLeft", true);
        }

        if (isStrafeLeft && walkingBackward || !strafeLeft || walkingForward)
        {
            animator.SetBool("isStrafeLeft", false);
        }

        // Strafe Right
        if (!isStrafeRight && strafeRight)
        {
            animator.SetBool("isStrafeRight", true);
        }

        if (isStrafeRight && walkingBackward || !strafeRight || walkingForward)
        {
            animator.SetBool("isStrafeRight", false);
        }
    }

    void RunningAnimation()
    {
        bool isRunning = animator.GetBool("isRunning");
        bool runningPressed = Input.GetKey(KeyCode.LeftShift);

        if (!isRunning && runningPressed)
        {
            animator.SetBool("isRunning", true);
        }

        if(isRunning && !runningPressed)
        {
            animator.SetBool("isRunning", false);
        }
    }
}
