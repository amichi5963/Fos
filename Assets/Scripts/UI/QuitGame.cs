using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        QuitGameMethod();
    }

    //�Q�[���I��
    private void QuitGameMethod()
    {
        //Esc�������ꂽ��
        if (Keyboard.current.escapeKey.isPressed)
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
        }

    }
}