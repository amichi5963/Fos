using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Cinemachine;

public class TitleSystem : MonoBehaviour
{
    private GameInputs _gameInputs;
    protected Vector2 _menuInputValue;

    [SerializeField] GameObject Canvas_TitleCredit;
    [SerializeField] GameObject Canvas_TitleMenu;


    [SerializeField] CinemachineDollyCart cinemachineDollyCart;

    PlayableDirector Director_Canvas_TitleCredit;
    PlayableDirector Director_Canvas_TitleMenu;
    [SerializeField] PlayableDirector Director_StartButton;

    [SerializeField] bool displayMenu = false;

    [SerializeField] LoadSceneManager loadSceneManager;

    [SerializeField] GameObject Select_Play;
    //[SerializeField] GameObject Select_Option;
    [SerializeField] GameObject Select_Exit;

    [SerializeField] GameObject Select_Story;
    [SerializeField] GameObject Select_Level_1;
    [SerializeField] GameObject Select_Level_2;
    [SerializeField] GameObject Select_Level_3;
    [SerializeField] GameObject Select_Level_4;
    [SerializeField] GameObject Select_Level_5;
    [SerializeField] GameObject Select_Level_6;

    [SerializeField] GameObject StageText;
    [SerializeField] GameObject MenuText;
    [SerializeField] GameObject TitleImage;

    bool menuLock = false;

    bool movieComplete = false;

    public enum GRAND_MENU
    {
        START_MENU,
        STAGE_SELECT,
    }


    public enum MENU_TYPE
    {
        PLAY,
        //OPTION,
        EXIT,
    }

    public enum STAGE_SELECT
    {
        STORY,
        STAGE_1,
        STAGE_2,
        STAGE_3,
        STAGE_4,
        STAGE_5,
        STAGE_6,
    }

    public GRAND_MENU current_GrandMenu = GRAND_MENU.START_MENU;
    public MENU_TYPE current_MenuType = MENU_TYPE.PLAY;

    public STAGE_SELECT current_SelectStage = STAGE_SELECT.STORY;

    private void Awake()
    {
        _gameInputs = new GameInputs();

        // Actionイベント登録
        _gameInputs.Player.Menu.started += OnMenu;
        _gameInputs.Player.Menu.performed += OnMenu;
        _gameInputs.Player.Menu.canceled += OnMenu;

        _gameInputs.Player.Move.started += OnMenu;
        _gameInputs.Player.Move.performed += OnMenu;
        _gameInputs.Player.Move.canceled += OnMenu;

        _gameInputs.Player.Enter.started += OnEnter;
        _gameInputs.Player.Enter.performed += OnEnter;
        _gameInputs.Player.Enter.canceled += OnEnter;

        _gameInputs.Player.Cancel.started += OnCancel;
        _gameInputs.Player.Cancel.performed += OnCancel;
        _gameInputs.Player.Cancel.canceled += OnCancel;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        _gameInputs.Enable();
    }

    private void OnMenu(InputAction.CallbackContext context)
    {
        // menuの入力取得
        _menuInputValue = -context.ReadValue<Vector2>();
        //Debug.Log(_menuInputValue);
    }

