using UnityEngine;
using System.Collections;

public class GhostEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    public float fadeSpeed = 3f;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void OnEnable()
    {
        sr.color = originalColor; 
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float alpha = sr.color.a;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}