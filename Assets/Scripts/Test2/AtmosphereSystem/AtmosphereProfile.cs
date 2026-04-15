using UnityEngine;

[CreateAssetMenu(menuName = "绘世书/Atmosphere Profile")]
public class AtmosphereProfile : ScriptableObject
{
    [Header("Color Adjustments")]
    [Range(-100, 100)]
    public float saturation = 0;

    [Range(-100, 100)]
    public float contrast = 0;

    [Range(-5f, 5f)]
    public float exposure = 0;

    [Header("Gain (空气感)")]
    [Range(-1f, 1f)]
    public float gain = 0;

    [Header("White Overlay")]
    [Range(0f, 1f)]
    public float whiteFade = 0;

    [Header("Audio")]
    [Range(0f, 1f)]
    public float environmentVolume = 1f;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Header("Transition")]
    public float transitionDuration = 1.5f;
}