    private void OnEnter(InputAction.CallbackContext context)
    {
        if (!displayMenu) return;

        //一度だけ実行
        if (context.performed)
        {
            //タイトルムービースキップ
            TitleSkip();

            if (current_GrandMenu == GRAND_MENU.STAGE_SELECT)
            {
                switch (current_SelectStage)
                {
                    //ストーリー復旧時差し替え
                    case STAGE_SELECT.STORY: loadSceneManager.LoadScene("Story"); break;
                    case STAGE_SELECT.STAGE_1: loadSceneManager.LoadScene("Level_01"); break;
                    case STAGE_SELECT.STAGE_2: loadSceneManager.LoadScene("Level_02"); break;
                    case STAGE_SELECT.STAGE_3: loadSceneManager.LoadScene("Level_03"); break;
                    case STAGE_SELECT.STAGE_4: loadSceneManager.LoadScene("Level_04"); break;
                    case STAGE_SELECT.STAGE_5: Debug.Log("未実装"); break;
                    case STAGE_SELECT.STAGE_6: Debug.Log("未実装"); break;
                }
            }

            switch (current_MenuType)
            {
                case MENU_TYPE.PLAY:
                    //Director_StartButton.Play();
                    current_GrandMenu = GRAND_MENU.STAGE_SELECT;
                    StageText.SetActive(true);
                    MenuText.SetActive(false);
                    TitleImage.SetActive(false);
                    break;
                case MENU_TYPE.EXIT:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
                    break;
            }

        }

    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (current_GrandMenu)
            {
                case GRAND_MENU.STAGE_SELECT:
                    //Director_StartButton.Play();
                    current_GrandMenu = GRAND_MENU.START_MENU;
                    StageText.SetActive(false);
                    MenuText.SetActive(true);
                    TitleImage.SetActive(true);
                    break;
            }
        }    
    }

    void HiddenAllCursor()
    {
        Select_Play.SetActive(false);
        //Select_Option.SetActive(false);
        Select_Exit.SetActive(false);


        Select_Story.SetActive(false);
        Select_Level_1.SetActive(false);
        Select_Level_2.SetActive(false);
        Select_Level_3.SetActive(false);
        Select_Level_4.SetActive(false);
        Select_Level_5.SetActive(false);
        Select_Level_6.SetActive(false);
    }

    void DisplayCursor_Menu()
    {
        switch (current_MenuType)
        {
            case MENU_TYPE.PLAY: Select_Play.SetActive(true); break;
            //case MENU_TYPE.OPTION: Select_Option.SetActive(true); break;
            case MENU_TYPE.EXIT: Select_Exit.SetActive(true);break;
        }
    }

    void DisplayCursor_SelectStage()
    {
        switch (current_SelectStage)
        {

            case STAGE_SELECT.STORY: Select_Story.SetActive(true); break;
            case STAGE_SELECT.STAGE_1: Select_Level_1.SetActive(true); break;
            case STAGE_SELECT.STAGE_2: Select_Level_2.SetActive(true); break;
            case STAGE_SELECT.STAGE_3: Select_Level_3.SetActive(true); break;
            case STAGE_SELECT.STAGE_4: Select_Level_4.SetActive(true); break;
            case STAGE_SELECT.STAGE_5: Select_Level_5.SetActive(true); break;
            case STAGE_SELECT.STAGE_6: Select_Level_6.SetActive(true); break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Director_Canvas_TitleCredit = Canvas_TitleCredit.GetComponent<PlayableDirector>();
        //Canvas_TitleCredit.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Invoke("hoge", 0.5f);
    }

    private void FixedUpdate()
    {
        // 大きさが1より大きかったら1に正規化する（主にキーボードのため）
        if (_menuInputValue.magnitude > 1) _menuInputValue.Normalize();

        if (_menuInputValue.y <= 0.1 && _menuInputValue.y >= -0.1) menuLock = false;


        //動きすぎ防止、カーソルが一つずつ進むように。
        if (!menuLock)
        {
            if (current_GrandMenu == GRAND_MENU.START_MENU)
            {
                if (_menuInputValue.y >= 0.8f && current_MenuType != MENU_TYPE.EXIT)
                {
                    current_MenuType++;
                    menuLock = true;

                    //Debug.Log(current_MenuType);
                }
                else
            if (_menuInputValue.y <= -0.8f && current_MenuType != MENU_TYPE.PLAY)
                {
                    current_MenuType--;
                    menuLock = true;

                    //Debug.Log(current_MenuType);
                }
            }


            if (current_GrandMenu == GRAND_MENU.STAGE_SELECT)
            {
                if (_menuInputValue.y >= 0.8f && current_SelectStage != STAGE_SELECT.STAGE_6)
                {
                    current_SelectStage++;
                    menuLock = true;

                    //Debug.Log(current_SelectStage);
                }
                else

                if (_menuInputValue.y <= -0.8f && current_SelectStage != STAGE_SELECT.STORY)
                {
                    current_SelectStage--;
                    menuLock = true;

                    //Debug.Log(current_SelectStage);
                }
            }  
        }

        //全てのcursorを非表示
        HiddenAllCursor();

        //cursor表示
        switch (current_GrandMenu)
        {
            case GRAND_MENU.START_MENU: DisplayCursor_Menu(); break;
            case GRAND_MENU.STAGE_SELECT: DisplayCursor_SelectStage(); break;
        }  
    }

    void hoge()
    {
        Canvas_TitleCredit.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDone() == true)
        {
            movieComplete = true;
            Canvas_TitleMenu.SetActive(true);
        }
    }

    void TitleSkip()
    {
        //TitleCreditが終了していない時のみスキップできる
        if (IsDone() == false && movieComplete == false)
        {
            //Director_Canvas_TitleCredit.enabled = false;
            //Canvas_TitleCredit.SetActive(false);
            //cinemachineDollyCart.m_Position = 3;

            movieComplete = true;

            //終了時の時間に設定
            Director_Canvas_TitleCredit.time = Director_Canvas_TitleCredit.duration;
        }
    }


    public bool IsDone()
    {
        return Director_Canvas_TitleCredit.time >= Director_Canvas_TitleCredit.duration;
    }
}