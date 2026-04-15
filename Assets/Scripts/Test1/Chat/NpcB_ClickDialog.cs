using UnityEngine;
using System.Collections.Generic;

public class NpcB_ClickDialog : MonoBehaviour
{
    [Header("对话设置")]
    [TextArea(3, 5)]
    public string initialDialog = " ";  // 初始对话

    [Header("选项1 - 上前一步")]
    public string option1Text = "上前一步";
    [TextArea(2, 3)]
    public string option1Response = " ";  // 选择后的额外台词

    [Header("选项2 - 默默走开")]
    public string option2Text = "默默走开";

    [Header("冷却设置")]
    public float dialogCooldown = 3f;
    private float lastDialogTime = -10f;

    private GameStateManager gameStateManager;

    void Start()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();
    }

    void OnMouseDown()
    {
        // 检查是否可以互动
        if (gameStateManager != null && !gameStateManager.CanInteract())
        {
            Debug.Log("当前状态无法与宫女对话");
            return;
        }

        if (Time.time - lastDialogTime < dialogCooldown)
            return;

        ShowDialog();
    }

    void ShowDialog()
    {
        lastDialogTime = Time.time;

        List<DialogOption> options = new List<DialogOption>();

        options.Add(new DialogOption
        {
            optionText = option1Text,
            onSelected = () => {
                // 播放额外台词
                DialogManager.Instance.ShowDialog(option1Response, () => {
                    Debug.Log("选择了上前一步，情感铺垫");
                });
            }
        });

        options.Add(new DialogOption
        {
            optionText = option2Text,
            onSelected = () => {
                Debug.Log("选择了默默走开");
            }
        });

        DialogManager.Instance.ShowDialogWithOptions(initialDialog, options);
    }

    void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}