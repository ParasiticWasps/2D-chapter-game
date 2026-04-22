using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMContorller : MonoBehaviour
{
    private static BGMContorller _instance;

    public static BGMContorller Get()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<BGMContorller>(true);
            if (_instance == null)
            {
                GameObject obj = new GameObject("BGMContorller");
                _instance = obj.AddComponent<BGMContorller>();
            }
        }
        return _instance;
    }

    private AudioSource _audioSource;

    [SerializeField] private AudioClip _clip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.playOnAwake = false;
        _audioSource.loop        = true;
        _audioSource.volume      = 0.0f;
        _audioSource.clip        = _clip;
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        _audioSource.Play();
        DOTween.To(() => _audioSource.volume, x => { _audioSource.volume = x; }, 0.3f, 2f).SetEase(Ease.InOutQuad);
    }

    public void FadeOut()
    {
        DOTween.To(() => _audioSource.volume, x => { _audioSource.volume = x; }, 0.0f, 3f).SetEase(Ease.InOutQuad)
            .OnComplete(() => _audioSource.Stop());
    }
}
