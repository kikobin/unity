using UnityEngine;
using System;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    [SerializeField] private int currentHp;
    [SerializeField] private int scoreOnDeath = 10;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteHitFlash hitFlash;
    [SerializeField] private float deathFadeDuration = 0.3f;

    private bool isDead;
    private Coroutine deathRoutine;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsDead => isDead;
    public event Action<EnemyHealth> Died;

    private void Awake()
    {
        maxHp = Mathf.Max(1, maxHp);
        currentHp = maxHp;
        scoreOnDeath = Mathf.Max(0, scoreOnDeath);
        deathFadeDuration = Mathf.Clamp(deathFadeDuration, 0.2f, 0.4f);
        isDead = false;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (hitFlash == null)
        {
            hitFlash = GetComponent<SpriteHitFlash>();
            if (hitFlash == null)
            {
                hitFlash = GetComponentInChildren<SpriteHitFlash>();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHp = Mathf.Clamp(currentHp - amount, 0, maxHp);
        if (hitFlash != null)
        {
            hitFlash.TriggerFlash();
        }

        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlaySfx(SfxType.Hit);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlaySfx(SfxType.Die);
        }

        Died?.Invoke(this);

        GameRoot gameRoot = GameRoot.Instance;
        if (gameRoot == null)
        {
            gameRoot = FindObjectOfType<GameRoot>();
        }

        if (gameRoot != null)
        {
            gameRoot.AddScore(scoreOnDeath);
        }
        else
        {
            Debug.LogWarning("EnemyHealth.Die: GameRoot not found, score was not added.");
        }

        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
        }

        deathRoutine = StartCoroutine(DeathFadeAndDestroy());
    }

    private IEnumerator DeathFadeAndDestroy()
    {
        DisableCombatInteractions();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("EnemyHealth: SpriteRenderer missing, enemy will be destroyed without fade.", this);
            Destroy(gameObject);
            yield break;
        }

        Color startColor = spriteRenderer.color;
        float duration = Mathf.Max(0.01f, deathFadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(startColor.a, 0f, t);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(gameObject);
    }

    private void DisableCombatInteractions()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.simulated = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = false;
            }
        }

        EnemyChaseAI chaseAi = GetComponent<EnemyChaseAI>();
        if (chaseAi != null)
        {
            chaseAi.enabled = false;
        }

        EnemyContactDamage contactDamage = GetComponent<EnemyContactDamage>();
        if (contactDamage != null)
        {
            contactDamage.enabled = false;
        }
    }

    private void OnValidate()
    {
        maxHp = Mathf.Max(1, maxHp);
        scoreOnDeath = Mathf.Max(0, scoreOnDeath);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        deathFadeDuration = Mathf.Clamp(deathFadeDuration, 0.2f, 0.4f);
    }
}
