using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using TMPro;

public class OpeningSceneController : MonoBehaviour
{
    [Header("UI组件")]
    public Image blackScreen;           // 黑屏遮罩
    public TextMeshProUGUI narrationText;          // 旁白文本
    public TextMeshProUGUI clickHintText;          // 点击提示文本
    public GameObject exploreUI;        // 探索界面（包含背景和内心独白）
    public TextMeshProUGUI innerMonologueText;     // 内心独白文本
    public Image backgroundImage;       // 春晓图背景图片

    [Header("角色控制")]
    public GameObject player;           // 玩家角色对象
    private PlayerController playerController; // 玩家控制器组件

    [Header("音频设置")]
    public AudioSource audioSource;     // 音频源
    public AudioClip[] narrationClips;  // 旁白音频片段（按顺序）
    public AudioClip innerMonologueClip; // 内心独白音频
    public AudioClip backgroundMusic;   // 探索背景音乐
    public float bgmFadeTime = 1.5f;    // 背景音乐淡入时间

    [Header("文字内容")]
    [TextArea(3, 5)]
    public string[] narrationLines;     // 旁白文字（按顺序）
    [TextArea(2, 3)]
    public string innerMonologueLine;   // 内心独白文字

    [Header("时间设置")]
    public float typeSpeed = 0.05f;      // 打字机效果速度
    public float linePauseTime = 1.0f;   // 行间暂停时间
    public float fadeTime = 1.0f;        // 淡入淡出时间
    public float backgroundFadeTime = 2.0f; // 背景图渐隐渐显时间

    [Header("事件")]
    public UnityEvent onOpeningComplete; // 开场结束事件

    private int currentLineIndex = 0;
    private bool isWaitingForClick = false;
    private bool isOpeningComplete = false;
    private Coroutine currentTypeCoroutine;

