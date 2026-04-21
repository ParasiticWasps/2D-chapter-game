using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InterAudioManager : MonoBehaviour
{
    public static InterAudioManager Instance;

    private AudioSource _interAudio;

    public AudioClip _interClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        _interAudio = GetComponent<AudioSource>();
        _interAudio.loop        = false;
        _interAudio.playOnAwake = false;
        _interAudio.clip        = _interClip;
    }

    public void PlayInterAudio()
    {
        //_interAudio.Stop();
        _interAudio.clip = _interClip;
        _interAudio.Play();
    }
}
