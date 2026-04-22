using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RviewInteration : MonoBehaviour
{
    public List<string> sides = new List<string>();

    public float lineDuration = 1.0f;

    private bool _isSelect = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("RviewInteration On Trigger Enter." + _isSelect);
        if (collision.CompareTag("Player") && _isSelect == false)
        {
            _isSelect = true;
            UIManager.Instance.SetSceneReviewText(sides, lineDuration);
        }
    }
}
