using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHani : MonoBehaviour
{
    [SerializeField]float killtime;
    Material mat;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        timer = killtime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        mat.SetFloat("_trans", timer / killtime);
        if(timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
