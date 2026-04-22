using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ASideController : MonoBehaviour
{
    public static ASideController Instance { get; private set; }

    // —— 容器列表 ——
    [SerializeField] private List<string>        _sides = new List<string>();
    [SerializeField] private List<BoxCollider2D> _fakeGroup = new List<BoxCollider2D>();
    [SerializeField] private List<BoxCollider2D> _trueGroup = new List<BoxCollider2D>();

    private int _currentSideIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;
    }

    private void Start()
    {
        // 隐藏‘饿殍’图触发点
        SetTrueGroupEnable(false);
    }

    public string GetCurrAside()
    {
        if (_currentSideIndex < 0 || _currentSideIndex >= _sides.Count) { return string.Empty; }
        return _sides[_currentSideIndex++];
    }

    public void SetFakeGroupEnable(bool enable)
    {
        foreach (var collider in _fakeGroup)
            collider.enabled = enable;
    }

    public void SetTrueGroupEnable(bool enable)
    {
        foreach (var collider in _trueGroup)
            collider.enabled = enable;
    }

    public void TryExecuteEndProgram()
    {
        foreach (var collider in _fakeGroup)
        {
            AsideInteration inter = collider.GetComponent<AsideInteration>();
            if (inter == null || inter.GetIsTrigger() == false) return;
        }

        foreach (var collider in _trueGroup)
        {
            AsideInteration inter = collider.GetComponent<AsideInteration>();
            if (inter == null || inter.GetIsTrigger() == false) return;
        }

        UIManager.Instance.ShowEndPanel(() => { SceneManager.LoadSceneAsync(5); });
    }

    public List<string> GetSides() => _sides;
}
