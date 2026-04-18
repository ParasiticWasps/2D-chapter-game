using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("UI组件")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public GameObject optionsContainer;
    public GameObject optionButtonPrefab;

    [Header("显示设置")]
    public float textSpeed = 0.05f;

    private GameStateManager gameStateManager;
    private List<GameObject> activeOptionButtons = new List<GameObject>(); // 跟踪当前激活的选项按钮

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

    void Start()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    // 显示单句对话（无选项）
    public void ShowDialog(string text, System.Action onComplete = null)
    {
        // 切换到对话状态
        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Dialog);

        StartCoroutine(ShowDialogRoutine(text, onComplete));
    }

    System.Collections.IEnumerator ShowDialogRoutine(string text, System.Action onComplete)
    {
        dialogPanel.SetActive(true);
        dialogText.text = "";

        // 逐字显示
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // 等待点击继续
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForEndOfFrame();

        dialogPanel.SetActive(false);

        // 对话结束，恢复Normal状态
        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Normal);

        onComplete?.Invoke();
    }

    // 显示带选项的对话
    public void ShowDialogWithOptions(string text, List<DialogOption> options)
    {
        // 切换到对话状态
        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Dialog);

        StartCoroutine(ShowDialogWithOptionsRoutine(text, options));
    }

    System.Collections.IEnumerator ShowDialogWithOptionsRoutine(string text, List<DialogOption> options)
    {
        dialogPanel.SetActive(true);
        dialogText.text = "";

        // 逐字显示
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // 先清除之前的所有选项按钮
        ClearOptionButtons();

        // 创建新的选项按钮
        CreateOptionButtons(options);
    }

    /// <summary>
    /// 清除所有已实例化的选项按钮
    /// </summary>
    private void ClearOptionButtons()
    {
        // 方法1：通过跟踪列表清除
        foreach (GameObject btn in activeOptionButtons)
        {
            if (btn != null)
                Destroy(btn);
        }
        activeOptionButtons.Clear();

        // 方法2：直接遍历容器清除（双重保障）
        //foreach (Transform child in optionsContainer.transform)
        //{
        //    Destroy(child.gameObject);
        //}
    }

    /// <summary>
    /// 创建选项按钮
    /// </summary>
    private void CreateOptionButtons(List<DialogOption> options)
    {
        foreach (var option in options)
        {
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsContainer.transform);

            // 添加到跟踪列表
            activeOptionButtons.Add(btnObj);

            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();

            if (btnText != null)
                btnText.text = option.optionText;

            // 为每个按钮添加点击事件
            btn.onClick.AddListener(() => {
                OnOptionSelected(option);
            });
        }
    }

    /// <summary>
    /// 选项被选中时的处理
    /// </summary>
    private void OnOptionSelected(DialogOption selectedOption)
    {
        // 关闭对话面板
        dialogPanel.SetActive(false);

        // 清除选项按钮
        ClearOptionButtons();

        // 恢复Normal状态
        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Normal);

        // 执行选项的回调
        selectedOption.onSelected?.Invoke();
    }

    // 隐藏对话（手动调用）
    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        ClearOptionButtons();

        if (gameStateManager != null)
            gameStateManager.SetState(GameState.Normal);
    }

    // 可选：当脚本销毁时清理（防止内存泄漏）
    void OnDestroy()
    {
        ClearOptionButtons();
    }
}
// 选项数据结构
[System.Serializable]
public class DialogOption
{
    public string optionText;           // 选项文字
    public System.Action onSelected;    // 选择后的回调
}
