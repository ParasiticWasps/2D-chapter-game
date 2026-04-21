using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class AsideInteration : MonoBehaviour
{
    public UnityEvent OnTriggerComplete;

    private bool _isTrigger = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isTrigger == false)
        {
            StartCoroutine(AsideTriggerCoroutine());
        }
    }

    private IEnumerator AsideTriggerCoroutine()
    {
        yield return StartCoroutine(UIManager.Instance.NextBlurAside());

        _isTrigger = true;
        OnTriggerComplete?.Invoke();
        ASideController.Instance.TryExecuteEndProgram();
    }

    public bool GetIsTrigger() => _isTrigger;
}
