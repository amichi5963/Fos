using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    /*
    //memo:�X�e�[�g�p�^�[���Ainterface������ɓ���čl����

    private int _PlayerHoldLight = 0;

    /// <summary>
    /// Player���ێ�����Light
    /// </summary>
    public int PlayerHoldLight
    {
        get => _PlayerHoldLight;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning("�}�C�i�X�ɂ��邱�Ƃ͏o���܂���B0�ɕύX���܂�");
                _PlayerHoldLight = 0;
            }
            else
            {
                _PlayerHoldLight = value;
                Debug.Log("���̐�: " + _PlayerHoldLight);
            }
        }
    }*/

    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        //�ێ����C�g�����Z�b�g
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