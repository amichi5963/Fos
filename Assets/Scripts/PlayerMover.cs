using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ���InputAction�̐ݒ�N���X�BPlayerController_New�Ōp�����Ďg��
/// </summary>
/// 
public class PlayerMover : MonoBehaviour
{
    private GameInputs _gameInputs;
    protected Vector2 _moveInputValue;
    private void Awake()
    {
        //_rigidbody = GetComponent<Rigidbody>();

        // Input Action�C���X�^���X����
        _gameInputs = new GameInputs();

        // Action�C�x���g�o�^
        _gameInputs.Player.Move.started += OnMove;
        _gameInputs.Player.Move.performed += OnMove;
        _gameInputs.Player.Move.canceled += OnMove;

        _gameInputs.Player.Call.performed += OnCall;
        _gameInputs.Player.Release.performed += OnRelease;
        _gameInputs.Player.SkillStart.performed += OnSkillStart;
        _gameInputs.Player.SkillCancel.performed += OnSkillCancel;
        _gameInputs.Player.StoneUp.performed += OnStoneUp;
        _gameInputs.Player.StoneDown.performed += OnStoneDown;

        // Input Action���@�\�����邽�߂ɂ́A
        // �L��������K�v������
        _gameInputs.Enable();
    }
    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _gameInputs.Disable();
        }
        else
        {
            _gameInputs.Enable();
        }
    }
    public void pause(bool pause, float time)
    {
        _gameInputs.Disable();
        StartCoroutine(pauseCoroutine(pause, time));
    }
    IEnumerator pauseCoroutine(bool pause, float time)
    {
        yield return new WaitForSeconds(time);
        if (pause)
        {
            _gameInputs.Disable();
        }
        else
        {
            _gameInputs.Enable();
        }
    }
    public bool isApplicationPause()
    {
        return !_gameInputs.asset.enabled;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Move�A�N�V�����̓��͎擾
        _moveInputValue = context.ReadValue<Vector2>();
    }

    protected virtual void OnCall(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnRelease(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnSkillStart(InputAction.CallbackContext context)
    {
    }
    protected virtual void OnSkillCancel(InputAction.CallbackContext context)
    {
    }
    protected virtual void OnStoneUp(InputAction.CallbackContext context)
    {
    }
    protected virtual void OnStoneDown(InputAction.CallbackContext context)
    {
    }
}