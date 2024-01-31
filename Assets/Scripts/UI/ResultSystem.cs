using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ResultSystem : MonoBehaviour
{
    [SerializeField] LoadSceneManager loadSceneManager;

    private GameInputs _gameInputs;

    [SerializeField] TextMeshProUGUI TMPtext_light;
    [SerializeField] TextMeshProUGUI TMPtext_clearTime;

    [SerializeField] int MAX_Light_Stage_1 = 20;
    [SerializeField] int MAX_Light_Stage_2 = 28;
    [SerializeField] int MAX_Light_Stage_3 = 37;
    [SerializeField] int MAX_Light_Stage_4 = 40;
    [SerializeField] int MAX_Light_Stage_5;
    [SerializeField] int MAX_Light_Stage_6;

    int tmp_MAXLight = 0;
    private void Awake()
    {
        _gameInputs = new GameInputs();

        // Actionイベント登録
        _gameInputs.Player.Cancel.started += OnCancel;
        _gameInputs.Player.Cancel.performed += OnCancel;
        _gameInputs.Player.Cancel.canceled += OnCancel;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        _gameInputs.Enable();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 動画再生完了時の処理
            switch(PlayerPrefs.GetInt("Lastplayed_Stage"))
            {
                case 1:
                    loadSceneManager.LoadScene("Level_02");
                    break;

                case 2:
                    loadSceneManager.LoadScene("Level_03");
                    break;

                case 3:
                    loadSceneManager.LoadScene("Level_04");
                    break;
                case 4:
                    loadSceneManager.LoadScene("Level_05");
                    break;
                default:
                    loadSceneManager.LoadScene("TitleScene");
                    break;

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        double time = PlayerPrefs.GetFloat("ClearTime_Stage_" + PlayerPrefs.GetInt("Lastplayed_Stage"));
        int h = (int)(time / 3600);
        int m = (int)((time - (h * 3600)) / 60);
        int s = (int)(time % 60);
        TMPtext_clearTime.text = /* + h + ":" */ m + ":" + s;



        switch (PlayerPrefs.GetInt("Lastplayed_Stage"))
        {
            case 1:
                tmp_MAXLight = MAX_Light_Stage_1;
                break;

            case 2:
                tmp_MAXLight = MAX_Light_Stage_2;
                break;

            case 3:
                tmp_MAXLight = MAX_Light_Stage_3;
                break;

            case 4:
                tmp_MAXLight = MAX_Light_Stage_4;
                break;

            case 5:
                tmp_MAXLight = MAX_Light_Stage_5;
                break;

            case 6:
                tmp_MAXLight = MAX_Light_Stage_6;
                break;

            default:
                break;
        }

        TMPtext_light.text = PlayerPrefs.GetInt("ClearLight_Stage_" + PlayerPrefs.GetInt("Lastplayed_Stage")) + "/" + tmp_MAXLight;

        Debug.Log("最後にクリアしたステージ:" + PlayerPrefs.GetFloat("Lastplayed_Stage"));
        Debug.Log("光:" + PlayerPrefs.GetInt("ClearLight_Stage_1"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
