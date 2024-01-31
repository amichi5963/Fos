using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class RebindingInput : MonoBehaviour
{
    //PlayerInputコンポーネントがあるゲームオブジェクト
    [SerializeField]
    private PlayerInput _pInputPlayer;
    [SerializeField]
    private PlayerInput _pInputUI;

    List<InputAction> actionList;

    //リバインディングしたいInputAction項目。
    InputAction attackAction;
    InputAction jumpAction;
    InputAction skillAction;
    InputAction changeLAction;
    InputAction changeRAction;
    InputAction moveAction;

    InputAction submitAction;
    InputAction cancelAction;
    InputAction pauseAction;

    //リバインディングテキスト。アクションと同じ順番。Defaultは初期設定
    [SerializeField] List<TextMeshProUGUI> keyboardTexts;
    [SerializeField] List<string> keyboardDefaultTexts;

    [SerializeField] List<TextMeshProUGUI> gamepadTexts;
    [SerializeField] List<string> gamepadDefaultTexts;
    int textIndex = 0;



    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    //途中でゲームパッドを挿したとき用
    bool gamepadCompositeComplete = false;

    public void Start()
    {
        attackAction = _pInputPlayer.actions["Attack"];
        jumpAction = _pInputPlayer.actions["Jump"];
        skillAction = _pInputPlayer.actions["Skill"];
        changeLAction = _pInputPlayer.actions["SkillChangeL"];
        changeRAction = _pInputPlayer.actions["SkillChangeR"];
        moveAction = _pInputPlayer.actions["Move"];

        submitAction = _pInputUI.actions["Submit"];
        cancelAction = _pInputUI.actions["Cancel"];
        pauseAction = _pInputUI.actions["Pause"];

        actionList = new List<InputAction>();

        actionList.Add(attackAction);
        actionList.Add(jumpAction);
        actionList.Add(skillAction);
        actionList.Add(changeLAction);
        actionList.Add(changeRAction);
        actionList.Add(moveAction);
        actionList.Add(moveAction);
        actionList.Add(moveAction);
        actionList.Add(moveAction);
        actionList.Add(submitAction);
        actionList.Add(cancelAction);
        actionList.Add(pauseAction);
        Debug.Log(actionList.Count);

        if (Gamepad.current == null)
        {
            gamepadCompositeComplete = false;
        }

        //DefaultTextの保存
        keyboardDefaultTexts = new List<string>();
        gamepadDefaultTexts = new List<string>();
        foreach (var text in keyboardTexts)
        {
            keyboardDefaultTexts.Add(text.text);
        }

        foreach (var text in gamepadTexts)
        {
            gamepadDefaultTexts.Add(text.text);
        }


        //すでにリバインディングしたことがある場合はシーン読み込み時に変更。
        LoadBindData();

    }


    private void Update()
    {
        //やってはみたけど途中からの接続じゃキーバインドできなかった
        if (Gamepad.current != null && !gamepadCompositeComplete && (_pInputPlayer.currentActionMap.name == "Player") && (_pInputUI.currentActionMap.name == "UI"))
        {
            Debug.Log("ゲームパッドLoading");
            LoadBindData();
            gamepadCompositeComplete = true;
        }
        else if (Gamepad.current == null && gamepadCompositeComplete)
        {
            Debug.Log("ゲームパッドdisconect");
            GamePadUnComposite();
            gamepadCompositeComplete = false;
        }


    }



    public void StartRebinding(int index)
    {
        //Submitアクションの入力によってキーボードかゲームパッドか判別
        var s = _pInputUI.actions["Submit"].activeControl.path;

        //ボタンの誤作動を防ぐため、何も無いアクションマップに切り替え
        _pInputPlayer.SwitchCurrentActionMap("Blank");
        _pInputUI.SwitchCurrentActionMap("Blank");

        if (s.Contains("Keyboard"))
        {
            Debug.Log("keyboard");
            keyboardTexts[index].color = new Color32(176, 16, 48, 255);
            //Fireボタンのリバインディング開始
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(actionList[index].GetBindingIndexForControl(actionList[index].controls[0]))  //contolsのindexは変数を渡すとエラーになりやすかったので直
                .WithControlsExcluding("Mouse") //.OnMatchWaitForAnother(0.1f)があると決定キー入力を受け付けない時がある
                .OnComplete(operation => RebindComplete(index, 0))
                .Start();
        }
        else
        {
            Debug.Log("GamePad");
            gamepadTexts[index].color = new Color32(176, 16, 48, 255);
            //Fireボタンのリバインディング開始
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(actionList[index].GetBindingIndexForControl(actionList[index].controls[1]))
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => RebindComplete(index, 1))
                .Start();
        }

    }

    public void RebindComplete(int index, int control)
    {
        //アクションのコントロール(バインディングしたコントロール)のインデックスを取得


        if (control == 0)
        {
            int bindingIndex = actionList[index].GetBindingIndexForControl(actionList[index].controls[control]);
            //Keyboardの時
            //バインディングしたキーの名称を取得する
            keyboardTexts[index].text = InputControlPath.ToHumanReadableString(
                actionList[index].bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            keyboardTexts[index].color = new Color32(255, 255, 255, 255);
        }
        else if (control == 1)
        {
            //GamePad
            int bindingIndex = actionList[index].GetBindingIndexForControl(actionList[index].controls[control]);
            gamepadTexts[index].text = InputControlPath.ToHumanReadableString(
                actionList[index].bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            gamepadTexts[index].color = new Color32(255, 255, 255, 255);
        }
        else
        {
            //GamePadのMove用
            //GetBindingIndexForControl(actionList[index].controls[control])はエラーの温床マジで
            //結局アクションコントロールのindexもindexと同じなのでそのまま利用
            gamepadTexts[index].text = InputControlPath.ToHumanReadableString(
                actionList[index].bindings[index].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            gamepadTexts[index].color = new Color32(255, 255, 255, 255);
        }

        _rebindingOperation.Dispose();


        //リバインディング時は空のアクションマップだったので通常のアクションマップに切り替え
        _pInputPlayer.SwitchCurrentActionMap("Player");
        _pInputUI.SwitchCurrentActionMap("UI");
        //リバインディングしたキーを保存(シーン開始時に読み込むため)
        PlayerPrefs.SetString("rebindSample", _pInputPlayer.actions.SaveBindingOverridesAsJson());
        PlayerPrefs.SetString("rebindUI", _pInputUI.actions.SaveBindingOverridesAsJson());
    }

    //Move用Start。キーボードは2DVectorの各方向。ゲームパッドはスティックのD-Pad選択
    public void StartRebinding(int index, string vector, string compositePart)
    {
        var s = _pInputUI.actions["Submit"].activeControl.path;

        _pInputPlayer.SwitchCurrentActionMap("Blank");
        _pInputUI.SwitchCurrentActionMap("Blank");

        if (s.Contains("Keyboard"))
        {
            var action = actionList[index];
            //vectorは2DVector、compositePartは各方向名
            int targetIndex = Get2DVectorCompositeBindingIndex(action, vector, compositePart);  //これもtargetIndexを取得しているが2DVectorは[0]が2DVectorで上下左右が[1]～[4]なので直でもいいかも

            Debug.Log("Movekeyboard");
            Debug.Log(targetIndex);
            keyboardTexts[index].color = new Color32(176, 16, 48, 255);
            //キーボードの各方向に対してリバインディング開始
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(targetIndex)
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => RebindComplete(index, vector, compositePart))  //キーボード用のRebindComplete
                .Start();
        }
        else
        {
            if (index == 7 || index == 8)
            {
                _pInputPlayer.SwitchCurrentActionMap("Player");
                _pInputUI.SwitchCurrentActionMap("UI");
                return;
            }
            Debug.Log("GamePad");
            Debug.Log(actionList[index]);
            gamepadTexts[index].color = new Color32(176, 16, 48, 255);
            //Moveアクションのリバインディング開始
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(index)   //actionList[index].GetBindingIndexForControl(actionList[index].controls[index]))では拾えない。やはりエラーの温床
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => RebindComplete(index, index))
                .Start();
        }

    }

    //MoveKeyBoard用
    public void RebindComplete(int index, string vector, string compositePart)
    {
        //アクションのコントロール(バインディングしたコントロール)のインデックスを取得
        var action = actionList[index];
        int targetIndex = Get2DVectorCompositeBindingIndex(action, vector, compositePart);
        //バインディングしたキーの名称を取得する
        keyboardTexts[index].text = InputControlPath.ToHumanReadableString(
            actionList[index].bindings[targetIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        keyboardTexts[index].color = new Color32(255, 255, 255, 255);

        _rebindingOperation.Dispose();


        //リバインディング時は空のアクションマップだったので通常のアクションマップに切り替え
        _pInputPlayer.SwitchCurrentActionMap("Player");
        _pInputUI.SwitchCurrentActionMap("UI");
        //リバインディングしたキーを保存(シーン開始時に読み込むため)
        PlayerPrefs.SetString("rebindSample", _pInputPlayer.actions.SaveBindingOverridesAsJson());
    }



    public void DefaultReBind()
    {
        _pInputPlayer.actions.RemoveAllBindingOverrides();
        _pInputUI.actions.RemoveAllBindingOverrides();
        PlayerPrefs.SetString("rebindSample", _pInputPlayer.actions.SaveBindingOverridesAsJson());
        PlayerPrefs.SetString("rebindUI", _pInputUI.actions.SaveBindingOverridesAsJson());

        LoadBindData(); //ロードにTextリセットがあるため
    }

    void GamePadUnComposite()
    {
        Debug.Log("Padが抜けました");
        foreach (var text in gamepadTexts)
        {
            text.color = new Color32(160, 160, 160, 255);
        }
    }

    //なくても良かった
    int Get2DVectorCompositeBindingIndex(InputAction inputAction, string actionName, string bindingName)
    {
        var tmpBindingSyntax = inputAction.ChangeCompositeBinding(actionName);
        Debug.Log(tmpBindingSyntax.bindingIndex);
        tmpBindingSyntax = tmpBindingSyntax.NextPartBinding(bindingName);

        Debug.Log(tmpBindingSyntax);
        return tmpBindingSyntax.bindingIndex;
    }

    private void LoadBindData()
    {
        //最初にDefaultTextをいれる
        foreach (var text in keyboardDefaultTexts)
        {
            int index = keyboardDefaultTexts.IndexOf(text);
            keyboardTexts[index].text = text;
        }
        foreach (var text in gamepadDefaultTexts)
        {
            int index = gamepadDefaultTexts.IndexOf(text);
            gamepadTexts[index].text = text;
        }

        if (Gamepad.current != null)
        {
            foreach (var text in gamepadTexts)
            {
                text.color = new Color32(255, 255, 255, 255);
            }

        }

        string rebinds = PlayerPrefs.GetString("rebindSample");
        string rebindUI = PlayerPrefs.GetString("rebindUI");

        if (!string.IsNullOrEmpty(rebinds) || !string.IsNullOrEmpty("rebindUI"))
        {
            _pInputPlayer.actions.LoadBindingOverridesFromJson(rebinds);
            _pInputUI.actions.LoadBindingOverridesFromJson(rebindUI);

            Debug.Log("RebindDataあり");
            //リバインディング状態をロード  
            foreach (var action in actionList)
            {

                //int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);  2DVector等Composite入力があるとうまくいかない
                keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);


                //keuboard用ロード
                switch (textIndex)
                {
                    case 5:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[3].effectivePath,   //左
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;
                    case 6:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[4].effectivePath,   //右
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;
                    case 7:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[1].effectivePath,   //上
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;
                    case 8:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[2].effectivePath,   //下
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;

                }

                if (Gamepad.current != null)
                {
                    Debug.Log("Padあり");
                    gamepadCompositeComplete = true;
                    //bindingIndex = action.GetBindingIndexForControl(action.controls[1]);
                    gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                    action.bindings[1].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);

                    //Gamepad用ロード
                    switch (textIndex)
                    {
                        case 5:
                            Debug.Log(textIndex);
                            gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                            action.bindings[5].effectivePath,   //LStick
                            InputControlPath.HumanReadableStringOptions.OmitDevice);
                            break;
                        case 6:
                            Debug.Log(textIndex);
                            gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                            action.bindings[6].effectivePath,   //D-Pad
                            InputControlPath.HumanReadableStringOptions.OmitDevice);
                            break;
                        case 7:
                            Debug.Log(textIndex);
                            gamepadTexts[textIndex].text = "―";
                            //gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                            //action.bindings[5].effectivePath,
                            //InputControlPath.HumanReadableStringOptions.OmitDevice);
                            break;
                        case 8:
                            Debug.Log(textIndex);
                            //gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                            //action.bindings[6].effectivePath,
                            //InputControlPath.HumanReadableStringOptions.OmitDevice);
                            gamepadTexts[textIndex].text = "―";
                            break;

                    }

                }
                textIndex++;
            }
            textIndex = 0;
        }


    }

}


