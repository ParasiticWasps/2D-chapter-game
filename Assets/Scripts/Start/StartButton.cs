using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Button           _selfButton;
    private TextMeshProUGUI  _selfText;
    private AudioSource      _audioSource;

    [SerializeField] private AudioClip _clickClip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.loop        = false;
        _audioSource.playOnAwake = false;
        _audioSource.clip        = _clickClip;

        _selfButton = GetComponent<Button>();
        _selfText = _selfButton?.GetComponentInChildren<TextMeshProUGUI>();
        _selfText?.DOFade(0.0f, 0.0f);

        _selfButton.enabled = false;
    }

    public void OnFade()
    {
        _selfText?.DOFade(1.0f, 2.5f).OnComplete(() =>
        {
            _selfButton.enabled = true;
            //transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
        });
    }

    public void OnClicked()
    {
        _audioSource.Play();
        _selfText?.DOFade(0.0f, 2.5f).OnComplete(() =>
        {
            _selfButton.enabled = false;
            //transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
        });
    }

    // ====================
    // 事件
    // ====================

    // 当鼠标指针进入UI元素区域时，系统会自动调用这个方法
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f);
    }

    // 当鼠标指针离开UI元素区域时，系统会自动调用这个方法
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1f);
    }
}
