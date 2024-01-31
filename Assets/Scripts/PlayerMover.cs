using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 主にInputActionの設定クラス。PlayerController_Newで継承して使う
/// </summary>
/// 
public class PlayerMover : MonoBehaviour
{
    private GameInputs _gameInputs;
    protected Vector2 _moveInputValue;
    private void Awake()
    {
        //_rigidbody = GetComponent<Rigidbody>();

        // Input Actionインスタンス生成
        _gameInputs = new GameInputs();

        // Actionイベント登録
        _gameInputs.Player.Move.started += OnMove;
        _gameInputs.Player.Move.performed += OnMove;
        _gameInputs.Player.Move.canceled += OnMove;

        _gameInputs.Player.Call.performed += OnCall;
        _gameInputs.Player.Release.performed += OnRelease;
        _gameInputs.Player.SkillStart.performed += OnSkillStart;
        _gameInputs.Player.SkillCancel.performed += OnSkillCancel;
        _gameInputs.Player.StoneUp.performed += OnStoneUp;
        _gameInputs.Player.StoneDown.performed += OnStoneDown;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
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
        // Moveアクションの入力取得
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