using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.SceneManagement;

public class ShadowManager : MonoBehaviour
{
    public static ShadowManager Instance;

    // —— 成员变量 ——
    private readonly Dictionary<int, bool> _shadowLiveBuffer = new Dictionary<int, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        // 初始化
        for (int i = 0; i < 3; i++)
        {
            _shadowLiveBuffer.Add(i, false);
        }
    }

    public void UpdateShadowBuffer(int shadowId)
    {
        if (_shadowLiveBuffer.ContainsKey(shadowId))
        {
            _shadowLiveBuffer[shadowId] = true;
            if (CheckFinishAll())
            {
                Debug.Log("所有的Interaction全部被点击了，执行结束程序...");

                GameObject go = GameObject.FindWithTag("Player");
                if (go == null) return;
                PlayerController _ctrl = go.GetComponent<PlayerController>();
                _ctrl.enabled          = false;

                UIManager.Instance.ShowEndPanel(() => { SceneManager.LoadSceneAsync(3); });
            }
        }
    }

    /// <summary>
    /// 检查是否全部点击了
    /// </summary>
    /// <returns></returns>
    private bool CheckFinishAll()
    {
        bool isFinished = true;

        foreach (var pair in _shadowLiveBuffer)
        {
            isFinished &= pair.Value == true;
        }
        return isFinished;
    }
}
