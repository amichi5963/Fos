using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class TouchSE : MonoBehaviour
{
    [SerializeField] float pitchRange = 0.1f;
    protected AudioSource source;
    [SerializeField] List<AudioClip> listAudioClips = new List<AudioClip>();
    [SerializeField] bool PlayOnce = false;
    bool played = false;
    private void Awake()
    {
        source = GetComponents<AudioSource>()[0];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!(played&&PlayOnce))
        PlayTouchSE();
    }
    public void PlayTouchSE()
    {
        source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        source.PlayOneShot(listAudioClips[Random.Range(0, listAudioClips.Count)]);
        played = true;
    }
}
