using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalUnit : MonoBehaviour
{
    [SerializeField] float searchLength;
    bool isRinging = false;
    bool preRingFrame = false;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    [SerializeField] Animation anim;
    bool ringframe = false;


    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchLength);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Stone") ||
                (collider.CompareTag("Player") && collider.GetComponent<PlayerController>().getRinging()))
            {
                Ring();
            }
        }
        if (!ringframe)
        {
            preRingFrame = false;
        }
        isRinging = ringframe;
        ringframe = false;
    }
    public void Ring()
    {
        if (!ringframe)
        {
            ringframe = true;
            if (preRingFrame == false)
            {
                if (audioSource != null && audioClip != null)
                    audioSource.PlayOneShot(audioClip);
                if (anim != null)
                    anim.Play("CubeAction");
            }
            preRingFrame = true;
        }
    }

    public bool IsRinging()
    {
        return isRinging;
    }
}