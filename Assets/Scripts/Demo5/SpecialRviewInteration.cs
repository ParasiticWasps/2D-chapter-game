using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpecialRviewInteration : MonoBehaviour
{
    public List<string> sides = new List<string>();

    public float lineDuration = 1.0f;

    private bool _isSelect = false;

    [SerializeField] private GameObject _fakeLayer;
    [SerializeField] private GameObject _trueLayer;

    private void Awake()
    {
        _fakeLayer?.gameObject.SetActive(true);
        _trueLayer?.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isSelect == false)
        {
            _fakeLayer?.gameObject.SetActive(false);
            _trueLayer?.gameObject.SetActive(true);
            UIManager.Instance.Flash(1.5f, () => UIManager.Instance.SetSceneReviewText(sides, lineDuration));
        }
    }
}
