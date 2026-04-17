using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // —— 结尾元素 ——
    [SerializeField] private Image           _endMaskImg;
    [SerializeField] private TextMeshProUGUI _endText;
    [SerializeField] private List<string>    _currEndList = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        InitializeGui();
    }

    private void InitializeGui()
    {
        _endMaskImg?.DOFade(0.0f, 0.0f).OnComplete(() => _endMaskImg.gameObject.SetActive(false));

        if (_endText != null)
        {
            _endText.text = string.Empty;
        }
    }

    // ==================
    // 开启结束面板
    // ==================
    public void ShowEndPanel(Action callback = null)
    {
        _endMaskImg.gameObject.SetActive(true);
        StartCoroutine(ShowEndCoroutine(callback));
    }

    private IEnumerator ShowEndCoroutine(Action callback = null)
    {
        // 黑屏渐入
        _endMaskImg.DOFade(1.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        // 渐入显示结语
        foreach (var text in _currEndList)
        {
            _endText.text = "";
            _endText.DOFade(1.0f, 0.0f);
            yield return StartCoroutine(DisplayWordByWord(text));

            yield return new WaitForSeconds(1f);

            _endText.DOFade(0.0f, 2.0f);
            yield return new WaitForSeconds(2.0f);
        }

        callback?.Invoke();
    }

    private IEnumerator DisplayWordByWord(string text)
    {
        _endText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            _endText.text += text[i];
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
    }

    // ════════════════════════════════════════════════════
    //  工具
    // ════════════════════════════════════════════════════

    private void DoFadeImg(Image img, float alpha, float duration)
    {
        img.DOFade(alpha, duration);
    }
}
