using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // —— 结尾元素 ——
    [SerializeField] private Image           _endMaskImg;
    [SerializeField] private TextMeshProUGUI _endText;
    [SerializeField] private List<string>    _currEndList = new List<string>();

    // —— 模糊UI背景和旁白 ——
    [SerializeField] private Image           _blurImage;
    [SerializeField] private TextMeshProUGUI _narrText;
    [SerializeField] private float           _intervalBlurDuration = 3.0f;

    // —— ‘刮掉’图层，交互UI
    [SerializeField] private List<Image> _layerImgs = new List<Image>();
    [SerializeField] private Image       _maskImg;
    [SerializeField] private Image       _scratchBlurImg;
    [SerializeField] private GameObject  _fakeLayer; // 虚假的图层
    private int                          _currLayerIndex = 0;
    private int                          _scratchCount   = 0;

    // —— 私有通用成员 ——
    private const float _fadeMinVlue = 0.0f;
    private const float _fadeMaxVlue = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        InitializeGui();
    }

    private void InitializeGui()
    {
        _maskImg?.DOFade(_fadeMinVlue, 0.0f);
        _endMaskImg?.DOFade(_fadeMinVlue, 0.0f).OnComplete(() => _endMaskImg.gameObject.SetActive(false));
        _blurImage?.DOFade(_fadeMinVlue, 0.0f);
        _scratchBlurImg?.DOFade(_fadeMinVlue, 0.0f);
        foreach (var img in _layerImgs)
        {
            img?.DOFade(_fadeMinVlue, 0.0f);
        }

        _endText.text  = string.Empty;
        _narrText.text = string.Empty;
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
    //  模糊背景开始旁白
    // ════════════════════════════════════════════════════
    public void StartBlurAside()
    {
        StartCoroutine(StartBlurAsideCoroutine());
    }

    private IEnumerator StartBlurAsideCoroutine()
    {
        // 循环显示旁白，直到旁白列表显示完毕
        for (int i = 0; i < ASideController.Instance.GetSides().Count; i++)
        {
            yield return new WaitForSeconds(_intervalBlurDuration);

            // 开始模糊停止玩家移动
            GameStateManager gameState = GameObject.FindObjectOfType<GameStateManager>();
            if (gameState) gameState.SetState(GameState.Dialog);

            // 等待显示第一段旁白
            yield return StartCoroutine(StartBlurFadeIn());

            // 等待结束第一段旁白
            yield return StartCoroutine(StartBlurFadeOut());

            // 解除玩家移动
            if (gameState) gameState.SetState(GameState.Normal);
        }

        // 执行刮开图层的交互
        StartCoroutine(ScratchOffInteraction());
    }

    private IEnumerator StartBlurFadeIn()
    {
        // 模糊背景渐入
        _narrText.text = string.Empty;
        _narrText.DOFade(1.0f, 2.0f);
        DoFadeImg(_blurImage, 1.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        // 逐字逐句显示旁白
        string currAside = ASideController.Instance.GetCurrAside();
        yield return StartCoroutine(WordByWord(_narrText, currAside));

        // 等待玩家点击屏幕开始下一段旁白
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    private IEnumerator StartBlurFadeOut()
    {
        // 模糊背景渐出
        _narrText.DOFade(0.0f, 2.0f);
        DoFadeImg(_blurImage, 0.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        // 等待玩家点击屏幕开始下一段旁白
        // yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    // ════════════════════════════════════════════════════
    //  刮开图层，交互UI
    // ════════════════════════════════════════════════════
    private IEnumerator ScratchOffInteraction()
    {
        yield return new WaitForSeconds(2.0f);

        GameStateManager gameState = GameObject.FindObjectOfType<GameStateManager>();
        if (gameState) gameState.SetState(GameState.Dialog);

        // 繁荣图渐入
        _scratchBlurImg.DOFade(_fadeMaxVlue, 2.0f);
        LayerFadeIn(2.0f); 

        yield return new WaitForSeconds(2.0f);

        // 破败图一渐入
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        LayerFadeIn(2.0f);
        Debug.Log(111);
        yield return new WaitForSeconds(2.0f);

        // 破败图二渐入
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        LayerFadeIn(2.0f);
        Debug.Log(222);
        yield return new WaitForSeconds(2.0f);

        // ‘白闪’显示完整破败图
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        _fakeLayer.gameObject.SetActive(false);
        DoFadeImg(_scratchBlurImg, _fadeMinVlue, 0.0f);
        foreach (var img in _layerImgs)
        {
            DoFadeImg(img, _fadeMinVlue, 0.0f);
        }

        DoFadeImg(_maskImg, _fadeMaxVlue, 0.0f);
        DoFadeImg(_maskImg, _fadeMinVlue, 3.5f);
        if (gameState) gameState.SetState(GameState.Normal);

        // 结束
        yield return new WaitForSeconds(8.0f);

        UIManager.Instance.ShowEndPanel(() => { SceneManager.LoadSceneAsync(3); });
    }

    private void LayerFadeIn(float duration)
    {
        if (_currLayerIndex >= _layerImgs.Count) return;

        _layerImgs[_currLayerIndex].DOFade(_fadeMaxVlue, duration);
        _currLayerIndex++;
    }

    // ════════════════════════════════════════════════════
    //  工具
    // ════════════════════════════════════════════════════

    private void DoFadeImg(Image img, float alpha, float duration, Action onComplate = null)
    {
        img.DOFade(alpha, duration).OnComplete(() => onComplate?.Invoke());
    }

    private IEnumerator WordByWord(TextMeshProUGUI t, string words)
    {
        foreach (var word in words)
        {
            t.text += word;
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
}
