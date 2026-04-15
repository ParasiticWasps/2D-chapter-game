using UnityEngine;

public enum CameraActionType
{
    PushIn,
    Pan,
    FocusTarget
}

[CreateAssetMenu(menuName = "绘世书/Camera Action Profile")]
public class CameraActionProfile : ScriptableObject
{
    public CameraActionType actionType;

    [Header("通用时间")]
    public float duration = 2f;

    [Header("Push In")]
    public float pushDistance = 1f;

    [Header("Pan")]
    public Vector3 panOffset;

    [Header("Focus Target")]
    public Transform focusTarget;
    public float focusOffset = 0f;
}