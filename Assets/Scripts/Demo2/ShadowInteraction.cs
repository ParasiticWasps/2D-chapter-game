using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowInteraction : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.DOFade(0.2f, 2.0f)
           .SetLoops(-1, LoopType.Yoyo)
           .SetEase(Ease.InOutSine);
    }

    private void OnMouseDown()
    {
        Debug.Log("ShadowInteration Mouse Down.");
    }
}
