using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMoverScript : MonoBehaviour
{
    [SerializeField] float CollectStartRadius = 2f;
    [SerializeField] float DelayTimer = 0.1f;
    [SerializeField] float speed = 1f;
    [SerializeField] bool isCollect = false;
    float timer = 0f;
    GameObject PlayerObject = null;
    Rigidbody Rigidbody = null;
    // Start is called before the first frame update
    void Start()
    {
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CollectStartRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == PlayerObject)
            {
                isCollect = true;
            }
        }

        //非回収時　動かない
        if (!isCollect)
        {
            return;
        }
        timer += Time.deltaTime;

        //収集開始前は立ち止まる
        if (timer < DelayTimer)
        {
            return;
        }
        //プレイヤーに向かって進ませる
        Rigidbody.AddForce((PlayerObject.transform.position - transform.position) * speed);
    }
}
