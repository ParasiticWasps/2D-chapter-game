using UnityEngine;

public class ObstacleFade : MonoBehaviour
{
    private SpriteRenderer sr;

    public float fadeAlpha = 0.4f;
    public float fadeSpeed = 5f;

    private float targetAlpha = 1f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        sr.color = c;
    }

    public void SetFade(bool fade)
    {
        targetAlpha = fade ? fadeAlpha : 1f;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        ObstacleFade fade = other.GetComponent<ObstacleFade>();
        if (fade != null)
            fade.SetFade(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ObstacleFade fade = other.GetComponent<ObstacleFade>();
        if (fade != null)
            fade.SetFade(false);
    }
}