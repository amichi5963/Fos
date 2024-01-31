using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] List<GameObject> insides = new List<GameObject>();
    [SerializeField] Animation anim;
    Collider col;
     Rigidbody rb;
    killTimer killTimer;

    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.GetComponent<Collider>();
        rb = gameObject.GetComponent<Rigidbody>();
        killTimer = gameObject.GetComponent<killTimer>();
        foreach (AnimationState state in anim)
        {
            state.speed = 0.0F;
        }
    }

    public void killMe()
    {
        foreach (AnimationState state in anim)
        {
            state.speed = 1.0F;

        }
        if (insides != null)
        {
            foreach (GameObject go in insides)
            {
                Instantiate(go, transform.position, Quaternion.identity,null);
                col.isTrigger = true;
                rb.isKinematic = false;
            }
        }
        col.enabled = false;
        if (killTimer != null)
            killTimer.enabled = true;
    }
}