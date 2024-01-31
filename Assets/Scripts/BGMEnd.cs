using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMEnd : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] float fadeStartTime = 1f;
    public double FadeOutSeconds = 1.0;
    bool IsFadeOut = false;
    double FadeDeltaTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fadeStartTime>0)
        {
            fadeStartTime -= Time.deltaTime;
            if (fadeStartTime <= 0)
            {
                IsFadeOut = true;
            }
        }
        if (IsFadeOut)
        {
            FadeDeltaTime += Time.deltaTime;
            if (FadeDeltaTime == FadeOutSeconds)
            {
                FadeDeltaTime = FadeOutSeconds;
                IsFadeOut = false;
            }
            audioSource.volume = (float)(1.0 - FadeDeltaTime / FadeOutSeconds);
        }
    }
}