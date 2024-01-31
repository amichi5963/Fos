using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] LoadSceneManager loadSceneManager;
    [SerializeField] int Current_StageNumber; //現在居るステージ名(1など)

    PlayerScore pScore;

    [SerializeField] GameObject ClearHUD;

    bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        pScore = GameObject.Find("Player").GetComponent<PlayerScore>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !triggered)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().OnApplicationPause(true);

            triggered = true;

            ClearHUD.SetActive(true);

            pScore.Goal(Current_StageNumber);

            // 動画再生完了時の処理
            loadSceneManager.LoadScene("ResultScene", 1.5f);
        }
    }
}