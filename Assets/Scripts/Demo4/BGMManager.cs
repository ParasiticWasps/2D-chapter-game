using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public enum BGMMode { Fake, Interaction, True };

    public static BGMManager Instance { get; private set; }

    private AudioSource _bgmAudio;

    public AudioClip _fakeBGM;
    public AudioClip _interBGM;
    public AudioClip _trueBGM;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        _bgmAudio = GetComponent<AudioSource>();
        if (_bgmAudio == null)
        {
            _bgmAudio = gameObject.AddComponent<AudioSource>();
        }
        _bgmAudio.loop        = true;
        _bgmAudio.playOnAwake = false;
        _bgmAudio.volume      = 0.4f;
    }

    public void SwitchBGM(BGMMode mode)
    {
        switch(mode)
        {
            case BGMMode.Fake:
                ReplaceBGM(_fakeBGM);
                break;
            case BGMMode.Interaction:
                ReplaceBGM(_interBGM);
                break;
            case BGMMode.True:
                ReplaceBGM(_trueBGM);
                break;
        }

    }

    private void ReplaceBGM(AudioClip clip)
    {
        _bgmAudio.Stop();
        _bgmAudio.clip = clip;
        _bgmAudio.Play();
    }
}
