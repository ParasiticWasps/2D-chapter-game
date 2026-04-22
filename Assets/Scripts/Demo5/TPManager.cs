using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPManager : MonoBehaviour
{
    private static TPManager _instance;

    public static TPManager Get()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<TPManager>();
        }
        return _instance;
    }

    public enum TPType { One, Two, Tree, Four, None }

    [Serializable]
    public class TPInfo
    {
        [SerializeField] public TPType type;
        [SerializeField] public GameObject sceneObject;
        [SerializeField] public GameObject interGroup;
    }

    private List<TPInfo> _infos = new List<TPInfo>();

    private TPInfo _currInfo;

    private void Start()
    {
        SwitchScene(TPType.One);
    }

    public void AddTPElement(TPInfo tp)
    {
        _infos.Add(tp);
    }

    public void SwitchScene(TPType t)
    {
        if (t == TPType.None) return;

        foreach (var i in _infos)
        {
            i.sceneObject.gameObject.SetActive(false);
            i.interGroup.gameObject.SetActive(false);
        }

        _currInfo = GetInfo(t);
        if (_currInfo != null)
        {
            _currInfo.sceneObject?.gameObject.SetActive(true);
            _currInfo.interGroup?.gameObject.SetActive(true);
        }
    }

    public TPInfo GetInfo(TPType t)
    {
        TPInfo info = _infos.Find(_ => _.type == t);
        return info;
    }
}
