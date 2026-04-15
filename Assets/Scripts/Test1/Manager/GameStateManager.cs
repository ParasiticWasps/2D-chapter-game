using UnityEngine;

public enum GameState
{
    Normal,      // 正常探索
    Dialog,      // 对话中
    Recall,      // 回忆中
    Blank        // 留白时刻
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("当前状态")]
    public GameState currentState = GameState.Normal;

    [Header("调试")]
    public bool debugStateChanges = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 切换状态
    public void SetState(GameState newState)
    {
        if (currentState != newState)
        {
            GameState oldState = currentState;
            currentState = newState;

            if (debugStateChanges)
            {
                Debug.Log($"游戏状态: {oldState} -> {newState}");
            }

            // 可以在这里触发状态进入/退出事件
        }
    }

    // 检查当前是否可以移动
    public bool CanPlayerMove()
    {
        return currentState == GameState.Normal;
    }

    // 检查当前是否可以互动（点击物体）
    public bool CanInteract()
    {
        // 在对话和回忆中不能互动
        return currentState == GameState.Normal;
    }
}