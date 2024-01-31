using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLight : MonoBehaviour
{
    //PlayerScore pScore;
    [SerializeField] GameObject effect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")||other.gameObject.CompareTag("Stone"))
        {
            //pScore.PlayerHoldLight++;

            /*
            int tmp = PlayerPrefs.GetInt("PlayerHoldLight",0);
            tmp++;
                        PlayerPrefs.SetInt("PlayerHoldLight", tmp);
            */

            PlayerPrefs.SetInt("InStage_HoldLight", PlayerPrefs.GetInt("InStage_HoldLight", 0) + 1);
            PlayerPrefs.Save();



            Instantiate(effect,transform).transform.parent = null;
            Destroy(gameObject);
        }
    }
}
