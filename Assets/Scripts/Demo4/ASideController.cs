using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASideController : MonoBehaviour
{
    public static ASideController Instance { get; private set; }

    // —— 旁边列表 ——
    [SerializeField] private List<string> _sides = new List<string>();

    private int _currentSideIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;
    }

    public string GetCurrAside()
    {
        if (_currentSideIndex < 0 || _currentSideIndex >= _sides.Count) { return string.Empty; }
        return _sides[_currentSideIndex++];
    }

    public List<string> GetSides() => _sides;
}
