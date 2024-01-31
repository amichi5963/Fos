using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        QuitGameMethod();
    }

    //ゲーム終了
    private void QuitGameMethod()
    {
        //Escが押された時
        if (Keyboard.current.escapeKey.isPressed)
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }

    }
}