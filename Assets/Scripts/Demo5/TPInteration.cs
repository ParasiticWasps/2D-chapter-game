using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TPInteration : MonoBehaviour
{
    public Vector3 startPostion;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.DOLocalMove(startPostion, 0.0f);
        }
    }
}
