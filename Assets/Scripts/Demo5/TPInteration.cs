using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TPInteration : MonoBehaviour
{
    public Vector3 startPostion;

    [SerializeField] private TPManager.TPInfo _info;

    [SerializeField] private TPManager.TPType _nextTP;

    private void Awake()
    {
        TPManager.Get().AddTPElement(_info);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            UIManager.Instance.Flash(3.0f, () => { });
            collision.transform.localPosition = startPostion;
            TPManager.Get().SwitchScene(_nextTP);
        }
    }
}
