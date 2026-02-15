using System.Collections;
using UnityEngine;

public class SpriteHitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Color flashColor = new Color(1f, 0.55f, 0.55f, 1f);
    [SerializeField] private float flashDuration = 0.1f;

    private Color originalColor = Color.white;
    private Coroutine flashRoutine;
    private bool hasCachedOriginalColor;

    private void Awake()
    {
        CacheRenderer();
        CacheOriginalColor();
    }

    private void OnEnable()
    {
        CacheRenderer();
        CacheOriginalColor();
    }

    public void TriggerFlash()
    {
        if (targetRenderer == null)
        {
            CacheRenderer();
            if (targetRenderer == null)
            {
                Debug.LogWarning("SpriteHitFlash: SpriteRenderer is missing. Flash skipped.", this);
                return;
            }
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        CacheOriginalColor();
        targetRenderer.color = flashColor;
        float duration = Mathf.Max(0.01f, flashDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            targetRenderer.color = Color.Lerp(flashColor, originalColor, t);
            yield return null;
        }

        targetRenderer.color = originalColor;
        flashRoutine = null;
    }

    private void CacheRenderer()
    {
        if (targetRenderer != null)
        {
            return;
        }

        targetRenderer = GetComponent<SpriteRenderer>();
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void CacheOriginalColor()
    {
        if (targetRenderer == null || hasCachedOriginalColor)
        {
            return;
        }

        originalColor = targetRenderer.color;
        hasCachedOriginalColor = true;
    }

    private void OnValidate()
    {
        flashDuration = Mathf.Clamp(flashDuration, 0.05f, 0.3f);
    }
}
