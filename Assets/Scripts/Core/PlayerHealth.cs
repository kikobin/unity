using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;
    [SerializeField] private SpriteHitFlash hitFlash;

    private bool isDead;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsDead => isDead;

    public event Action<int, int> HealthChanged;

    private void Awake()
    {
        maxHp = Mathf.Max(1, maxHp);
        currentHp = currentHp <= 0 ? maxHp : Mathf.Clamp(currentHp, 0, maxHp);
        isDead = false;

        if (hitFlash == null)
        {
            hitFlash = GetComponent<SpriteHitFlash>();
        }
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || isDead)
        {
            return;
        }

        int newHp = Mathf.Clamp(currentHp - amount, 0, maxHp);
        if (newHp == currentHp)
        {
            return;
        }

        currentHp = newHp;
        if (hitFlash != null)
        {
            hitFlash.TriggerFlash();
        }

        NotifyHealthChanged();

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

        GameRoot gameRoot = FindObjectOfType<GameRoot>();
        if (gameRoot == null)
        {
            Debug.LogWarning("PlayerHealth.Die: GameRoot not found, cannot call EndGame(false).");
            return;
        }

        gameRoot.EndGame(false);
    }

    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(currentHp, maxHp);
    }

    public void NotifyCurrentHealth()
    {
        NotifyHealthChanged();
    }

    private void OnValidate()
    {
        maxHp = Mathf.Max(1, maxHp);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }
}
