using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    [SerializeField] GameObject TeleportSpot;

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.position=TeleportSpot.transform.position;
    }
}