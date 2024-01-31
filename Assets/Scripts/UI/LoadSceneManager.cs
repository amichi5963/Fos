using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    //[SerializeField] GameObject canvas;
    //[SerializeField] GameObject Startup_Fade_Obj;
    //[SerializeField] GameObject Loading_Fade_Obj;

    //[SerializeField] GameObject TriangleObj;

    [SerializeField] bool startEffects = false;
    bool nowLoading = false;

    [SerializeField] Image Startup_Fade;
    [SerializeField] Image Loading_Fade;

    private void Start()
    {

        //Startup_Fade = Startup_Fade.GetComponent<Image>();
        //Loading_Fade = Loading_Fade.GetComponent<Image>();
        //WallPaper_End = WallPaper_End_Obj.GetComponent<Image>();


        if (startEffects)
        {
            Startup_Fade.color = new Color32(0,0,0,255);

            float duration = 1f;
            DOTween.ToAlpha(() => Startup_Fade.color, x => Startup_Fade.color = x, 0f , duration);

            //canvas.SetActive(true);
            //WallPaper_Start_Obj.SetActive(true);
            //TriangleObj.SetActive(true);
            //DOTween.To(() => WallPaper_Start.fillAmount, x => WallPaper_Start.fillAmount = x, 0, 1f).SetEase(Ease.InOutQuart).OnComplete(() => canvas.SetActive(false));
            //TriangleObj.transform.DOScale(Vector3.one * 0.75f, 0.5f).SetEase(Ease.OutQuart).SetLoops(-1);
            //TriangleObj.transform.DORotate(new Vector3(0, 0, -180), 5).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }
    }

    /// <summary>
    /// シーンを読み込む
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, float deleaySeconds = 0)
    {
        if (!nowLoading)
        {
            // コルーチンでロード画面を実行
            StartCoroutine(LoadSceneExecute(name, deleaySeconds));

            nowLoading = true;
        }
    }

    IEnumerator LoadSceneExecute(string name, float deleaySeconds = 0)
    {
        yield return new WaitForSeconds(deleaySeconds);

        Loading_Fade.color = new Color32(0, 0, 0, 0);
        float duration = 1f;
        DOTween.ToAlpha(() => Loading_Fade.color, x => Loading_Fade.color = x, 1f, duration);

        //TriangleObj.SetActive(true);
        //WallPaper_End_Obj.SetActive(true);
        //canvas.SetActive(true);

        //DOTween.To(() =>WallPaper_End.fillAmount, x => WallPaper_End.fillAmount = x, 1, 1f).SetEase(Ease.InOutQuart);
        //TriangleObj.transform.DOScale(Vector3.one * 0.75f, 0.5f).SetEase(Ease.OutQuart).SetLoops(-1);
        //TriangleObj.transform.DORotate(new Vector3(0, 0, -180), 5).SetEase(Ease.Linear).SetLoops(-1,LoopType.Incremental);

        yield return new WaitForSeconds(2f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        asyncLoad.allowSceneActivation = false;
        // スライダーの値更新とロード画面の表示

        while (true)
        {
            yield return null;

            if (asyncLoad.progress >= 0.9f)
            {
                // ロードバーが100%になっても1秒だけ表示維持
                yield return new WaitForSeconds(3f);

                asyncLoad.allowSceneActivation = true;
                break;
            }
        }

        // ロード画面の非表示
        //canvas.SetActive(false);
    }
}