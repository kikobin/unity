using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Rigidbody2D playerRb;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private bool warnedAboutMissingAnimator;
    private bool warnedAboutMissingController;

    private void Awake()
    {
        CacheDependencies();

        WarnIfAnimatorMissing();
        WarnIfControllerMissing();
    }

    private void OnEnable()
    {
        CacheDependencies();

        if (playerAttack != null)
        {
            playerAttack.Fired += OnPlayerFired;
        }
    }

    private void OnDisable()
    {
        if (playerAttack != null)
        {
            playerAttack.Fired -= OnPlayerFired;
        }
    }

    private void Update()
    {
        if (playerAnimator == null || playerController == null || playerHealth == null)
        {
            CacheDependencies();
        }

        if (!HasAnimator())
        {
            return;
        }

        float speed = ResolveSpeed();
        playerAnimator.SetFloat(SpeedHash, speed);

        bool isDead = playerHealth != null && playerHealth.IsDead;
        playerAnimator.SetBool(IsDeadHash, isDead);
    }

    private void OnPlayerFired()
    {
        if (!HasAnimator())
        {
            return;
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            return;
        }

        playerAnimator.SetTrigger(AttackHash);
    }

    private bool HasAnimator()
    {
        if (playerAnimator != null)
        {
            return true;
        }

        WarnIfAnimatorMissing();
        return false;
    }

    private void WarnIfAnimatorMissing()
    {
        if (warnedAboutMissingAnimator || playerAnimator != null)
        {
            return;
        }

        warnedAboutMissingAnimator = true;
        Debug.LogWarning("PlayerAnimatorDriver: Animator is missing. Animation parameters will not be updated.", this);
    }

    private void WarnIfControllerMissing()
    {
        if (warnedAboutMissingController || playerController != null)
        {
            return;
        }

        warnedAboutMissingController = true;
        Debug.LogWarning("PlayerAnimatorDriver: PlayerController is missing. Run animation may not update.", this);
    }

    private void CacheDependencies()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                playerAnimator = GetComponentInChildren<Animator>();
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

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = GetComponentInParent<PlayerHealth>();
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

        if (playerRb == null)
        {
            playerRb = GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                playerRb = GetComponentInParent<Rigidbody2D>();
            }
        }
    }

    private float ResolveSpeed()
    {
        if (playerController != null)
        {
            return Mathf.Max(0f, playerController.CurrentSpeed);
        }

        if (playerRb != null)
        {
            return Mathf.Max(0f, playerRb.linearVelocity.magnitude);
        }

        return 0f;
    }
}
