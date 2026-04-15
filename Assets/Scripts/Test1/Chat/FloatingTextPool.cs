using UnityEngine;
using System.Collections.Generic;

public class FloatingTextPool : MonoBehaviour
{
    [Header("对象池设置")]
    public GameObject floatingTextPrefab;
    public int initialPoolSize = 3;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // 初始化对象池
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewTextObject();
        }
    }

    GameObject CreateNewTextObject()
    {
        GameObject obj = Instantiate(floatingTextPrefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    // 从池中获取一个文字对象
    public GameObject GetFloatingText()
    {
        if (pool.Count == 0)
        {
            CreateNewTextObject();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    // 回收文字对象
    public void ReturnFloatingText(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}