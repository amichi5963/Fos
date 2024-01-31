using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    /*
    //memo:ステートパターン、interfaceを視野に入れて考える

    private int _PlayerHoldLight = 0;

    /// <summary>
    /// Playerが保持するLight
    /// </summary>
    public int PlayerHoldLight
    {
        get => _PlayerHoldLight;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("マイナスにすることは出来ません。0に変更します");
                _PlayerHoldLight = 0;
            }
            else
            {
                _PlayerHoldLight = value;
                Debug.Log("光の数: " + _PlayerHoldLight);
            }
        }
    }*/

    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        //保持ライトをリセット
        PlayerPrefs.SetInt("InStage_HoldLight", 0);
        PlayerPrefs.SetFloat("ClearTime", 0);
    }

    public void Goal(int StageNumber)
    {
        PlayerPrefs.SetInt("Lastplayed_Stage", StageNumber);
        PlayerPrefs.SetFloat("ClearTime_Stage_" + StageNumber, time);
        PlayerPrefs.SetInt("ClearLight_Stage_" + StageNumber, PlayerPrefs.GetInt("InStage_HoldLight", 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
    }
    public int GetScore()
    {
        return PlayerPrefs.GetInt("InStage_HoldLight", 0);
    }
}