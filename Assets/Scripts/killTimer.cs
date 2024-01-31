using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killTimer : MonoBehaviour
{
    [SerializeField] float killTime=10f;
    [SerializeField] GameObject kieObje = null;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(timer > killTime)
        {
            if(kieObje != null)
            {
                Instantiate(kieObje, transform.position, transform.rotation,null);
            }
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
