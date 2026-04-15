using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SceneEffectManager : MonoBehaviour
{
    public static SceneEffectManager Instance;

    [Header("后处理Volume")]
    public Volume postProcessVolume;

    // 后处理组件引用
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    [Header("饱和度控制")]
    public float normalSaturation = 0f;
    public float recallSaturation = -50f;
    public float blankSaturation = -30f;

    [Header("Vignette控制（用于相机聚焦）")]
    public bool useVignetteForFocus = true;
    public float normalVignetteIntensity = 0f;
    public float normalVignetteSmoothness = 0f;
    public float focusVignetteIntensity = 0.4f;
    public float focusVignetteSmoothness = 0.5f;
    public Color focusVignetteColor = Color.black;

    [Header("环境音效")]
    public AudioSource environmentAudio;
    public float normalEnvironmentVolume = 1f;
    public float recallEnvironmentVolume = 0.3f;
    public float blankEnvironmentVolume = 0.1f;

    [Header("过渡时间")]
    public float transitionTime = 1.5f;

    private float targetSaturation = 0f;
    private float targetEnvironmentVolume = 1f;

    // 临时效果标志
    private bool isTemporaryVignetteActive = false;
    private Coroutine vignetteCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (postProcessVolume != null)
        {
            // 获取ColorAdjustments
            if (!postProcessVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.LogWarning("Post Process Volume中没有Color Adjustments，饱和度控制将不可用");
            }

            // 获取Vignette
            if (!postProcessVolume.profile.TryGet(out vignette))
            {
                Debug.LogWarning("Post Process Volume中没有Vignette，聚焦效果将不可用");
            }
            else
            {
                // 初始化Vignette为正常值
                vignette.intensity.value = normalVignetteIntensity;
                vignette.smoothness.value = normalVignetteSmoothness;
            }
        }

        // 初始状态
        SetNormal();
    }

    void Update()
    {
        // 平滑过渡饱和度
        if (colorAdjustments != null)
        {
            float currentSaturation = colorAdjustments.saturation.value;
            float newSaturation = Mathf.Lerp(currentSaturation, targetSaturation, Time.deltaTime * 2f);
            colorAdjustments.saturation.value = newSaturation;
        }

        // 平滑过渡环境音量
        if (environmentAudio != null)
        {
            environmentAudio.volume = Mathf.Lerp(
                environmentAudio.volume,
                targetEnvironmentVolume,
                Time.deltaTime * 2f
            );
        }

        // 只有在没有临时Vignette效果时才平滑过渡到目标值
        if (vignette != null && !isTemporaryVignetteActive)
        {
            float currentIntensity = vignette.intensity.value;
            float newIntensity = Mathf.Lerp(currentIntensity, targetVignetteIntensity, Time.deltaTime * 2f);
            vignette.intensity.value = newIntensity;

            float currentSmoothness = vignette.smoothness.value;
            float newSmoothness = Mathf.Lerp(currentSmoothness, targetVignetteSmoothness, Time.deltaTime * 2f);
            vignette.smoothness.value = newSmoothness;
        }
    }

    // Vignette的目标值（用于常态控制）
    private float targetVignetteIntensity = 0f;
    private float targetVignetteSmoothness = 0f;

    public void SetNormal()
    {
        targetSaturation = normalSaturation;
        targetEnvironmentVolume = normalEnvironmentVolume;

        targetVignetteIntensity = normalVignetteIntensity;
        targetVignetteSmoothness = normalVignetteSmoothness;
    }

    public void SetRecallMode()
    {
        targetSaturation = recallSaturation;
        targetEnvironmentVolume = recallEnvironmentVolume;
        // 不改变Vignette目标值
    }

    public void SetBlankMode()
    {
        targetSaturation = blankSaturation;
        targetEnvironmentVolume = blankEnvironmentVolume;
        // 不改变Vignette目标值
    }

    // 触发Vignette聚焦效果
    public void TriggerFocusVignette(float duration, float intensity = -1f, float smoothness = -1f)
    {
        if (vignette == null || !useVignetteForFocus) return;

        if (vignetteCoroutine != null)
            StopCoroutine(vignetteCoroutine);

        vignetteCoroutine = StartCoroutine(FocusVignetteRoutine(
            duration,
            intensity > 0 ? intensity : focusVignetteIntensity,
            smoothness > 0 ? smoothness : focusVignetteSmoothness
        ));
    }

    IEnumerator FocusVignetteRoutine(float duration, float targetIntensity, float targetSmoothness)
    {
        isTemporaryVignetteActive = true;

        // 记录原始值（使用当前的常态目标值）
        float originalIntensity = targetVignetteIntensity;
        float originalSmoothness = targetVignetteSmoothness;
        Color originalColor = vignette.color.value;

        float elapsed = 0f;
        float fadeInTime = 0.5f;

        // 淡入Vignette
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInTime;

            vignette.intensity.value = Mathf.Lerp(originalIntensity, targetIntensity, t);
            vignette.smoothness.value = Mathf.Lerp(originalSmoothness, targetSmoothness, t);
            vignette.color.value = Color.Lerp(originalColor, focusVignetteColor, t);

            yield return null;
        }

        // 确保达到目标值
        vignette.intensity.value = targetIntensity;
        vignette.smoothness.value = targetSmoothness;
        vignette.color.value = focusVignetteColor;

        // 保持
        yield return new WaitForSeconds(duration);

        elapsed = 0f;
        float fadeOutTime = 0.5f;

        // 淡出Vignette
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutTime;

            vignette.intensity.value = Mathf.Lerp(targetIntensity, originalIntensity, t);
            vignette.smoothness.value = Mathf.Lerp(targetSmoothness, originalSmoothness, t);
            vignette.color.value = Color.Lerp(focusVignetteColor, originalColor, t);

            yield return null;
        }

        // 确保恢复到常态目标值
        vignette.intensity.value = originalIntensity;
        vignette.smoothness.value = originalSmoothness;
        vignette.color.value = originalColor;

        isTemporaryVignetteActive = false;
    }

    public void TriggerSilence(float duration)
    {
        StartCoroutine(SilenceRoutine(duration));
    }

    System.Collections.IEnumerator SilenceRoutine(float duration)
    {
        float originalEnvVolume = targetEnvironmentVolume;
        targetEnvironmentVolume = 0f;

        yield return new WaitForSeconds(duration);

        targetEnvironmentVolume = originalEnvVolume;
    }
}