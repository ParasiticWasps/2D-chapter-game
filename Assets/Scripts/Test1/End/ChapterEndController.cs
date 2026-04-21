using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class ChapterEndController : MonoBehaviour
{
    [Header("UI组件")]
    public Image blackScreen;           // 黑屏遮罩
    public TextMeshProUGUI endingText;             // 结尾文字

    [Header("音频设置")]
    public AudioSource audioSource;     // 音频源
    public AudioClip endingBGM;         // 结尾背景音乐
    public float bgmFadeTime = 1.5f;    // 背景音乐淡入时间

    [Header("文字内容")]
    [TextArea(3, 5)]
    public string endingLine1;          // 第一行文字
    [TextArea(3, 5)]
    public string endingLine2;          // 第二行文字

    [Header("时间设置")]
    public float fadeInTime = 1.5f;      // 渐暗时间（画面1.5秒渐暗）
    public float blackHoldTime = 0.5f;   // 完全黑屏后的停留时间
    public float typeSpeed = 0.08f;      // 打字机效果速度
    public float textStayTime = 4f;      // 文字停留时间
    public float fadeOutTime = 1f;       // 最终淡出时间（可选）

    [Header("场景切换")]
    public string nextSceneName;         // 下一个场景名称（如主菜单或下一章）
    public bool autoLoadNextScene = true;// 是否自动加载下一场景

    [Header("初始状态")]
    public bool initializeOnStart = false; // 是否在Start时初始化（默认false）

    private bool isEndingActive = false;   // 结尾是否正在播放
    private Coroutine endingCoroutine;     // 结尾协程引用

    void Start()
    {
        // 如果设置为在Start时初始化，则初始化UI状态
        if (initializeOnStart)
        {
            InitializeUI();
        }
    }

    /// <summary>
    /// 初始化UI状态（黑屏透明，文字清空）
    /// 在场景加载时调用一次
    /// </summary>
    public void InitializeUI()
    {
        Debug.Log("初始化章尾UI");

        // 初始化黑屏为透明
        if (blackScreen != null)
        {
            Color blackColor = blackScreen.color;
            blackScreen.color = new Color(blackColor.r, blackColor.g, blackColor.b, 0);
            blackScreen.gameObject.SetActive(true);
        }

        // 清空结尾文字
        if (endingText != null)
        {
            endingText.text = "";
            endingText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 开始章尾黑屏效果
    /// 在需要触发结尾的地方调用这个方法
    /// </summary>
    public void StartChapterEnd()
    {
        if (isEndingActive)
        {
            Debug.LogWarning("章尾效果已经在播放中");
            return;
        }

        Debug.Log("开始章尾黑屏效果");

        // 确保UI已经初始化
        if (blackScreen != null && blackScreen.color.a > 0)
        {
            // 如果黑屏不透明，先重置
            InitializeUI();
        }

        // 开始结尾序列
        endingCoroutine = StartCoroutine(ChapterEndSequence());
    }

    /// <summary>
    /// 停止章尾效果（如果需要提前中断）
    /// </summary>
    public void StopChapterEnd()
    {
        if (endingCoroutine != null)
        {
            StopCoroutine(endingCoroutine);
            endingCoroutine = null;
        }

        isEndingActive = false;
    }

    /// <summary>
    /// 设置自定义的结尾文字（可选）
    /// </summary>
    public void SetEndingLines(string line1, string line2)
    {
        endingLine1 = line1;
        endingLine2 = line2;
    }

    /// <summary>
    /// 设置下一场景名称（可选）
    /// </summary>
    public void SetNextScene(string sceneName)
    {
        nextSceneName = sceneName;
    }

    IEnumerator ChapterEndSequence()
    {
        isEndingActive = true;

        // 1. 画面1.5秒渐暗
        yield return StartCoroutine(FadeScreen(0, 1, fadeInTime));

        // 2. 完全黑屏后短暂停留
        yield return new WaitForSeconds(blackHoldTime);

        // 播放结尾背景音乐（淡入）
        if (endingBGM != null && audioSource != null)
        {
            StartCoroutine(FadeInBGM());
        }

        // 3. 白字逐字浮现第一句
        if (!string.IsNullOrEmpty(endingLine1))
        {
            yield return StartCoroutine(TypeText(endingLine1));
        }

        // 换行停顿
        yield return new WaitForSeconds(0.3f);

        // 4. 白字逐字浮现第二句
        if (!string.IsNullOrEmpty(endingLine2))
        {
            yield return StartCoroutine(TypeText(endingLine2));
        }

        // 5. 停留4秒
        yield return new WaitForSeconds(textStayTime);

        // 6. 自动结束
        isEndingActive = false;

        // 可选：淡出黑屏或切换到下一场景
        if (autoLoadNextScene && !string.IsNullOrEmpty(nextSceneName))
        {
            // 淡出黑屏（可选）
            yield return StartCoroutine(FadeScreen(1, 0, 1f));

            // 加载下一场景
            SceneManager.LoadSceneAsync(1);
        }
    }

    IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        if (blackScreen == null) yield break;

        float elapsedTime = 0;
        Color screenColor = blackScreen.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            blackScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, alpha);
            yield return null;
        }

        blackScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, endAlpha);
    }

    IEnumerator TypeText(string text)
    {
        if (endingText == null) yield break;

        // 如果是第一句，先清空；如果是第二句，加换行
        if (!string.IsNullOrEmpty(endingText.text))
        {
            endingText.text += "\n\n"; // 两行之间空一行
        }

        // 逐字显示
        foreach (char c in text.ToCharArray())
        {
            endingText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator FadeInBGM()
    {
        if (audioSource == null || endingBGM == null) yield break;

        audioSource.clip = endingBGM;
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

    // 公开方法：手动跳过结尾（可选）
    public void SkipEnding()
    {
        if (!isEndingActive) return;

        StopChapterEnd();

        if (autoLoadNextScene && !string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
