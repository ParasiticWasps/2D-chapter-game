using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string     _aside;
    [SerializeField] private GameObject _targetImg;

    private Button          _selfButton;
    private TextMeshProUGUI _nameText;
    private Image           _bgImg;

    private const float FADE_MIN_VALUE = 0.0f;
    private const float FADE_MAX_VALUE = 1.0f;
    private const float FADE_DURATION  = 1.0f;

    private void Awake()
    {
        _selfButton = GetComponent<Button>();
        _bgImg      = GetComponent<Image>();
        _nameText   = GetComponentInChildren<TextMeshProUGUI>();

        _selfButton.onClick.AddListener(() => FadeInSpriteRenderer());

        _bgImg.DOFade(FADE_MIN_VALUE, 0f);
        _nameText.DOFade(FADE_MIN_VALUE, 0f).OnComplete(() => gameObject.SetActive(false));
    }

    public void FadeInSpriteRenderer()
    {
        StartCoroutine(FadeInSpriteCoroutine());
    }

    private IEnumerator FadeInSpriteCoroutine()
    {
        Image sprite = _targetImg.GetComponent<Image>();
        sprite.DOFade(FADE_MAX_VALUE, 2.5f);

        yield return StartCoroutine(UIManager.Instance.ReviewFinish());

        yield return StartCoroutine(UIManager.Instance.SelectTextCoroutine(_aside));

        yield return new WaitForSeconds(2.0f);

        sprite.DOFade(FADE_MIN_VALUE, 1.5f);
        yield return StartCoroutine(UIManager.Instance.SelectFinished(StartLuncher.Instance.GetEndAsides()));
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        _bgImg   .DOFade(FADE_MAX_VALUE, FADE_DURATION);
        _nameText.DOFade(FADE_MAX_VALUE, FADE_DURATION);
    }

    public void FadeOut()
    {
        _bgImg.DOFade(FADE_MIN_VALUE, FADE_DURATION);
        _nameText.DOFade(FADE_MIN_VALUE, FADE_DURATION).OnComplete(() => _selfButton.enabled = false);
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
