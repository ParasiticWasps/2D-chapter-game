using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoorManager : MonoBehaviour
{
    public static PoorManager Instance;

    // —— 私有成员 ——
    private Dictionary<NpcInteraction.NpcInterType, bool> _npcLiverBuffer = new Dictionary<NpcInteraction.NpcInterType, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(Instance); return; }
        Instance = this;

        foreach (NpcInteraction.NpcInterType type in System.Enum.GetValues(typeof(NpcInteraction.NpcInterType)))
        {
            _npcLiverBuffer.Add(type, false);
        }
    }

    /// ════════════════════════════════════════════════════
    /// 对外接口
    /// ════════════════════════════════════════════════════
    public bool CheckIn()
    {
        NpcInteraction.NpcInterType[] typeArray = new NpcInteraction.NpcInterType[]
        {
            NpcInteraction.NpcInterType.Poor_1,
            NpcInteraction.NpcInterType.Poor_2,
            NpcInteraction.NpcInterType.Poor_3
        };

        bool isFinished = true;

        foreach (var type in typeArray)
        {
            isFinished &=_npcLiverBuffer.ContainsKey(type) && _npcLiverBuffer[type];
        }
        return isFinished;
    }

    public void SetNpcLiver(NpcInteraction.NpcInterType type)
    {
        if (_npcLiverBuffer.ContainsKey(type)) _npcLiverBuffer[type] = true;

        if (CheckIn())
        {
            GameObject go = GameObject.FindWithTag("Player");
            if (go == null) return;
            PlayerController _ctrl = go.GetComponent<PlayerController>();
            _ctrl.enabled = false;

            UIManager.Instance.ShowEndPanel(() => { SceneManager.LoadSceneAsync(3); });
        }
    }
}
