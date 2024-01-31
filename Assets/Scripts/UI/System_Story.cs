using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class System_Story : MonoBehaviour
{
    [SerializeField] GameObject SkipText;
    [SerializeField] LoadSceneManager loadSceneManager;
    [SerializeField] VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += LoopPointReached;
        videoPlayer.Play();
    }

    public void LoopPointReached(VideoPlayer vp)
    {
        // 動画再生完了時の処理
        loadSceneManager.LoadScene("Level_01");
    }

    // Update is called once per frame
    void Update()
    {

        //スキップ方法表示
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SkipText.SetActive(true);
        }

        //スキップ
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            loadSceneManager.LoadScene("TitleScene");
        }
    }
}
