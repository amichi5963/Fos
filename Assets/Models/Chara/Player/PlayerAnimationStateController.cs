using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationStateController : MonoBehaviour
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
        
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            animator.SetBool("isMoving", true); 
        }
        if(context.phase==InputActionPhase.Canceled)
        {
            animator.SetBool("isMoving", false);
        }
    }
    public void SkillStart(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Debug.Log("‚Í‚Ë");
            animator.SetBool("isJumping", true);
            Invoke(nameof(StopJump), 0.1f);
        }

    }
    public void StopJump()
    {
        animator.SetBool("isJumping", false);
    }
}