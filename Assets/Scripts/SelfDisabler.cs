using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisabler : MonoBehaviour
{
    [SerializeField] float killTime = 1f;
    float timer = 0;
    [SerializeField]bool PlayerPause = false;
    [SerializeField] float pauseTime = 1f;
    GameObject _playerObject;
    PlayerController PlayerController;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10000000);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                _playerObject = collider.gameObject;
                PlayerController = _playerObject.GetComponent<PlayerController>();
                break;
            }
        }
        if (PlayerPause)
            PlayerController.OnApplicationPause(true);
    }

    // Update is called once per frame

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > killTime)
        {
            timer = 0;
            if (PlayerPause)
                PlayerController.pause(false,pauseTime);
            gameObject.SetActive(false);
        }
    }
}