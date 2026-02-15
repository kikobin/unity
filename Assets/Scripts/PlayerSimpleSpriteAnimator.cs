using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSimpleSpriteAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Animator legacyAnimator;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite walkFrameA;
    [SerializeField] private Sprite walkFrameB;
    [SerializeField] private Sprite attackSprite;

    [Header("Timing")]
    [SerializeField, Min(0.01f)] private float walkFrameDuration = 0.14f;
    [SerializeField, Min(0.01f)] private float attackDuration = 0.12f;
    [SerializeField, Min(0f)] private float moveThreshold = 0.05f;

    [Header("Behavior")]
    [SerializeField] private bool disableLegacyAnimator = true;

    private float walkTimer;
    private bool useFirstWalkFrame = true;
    private float attackUntilTime;
    private bool warnedMissingRenderer;

    private void Awake()
    {
        CacheReferences();
        if (idleSprite == null && targetRenderer != null)
        {
            idleSprite = targetRenderer.sprite;
        }
    }

    private void OnEnable()
    {
        CacheReferences();

        if (disableLegacyAnimator && legacyAnimator != null)
        {
            legacyAnimator.enabled = false;
        }

        if (playerAttack != null)
        {
            playerAttack.Fired += OnFired;
        }
    }

    private void OnDisable()
    {
        if (playerAttack != null)
        {
            playerAttack.Fired -= OnFired;
        }
    }

    private void Update()
    {
        if (targetRenderer == null)
        {
            CacheReferences();
            if (targetRenderer == null)
            {
                if (!warnedMissingRenderer)
                {
                    warnedMissingRenderer = true;
                    Debug.LogWarning("PlayerSimpleSpriteAnimator: SpriteRenderer is missing.", this);
                }

                return;
            }
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            ApplyIdleSprite();
            return;
        }

        if (Time.time < attackUntilTime && attackSprite != null)
        {
            targetRenderer.sprite = attackSprite;
            return;
        }

        float speed = playerController != null ? playerController.CurrentSpeed : 0f;
        if (speed >= moveThreshold)
        {
            AnimateWalk();
        }
        else
        {
            walkTimer = 0f;
            useFirstWalkFrame = true;
            ApplyIdleSprite();
        }
    }

    private void AnimateWalk()
    {
        Sprite first = walkFrameA != null ? walkFrameA : idleSprite;
        Sprite second = walkFrameB != null ? walkFrameB : first;

        if (first == null && second == null)
        {
            return;
        }

        walkTimer += Time.deltaTime;
        if (walkTimer >= walkFrameDuration)
        {
            walkTimer = 0f;
            useFirstWalkFrame = !useFirstWalkFrame;
        }

        targetRenderer.sprite = useFirstWalkFrame ? first : second;
    }

    private void ApplyIdleSprite()
    {
        if (idleSprite != null)
        {
            targetRenderer.sprite = idleSprite;
            return;
        }

        if (walkFrameA != null)
        {
            targetRenderer.sprite = walkFrameA;
            return;
        }

        if (walkFrameB != null)
        {
            targetRenderer.sprite = walkFrameB;
        }
    }

    private void OnFired()
    {
        if (attackSprite == null)
        {
            return;
        }

        attackUntilTime = Time.time + attackDuration;
    }

    private void CacheReferences()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = GetComponentInParent<PlayerController>();
            }
        }

        if (playerAttack == null)
        {
            playerAttack = GetComponent<PlayerAttack>();
            if (playerAttack == null)
            {
                playerAttack = GetComponentInParent<PlayerAttack>();
            }
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = GetComponentInParent<PlayerHealth>();
            }
        }

        if (legacyAnimator == null)
        {
            legacyAnimator = GetComponent<Animator>();
            if (legacyAnimator == null)
            {
                legacyAnimator = GetComponentInChildren<Animator>();
            }
        }
    }
}
