using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        QuitGameMethod();
    }

    //Q[IΉ
    private void QuitGameMethod()
    {
        //Escͺ³κ½
        if (Keyboard.current.escapeKey.isPressed)
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//Q[vCIΉ
#else
    Application.Quit();//Q[vCIΉ
#endif
        }

    }
}