    void Start()
    {
       // SkipOpening();
        // 获取PlayerController组件并禁用移动
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false; // 开场时禁用移动
                Debug.Log("开场：已禁用玩家移动");
            }
        }

        // 初始化状态
        if (blackScreen != null)
            blackScreen.color = Color.black;

        if (narrationText != null)
            narrationText.text = "";

        if (clickHintText != null)
            clickHintText.gameObject.SetActive(false);

        if (exploreUI != null)
            exploreUI.SetActive(false);

        // 设置背景图初始透明度为0（完全透明）
        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            backgroundImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
            Debug.Log("开场：背景图初始透明度设置为0");
        }

        // 开始开场序列
        StartCoroutine(OpeningSequence());
    }

    void Update()
    {
        // 点击继续逻辑
        if (isWaitingForClick && Input.GetMouseButtonDown(0))
        {
            isWaitingForClick = false;

            if (clickHintText != null)
                clickHintText.gameObject.SetActive(false);

            if (currentLineIndex < narrationLines.Length)
            {
                // 还有下一句旁白
                Debug.Log($"开始播放第 {currentLineIndex + 1} 段旁白");
                StartCoroutine(PlayNextNarration());
            }
            else if (!isOpeningComplete)
            {
                // 旁白结束，切换到探索界面
                Debug.Log("旁白结束，切换到探索界面");
                StartCoroutine(TransitionToExplore());
            }
        }
    }

    IEnumerator OpeningSequence()
    {
        // 初始黑屏（2秒）
        yield return new WaitForSeconds(2f);

        // 开始播放第一句旁白
        yield return StartCoroutine(PlayNextNarration());
    }

    IEnumerator PlayNextNarration()
    {
        // 清除当前文本 - 确保每次播放新旁白前清空
        if (narrationText != null)
        {
            narrationText.text = "";
            Debug.Log("清除旁白文本");
        }

        // 检查索引是否有效
        if (currentLineIndex < narrationLines.Length)
        {
            // 开始打字效果并同步播放音频
            if (currentTypeCoroutine != null)
                StopCoroutine(currentTypeCoroutine);

            currentTypeCoroutine = StartCoroutine(TypeTextWithAudio(narrationLines[currentLineIndex], currentLineIndex));

            // 等待打字效果完成
            yield return currentTypeCoroutine;

            // 打字完成后，等待1秒再显示点击提示（可选）
            yield return new WaitForSeconds(0.5f);

            // 显示点击提示
            if (clickHintText != null)
            {
                clickHintText.gameObject.SetActive(true);
                Debug.Log("显示点击提示");
            }

            isWaitingForClick = true;
            currentLineIndex++;
        }
        else
        {
            Debug.LogWarning("没有更多的旁白文本了");
        }
    }

    IEnumerator TypeTextWithAudio(string text, int lineIndex)
    {
        // 开始播放对应的音频（在打字开始时播放）
        if (audioSource != null && narrationClips != null && lineIndex < narrationClips.Length && narrationClips[lineIndex] != null)
        {
            audioSource.clip = narrationClips[lineIndex];
            audioSource.Play();
            Debug.Log($"开始播放第 {lineIndex + 1} 段音频");
        }

        // 逐字显示文本
        if (narrationText != null)
        {
            narrationText.text = "";
            foreach (char c in text.ToCharArray())
            {
                narrationText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            Debug.Log($"第 {lineIndex + 1} 段旁白打字完成");
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator TransitionToExplore()
    {
        isOpeningComplete = true;

        // 最后清除一次旁白文本
        if (narrationText != null)
        {
            narrationText.text = "";
            Debug.Log("开场结束：清除最后一段旁白");
        }

        // 隐藏点击提示
        if (clickHintText != null)
            clickHintText.gameObject.SetActive(false);

        // 让春晓图渐显（从0到1）
        if (backgroundImage != null)
        {
            Debug.Log("开始春晓图渐显");
            yield return StartCoroutine(FadeBackground(1, 0, backgroundFadeTime));
            Debug.Log("春晓图渐显完成");
        }

        // 淡出黑屏
        if (blackScreen != null)
        {
            float elapsedTime = 0;
            Color blackColor = blackScreen.color;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeTime);
                blackScreen.color = new Color(blackColor.r, blackColor.g, blackColor.b, alpha);
                yield return null;
            }

            blackScreen.gameObject.SetActive(false);
            Debug.Log("黑屏淡出完成");
        }

        // 显示探索界面
        if (exploreUI != null)
        {
            exploreUI.SetActive(true);
            Debug.Log("显示探索界面");
        }

        // 播放背景音乐（淡入）
        if (backgroundMusic != null && audioSource != null)
        {
            StartCoroutine(FadeInBGM());
        }

        // 显示内心独白
        if (!string.IsNullOrEmpty(innerMonologueLine) && innerMonologueText != null)
        {
           // audioSource.clip = innerMonologueClip;
            yield return StartCoroutine(TypeInnerMonologue(innerMonologueLine));
        }
        else
        {
            innerMonologueText.gameObject.SetActive(false);
        }

        // 启用玩家移动
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("开场结束：启用玩家移动");
        }

        // 开场结束，触发事件
        onOpeningComplete?.Invoke();
        Debug.Log("开场序列全部完成");
    }

    IEnumerator FadeBackground(float startAlpha, float endAlpha, float duration)
    {
        if (backgroundImage == null)
        {
            Debug.LogError("backgroundImage 为空，无法执行渐隐渐显");
            yield break;
        }

        float elapsedTime = 1;
        Color bgColor = backgroundImage.color;
        Debug.Log($"开始背景图渐变：从 {startAlpha} 到 {endAlpha}，时长 {duration}秒");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            backgroundImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);

            // 每0.5秒输出一次当前透明度（可选，调试用）
            if (elapsedTime % 0.5f < Time.deltaTime)
            {
                Debug.Log($"背景图当前透明度: {alpha}");
            }

            yield return null;
        }

        backgroundImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, endAlpha);
        Debug.Log($"背景图渐变完成，最终透明度: {endAlpha}");
    }

    IEnumerator TypeInnerMonologue(string text)
    {
        if (innerMonologueText == null) yield break;

        innerMonologueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            innerMonologueText.text += c;
            
            yield return new WaitForSeconds(typeSpeed * 1.2f); // 内心独白稍慢
        }

        // 内心独白停留3秒后淡出
        yield return new WaitForSeconds(3f);

        float elapsedTime = 0;
        Color textColor = innerMonologueText.color;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime);
            innerMonologueText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        innerMonologueText.gameObject.SetActive(false);
    }

    IEnumerator FadeInBGM()
    {
        if (audioSource == null || backgroundMusic == null) yield break;

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = 0;
        audioSource.Play();

        float elapsedTime = 0;
        while (elapsedTime < bgmFadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, elapsedTime / bgmFadeTime);
            yield return null;
        }
    }

    public void SkipOpening()
    {
        // 直接跳过开场，触发结束事件
        StopAllCoroutines();
        // 确保所有UI状态正确
        if (blackScreen != null)
            blackScreen.gameObject.SetActive(false);
        if (narrationText != null)
            narrationText.text = "";
        if (clickHintText != null)
            clickHintText.gameObject.SetActive(false);
        if (exploreUI != null)
            exploreUI.SetActive(true);
        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            backgroundImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0);
        }
        if(innerMonologueText != null)
            innerMonologueText.gameObject.SetActive(false);

        if(narrationClips!= null)
            narrationClips = null; // 停止所有旁白音频
        // 启用玩家移动
        if (playerController != null)
            playerController.enabled = true;
        // 播放背景音乐（如果有）
        if (backgroundMusic != null && audioSource != null)
            audioSource.PlayOneShot(backgroundMusic);
        onOpeningComplete?.Invoke();
        Debug.Log("开场被跳过，直接进入探索界面");
    }
}
