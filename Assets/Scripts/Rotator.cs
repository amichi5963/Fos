using UnityEngine;
using System.Collections;


public class RotateCube : MonoBehaviour
{
    [SerializeField] float speed = 5.0f;
    void Update()
    {
        transform.Rotate(new Vector3(0, speed, 0));
    }
}