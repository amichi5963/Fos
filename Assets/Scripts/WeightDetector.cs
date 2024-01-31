using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class WeightDetector : MonoBehaviour
{
    [SerializeField] int detectStones = 1;
    [SerializeField] float detectRange = 2f;
    [SerializeField] Vector3 boxSize;
    Rigidbody rb;
    [SerializeField] float pitchRange = 0.1f;
    protected AudioSource source;
    [SerializeField] List<AudioClip> listAudioClips = new List<AudioClip>();
    [SerializeField] Animation anim;
    bool able = true;

    private void Awake()
    {
        source = GetComponents<AudioSource>()[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (able)
        {
            int count = 0;
            RaycastHit[] detected = Physics.BoxCastAll(transform.position, boxSize, Vector3.up * detectRange);
            int playersCount = 0;
            foreach (RaycastHit hit in detected)
            {
                if (hit.collider.enabled && hit.collider.CompareTag("Stone"))
                {
                    count++;
                }
                if (hit.collider.enabled && hit.collider.CompareTag("Player"))
                {
                    PlayerController playerController = hit.collider.GetComponent<PlayerController>();
                    if (playerController != null)
                        playersCount = playerController.getStonesCount();
                }
            }
            count += playersCount;
            //Debug.Log("count: " + count + " playersCount: " + playersCount);
            if (count >= detectStones)
            {
                able = false;
                source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
                source.PlayOneShot(listAudioClips[Random.Range(0, listAudioClips.Count)]);
                if (anim != null)
                {
                    anim.Play("Take 001");
                }
                else
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }
            }
        }
    }
}