using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndInteraction : MonoBehaviour
{
    private bool _isTrigger = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isTrigger == false)
        {
            _isTrigger = true;
            StartCoroutine(StartLuncher.Instance.Luncher());
        }
    }
}
