using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLuncher : MonoBehaviour
{
    public static StartLuncher Instance { get; private set; }

    //// —— ‘回顾’ ——
    //[Serializable]
    //public class ReviewElement
    //{
    //    [SerializeField] public Sprite TargetImg;
    //    [SerializeField] public List<string> Asides = new List<string>();
    //}

    //private List<ReviewElement> reviews = new List<ReviewElement>();

    //private int _currReviewIndex = 0;

    [SerializeField] private List<string> _startAsides = new List<string>();

    [SerializeField] private List<string> _endAsides = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;
    }

    private void Start()
    {
        UIManager.Instance.ReviewGUIInitialized();

        StartCoroutine(Luncher());
    }

    public IEnumerator Luncher()
    {
        yield return StartCoroutine(UIManager.Instance.StartReview(_startAsides));
    }

    public List<string> GetEndAsides() => _endAsides;
}
