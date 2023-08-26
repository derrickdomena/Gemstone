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
        // Movement
        WalkingAnimation();
        RunningAnimation();
        JumpingAnimation();
        // Abilities
        //AbilityAnimation();
        // Weapons
        MeleeAnimation();
        //ReloadingAnimation();
        //ShootingAnimation();
        //DeathAnimation();
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

        if (isRunning && !runningPressed)
        {
            animator.SetBool("isRunning", false);
        }
    }

    void JumpingAnimation()
    {
        bool isJumping = animator.GetBool("isJumping");
        bool jumpingKey = Input.GetKeyDown(KeyCode.Space);

        if (!isJumping && jumpingKey || gameManager.instance.playerScript.controller.isGrounded && Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("isJumping", true);
        }

        if (isJumping && !jumpingKey)
        {
            animator.SetBool("isJumping", false);
        }
    }

    //void AbilityAnimation()
    //{
    //    bool isFireball = animator.GetBool("isAbility");
    //    bool fireballAbility = Input.GetKeyDown(KeyCode.Q);
    //    bool stasisAbility = Input.GetKeyDown(KeyCode.C);

    //    if (!isFireball && (fireballAbility || stasisAbility) && fireball.canThrow == true)
    //    {
    //        animator.SetBool("isAbility", true);
    //    }

    //    if (isFireball && (!fireballAbility || !stasisAbility) && fireball.canThrow == false)
    //    {
    //        animator.SetBool("isAbility", false);
    //    }
    //}

    void MeleeAnimation()
    {
        bool isAttacking = animator.GetBool("isAttacking");
        bool attackKey = Input.GetMouseButtonDown(0);

        // Changing the gamemanager section to be when melee is equipped.
        if (!isAttacking && attackKey && gameManager.instance.playerScript.weaponType == "Melee")
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine(waitForAttackSpeed());
        }
    }
    IEnumerator waitForAttackSpeed()
    {
        gameManager.instance.playerScript.meleeModel.SetActive(false);
        yield return new WaitForSeconds(gameManager.instance.playerScript.attackSpeed);
        animator.SetBool("isAttacking", false);
        if (gameManager.instance.playerScript.weaponList[gameManager.instance.playerScript.selectedWeapon].ammoReserve > 0)
        {
            gameManager.instance.playerScript.gunModel.SetActive(true);
            gameManager.instance.playerScript.meleeModel.SetActive(false);
        }
        else
        {
            gameManager.instance.playerScript.gunModel.SetActive(false);
            gameManager.instance.playerScript.meleeModel.SetActive(true);
        }
    }
    //void ReloadingAnimation()
    //{
    //    bool isReloading = animator.GetBool("isReloading");
    //    bool reloadKey = Input.GetKey(KeyCode.R);

    //    if (!isReloading && reloadKey)
    //    {
    //        animator.SetBool("isReloading", true);
    //    }
    //    if (isReloading && !reloadKey)
    //    {
    //        animator.SetBool("isReloading", false);
    //    }
    //}

    //void ShootingAnimation()
    //{
    //    bool isShooting = animator.GetBool("isShooting");
    //    bool shootingKey = Input.GetMouseButton(0);

    //    if (!isShooting && shootingKey)
    //    {
    //        animator.SetBool("isShooting", true);
    //    }
    //    if (isShooting && !shootingKey)
    //    {
    //        animator.SetBool("isShooting", false);
    //    }
    //}

    //void DeathAnimation()
    //{
    //    bool isDead = animator.GetBool("isDead");

    //    if (!isDead && gameManager.instance.playerScript.hp <= 0)
    //    {
    //        animator.SetBool("isDead", true);
    //    }
    //}
}
