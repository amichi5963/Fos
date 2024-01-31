using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class RebindingInput : MonoBehaviour
{
    //PlayerInput�R���|�[�l���g������Q�[���I�u�W�F�N�g
    [SerializeField]
    private PlayerInput _pInputPlayer;
    [SerializeField]
    private PlayerInput _pInputUI;

    List<InputAction> actionList;

    //���o�C���f�B���O������InputAction���ځB
    InputAction attackAction;
    InputAction jumpAction;
    InputAction skillAction;
    InputAction changeLAction;
    InputAction changeRAction;
    InputAction moveAction;

    InputAction submitAction;
    InputAction cancelAction;
    InputAction pauseAction;

    //���o�C���f�B���O�e�L�X�g�B�A�N�V�����Ɠ������ԁBDefault�͏����ݒ�
    [SerializeField] List<TextMeshProUGUI> keyboardTexts;
    [SerializeField] List<string> keyboardDefaultTexts;

    [SerializeField] List<TextMeshProUGUI> gamepadTexts;
    [SerializeField] List<string> gamepadDefaultTexts;
    int textIndex = 0;



    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    //�r���ŃQ�[���p�b�h��}�����Ƃ��p
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

        //DefaultText�̕ۑ�
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


        //���łɃ��o�C���f�B���O�������Ƃ�����ꍇ�̓V�[���ǂݍ��ݎ��ɕύX�B
        LoadBindData();

    }


    private void Update()
    {
        //����Ă݂͂����Ǔr������̐ڑ�����L�[�o�C���h�ł��Ȃ�����
        if (Gamepad.current != null && !gamepadCompositeComplete && (_pInputPlayer.currentActionMap.name == "Player") && (_pInputUI.currentActionMap.name == "UI"))
        {
            Debug.Log("�Q�[���p�b�hLoading");
            LoadBindData();
            gamepadCompositeComplete = true;
        }
        else if (Gamepad.current == null && gamepadCompositeComplete)
        {
            Debug.Log("�Q�[���p�b�hdisconect");
            GamePadUnComposite();
            gamepadCompositeComplete = false;
        }


    }



    public void StartRebinding(int index)
    {
        //Submit�A�N�V�����̓��͂ɂ���ăL�[�{�[�h���Q�[���p�b�h������
        var s = _pInputUI.actions["Submit"].activeControl.path;

        //�{�^���̌�쓮��h�����߁A���������A�N�V�����}�b�v�ɐ؂�ւ�
        _pInputPlayer.SwitchCurrentActionMap("Blank");
        _pInputUI.SwitchCurrentActionMap("Blank");

        if (s.Contains("Keyboard"))
        {
            Debug.Log("keyboard");
            keyboardTexts[index].color = new Color32(176, 16, 48, 255);
            //Fire�{�^���̃��o�C���f�B���O�J�n
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(actionList[index].GetBindingIndexForControl(actionList[index].controls[0]))  //contols��index�͕ϐ���n���ƃG���[�ɂȂ�₷�������̂Œ�
                .WithControlsExcluding("Mouse") //.OnMatchWaitForAnother(0.1f)������ƌ���L�[���͂��󂯕t���Ȃ���������
                .OnComplete(operation => RebindComplete(index, 0))
                .Start();
        }
        else
        {
            Debug.Log("GamePad");
            gamepadTexts[index].color = new Color32(176, 16, 48, 255);
            //Fire�{�^���̃��o�C���f�B���O�J�n
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(actionList[index].GetBindingIndexForControl(actionList[index].controls[1]))
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => RebindComplete(index, 1))
                .Start();
        }

    }

    public void RebindComplete(int index, int control)
    {
        //�A�N�V�����̃R���g���[��(�o�C���f�B���O�����R���g���[��)�̃C���f�b�N�X���擾


        if (control == 0)
        {
            int bindingIndex = actionList[index].GetBindingIndexForControl(actionList[index].controls[control]);
            //Keyboard�̎�
            //�o�C���f�B���O�����L�[�̖��̂��擾����
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
            //GamePad��Move�p
            //GetBindingIndexForControl(actionList[index].controls[control])�̓G���[�̉����}�W��
            //���ǃA�N�V�����R���g���[����index��index�Ɠ����Ȃ̂ł��̂܂ܗ��p
            gamepadTexts[index].text = InputControlPath.ToHumanReadableString(
                actionList[index].bindings[index].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            gamepadTexts[index].color = new Color32(255, 255, 255, 255);
        }

        _rebindingOperation.Dispose();


        //���o�C���f�B���O���͋�̃A�N�V�����}�b�v�������̂Œʏ�̃A�N�V�����}�b�v�ɐ؂�ւ�
        _pInputPlayer.SwitchCurrentActionMap("Player");
        _pInputUI.SwitchCurrentActionMap("UI");
        //���o�C���f�B���O�����L�[��ۑ�(�V�[���J�n���ɓǂݍ��ނ���)
        PlayerPrefs.SetString("rebindSample", _pInputPlayer.actions.SaveBindingOverridesAsJson());
        PlayerPrefs.SetString("rebindUI", _pInputUI.actions.SaveBindingOverridesAsJson());
    }

    //Move�pStart�B�L�[�{�[�h��2DVector�̊e�����B�Q�[���p�b�h�̓X�e�B�b�N��D-Pad�I��
    public void StartRebinding(int index, string vector, string compositePart)
    {
        var s = _pInputUI.actions["Submit"].activeControl.path;

        _pInputPlayer.SwitchCurrentActionMap("Blank");
        _pInputUI.SwitchCurrentActionMap("Blank");

        if (s.Contains("Keyboard"))
        {
            var action = actionList[index];
            //vector��2DVector�AcompositePart�͊e������
            int targetIndex = Get2DVectorCompositeBindingIndex(action, vector, compositePart);  //�����targetIndex���擾���Ă��邪2DVector��[0]��2DVector�ŏ㉺���E��[1]�`[4]�Ȃ̂Œ��ł���������

            Debug.Log("Movekeyboard");
            Debug.Log(targetIndex);
            keyboardTexts[index].color = new Color32(176, 16, 48, 255);
            //�L�[�{�[�h�̊e�����ɑ΂��ă��o�C���f�B���O�J�n
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(targetIndex)
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => RebindComplete(index, vector, compositePart))  //�L�[�{�[�h�p��RebindComplete
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
            //Move�A�N�V�����̃��o�C���f�B���O�J�n
            _rebindingOperation = actionList[index].PerformInteractiveRebinding()
                .WithTargetBinding(index)   //actionList[index].GetBindingIndexForControl(actionList[index].controls[index]))�ł͏E���Ȃ��B��͂�G���[�̉���
                .WithControlsExcluding("Mouse")
                .OnComplete(operation => RebindComplete(index, index))
                .Start();
        }

    }

    //MoveKeyBoard�p
    public void RebindComplete(int index, string vector, string compositePart)
    {
        //�A�N�V�����̃R���g���[��(�o�C���f�B���O�����R���g���[��)�̃C���f�b�N�X���擾
        var action = actionList[index];
        int targetIndex = Get2DVectorCompositeBindingIndex(action, vector, compositePart);
        //�o�C���f�B���O�����L�[�̖��̂��擾����
        keyboardTexts[index].text = InputControlPath.ToHumanReadableString(
            actionList[index].bindings[targetIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        keyboardTexts[index].color = new Color32(255, 255, 255, 255);

        _rebindingOperation.Dispose();


        //���o�C���f�B���O���͋�̃A�N�V�����}�b�v�������̂Œʏ�̃A�N�V�����}�b�v�ɐ؂�ւ�
        _pInputPlayer.SwitchCurrentActionMap("Player");
        _pInputUI.SwitchCurrentActionMap("UI");
        //���o�C���f�B���O�����L�[��ۑ�(�V�[���J�n���ɓǂݍ��ނ���)
        PlayerPrefs.SetString("rebindSample", _pInputPlayer.actions.SaveBindingOverridesAsJson());
    }



    public void DefaultReBind()
    {
        _pInputPlayer.actions.RemoveAllBindingOverrides();
        _pInputUI.actions.RemoveAllBindingOverrides();
        PlayerPrefs.SetString("rebindSample", _pInputPlayer.actions.SaveBindingOverridesAsJson());
        PlayerPrefs.SetString("rebindUI", _pInputUI.actions.SaveBindingOverridesAsJson());

        LoadBindData(); //���[�h��Text���Z�b�g�����邽��
    }

    void GamePadUnComposite()
    {
        Debug.Log("Pad�������܂���");
        foreach (var text in gamepadTexts)
        {
            text.color = new Color32(160, 160, 160, 255);
        }
    }

    //�Ȃ��Ă��ǂ�����
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
        //�ŏ���DefaultText�������
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

            Debug.Log("RebindData����");
            //���o�C���f�B���O��Ԃ����[�h  
            foreach (var action in actionList)
            {

                //int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);  2DVector��Composite���͂�����Ƃ��܂������Ȃ�
                keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);


                //keuboard�p���[�h
                switch (textIndex)
                {
                    case 5:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[3].effectivePath,   //��
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;
                    case 6:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[4].effectivePath,   //�E
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;
                    case 7:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[1].effectivePath,   //��
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;
                    case 8:
                        keyboardTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                        action.bindings[2].effectivePath,   //��
                        InputControlPath.HumanReadableStringOptions.OmitDevice);
                        break;

                }

                if (Gamepad.current != null)
                {
                    Debug.Log("Pad����");
                    gamepadCompositeComplete = true;
                    //bindingIndex = action.GetBindingIndexForControl(action.controls[1]);
                    gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                    action.bindings[1].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);

                    //Gamepad�p���[�h
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
                            gamepadTexts[textIndex].text = "�\";
                            //gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                            //action.bindings[5].effectivePath,
                            //InputControlPath.HumanReadableStringOptions.OmitDevice);
                            break;
                        case 8:
                            Debug.Log(textIndex);
                            //gamepadTexts[textIndex].text = InputControlPath.ToHumanReadableString(
                            //action.bindings[6].effectivePath,
                            //InputControlPath.HumanReadableStringOptions.OmitDevice);
                            gamepadTexts[textIndex].text = "�\";
                            break;

                    }

                }
                textIndex++;
            }
            textIndex = 0;
        }


    }

}


