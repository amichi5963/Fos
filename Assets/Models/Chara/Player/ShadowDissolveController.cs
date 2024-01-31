using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDissolveController : MonoBehaviour
{
    [SerializeField] public Material mat;
    [SerializeField] public float speed = 0.2f;
    public float amount = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        mat.SetFloat("_State", 0);
        amount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        amount += Time.deltaTime * speed;
        mat.SetFloat("_State", amount);
    }
}
