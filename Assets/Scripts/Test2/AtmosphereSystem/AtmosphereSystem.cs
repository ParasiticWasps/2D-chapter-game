using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class AtmosphereSystem : MonoBehaviour
{
    public Volume globalVolume;
    public Image whiteOverlay;
    public AudioMixer audioMixer;

    private ColorAdjustments colorAdjustments;
    private LiftGammaGain liftGammaGain;

    void Awake()
    {
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume 未赋值！");
            return;
        }

        if (!globalVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("Volume 中没有 ColorAdjustments！");
        }

        if (!globalVolume.profile.TryGet(out liftGammaGain))
        {
            Debug.LogError("Volume 中没有 LiftGammaGain！");
        }

        if (whiteOverlay == null)
        {
            Debug.LogError("WhiteOverlay 未赋值！");
        }

        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer 未赋值！");
        }
    }
    public void ApplyProfile(AtmosphereProfile profile)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(profile));
    }

    IEnumerator Transition(AtmosphereProfile profile)
    {
        float duration = profile.transitionDuration;
        float time = 0f;

        float startSat = colorAdjustments.saturation.value;
        float startCon = colorAdjustments.contrast.value;
        float startExp = colorAdjustments.postExposure.value;
        float startGain = liftGammaGain.gain.value.x;
        float startWhite = whiteOverlay.color.a;

        audioMixer.GetFloat("EnvironmentVolume", out float startEnvVol);
        audioMixer.GetFloat("MusicVolume", out float startMusicVol);

        float targetEnv = Mathf.Lerp(-80f, 0f, profile.environmentVolume);
        float targetMusic = Mathf.Lerp(-80f, 0f, profile.musicVolume);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            colorAdjustments.saturation.value = Mathf.Lerp(startSat, profile.saturation, t);
            colorAdjustments.contrast.value = Mathf.Lerp(startCon, profile.contrast, t);
            colorAdjustments.postExposure.value = Mathf.Lerp(startExp, profile.exposure, t);

            float gainValue = Mathf.Lerp(startGain, profile.gain, t);
            liftGammaGain.gain.value = new Vector4(gainValue, gainValue, gainValue, 0);

            float whiteValue = Mathf.Lerp(startWhite, profile.whiteFade, t);
            Color c = whiteOverlay.color;
            c.a = whiteValue;
            whiteOverlay.color = c;

            audioMixer.SetFloat("EnvironmentVolume", Mathf.Lerp(startEnvVol, targetEnv, t));
            audioMixer.SetFloat("MusicVolume", Mathf.Lerp(startMusicVol, targetMusic, t));

            yield return null;
        }
    }
}