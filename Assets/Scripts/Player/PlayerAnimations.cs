using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isIdling = true;
    private bool isMoving = false;
    private bool isRunning = false;
    private bool isMidAir = false;

    public bool IsIdling
    {
        get { return isIdling; }

        set
        {
            setState(false);
            isIdling = value;
            animator.SetBool("isIdle", value);
        }
    }

    public bool IsMoving
    {
        get { return isMoving; }

        set
        {
            setState(false);
            isMoving = value;
            animator.SetBool("isWalk", value);
        }
    }

    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            setState(false);
            isRunning = value;
            animator.SetBool("isRun", value);
        }
    }

    public bool IsMidAir
    {
        get { return isMidAir; }
        set
        {
            setState(false);
            isMidAir = value;
            animator.SetBool("isMidAir", value);
        }
    }

    void setState(bool value)
    {
        isIdling = value;
        isMoving = value;
        isRunning = value;
        isMidAir = value;

        animator.SetBool("isIdle", value);
        animator.SetBool("isWalk", value);
        animator.SetBool("isRun", value);
        animator.SetBool("isMidAir", value);
    }
}
