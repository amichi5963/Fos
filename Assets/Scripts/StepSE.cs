using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class StepSE : MonoBehaviour
{
    [SerializeField] float pitchRange = 0.1f;
    protected AudioSource source;
    [SerializeField] List<AudioClip> listAudioClips = new List<AudioClip>();
    [SerializeField] float interval = 0.5f;
    float timer = 0f;

    public bool isMoving = false;
    private void Awake()
    {
        source = GetComponents<AudioSource>()[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (isMoving == false)
        {
            return;
        }

        if (timer > interval)
        {
            timer = 0;
            PlayFootstepSE();
        }
    }
    public void PlayFootstepSE()
    {

        source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        source.PlayOneShot(listAudioClips[Random.Range(0, listAudioClips.Count)]);
    }
}
