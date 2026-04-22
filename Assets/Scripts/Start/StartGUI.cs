using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartGUI : MonoBehaviour
{
    // —— 视频组件 ——
    [SerializeField] private VideoPlayer _videoPlay;
    [SerializeField] private VideoClip   _openingClip;
    [SerializeField] private VideoClip   _endClip;

    // —— UI组件 ——
    [SerializeField] private StartButton _startButton;

    private void Awake()
    {

    }

    private void Start()
    {
        StartCoroutine(OnOpeningAudioPlayEnd());
    }

    public void Update()
    {
        Debug.Log(_videoPlay.isPlaying);
    }

    public IEnumerator OnOpeningAudioPlayEnd()
    {
        _videoPlay.Play();

        yield return new WaitForSeconds(11.0f);

        //yield return new WaitUntil(() => _videoPlay.isPlaying == false);
        _startButton.OnFade();
    }

    public void OnClickStartEvent()
    {
        StartCoroutine(PlayEndAudioClip());
    }

    private IEnumerator PlayEndAudioClip()
    {
        _startButton.OnClicked();
        _videoPlay.clip = _endClip;
        _videoPlay.Play();
        yield return new WaitForSeconds(0.5f);

        BGMContorller.Get().FadeOut();

        yield return new WaitUntil(() => _videoPlay.isPlaying == false);
        SceneManager.LoadSceneAsync(1);
    }
}
