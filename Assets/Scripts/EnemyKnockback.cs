using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float controlLockDuration = 0.12f;

    private Rigidbody2D rb;
    private Behaviour chaseController;
    private Coroutine controlLockRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"[{nameof(EnemyKnockback)}] Rigidbody2D is missing on {name}. Knockback will be skipped.", this);
        }

        ResolveChaseController();
    }

    private void FixedUpdate()
    {
        ClampVelocity();
    }

    public void ApplyKnockback(Vector2 sourcePos, float force)
    {
        if (rb == null)
        {
            Debug.LogError($"[{nameof(EnemyKnockback)}] Cannot apply knockback on {name}: Rigidbody2D is missing.", this);
            return;
        }

        Vector2 direction = (rb.position - sourcePos).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right;
        }

        float safeForce = Mathf.Max(0f, force);
        rb.AddForce(direction * safeForce, ForceMode2D.Impulse);

        StartControlLock();
        ClampVelocity();
    }

    public float GetKnockbackForce()
    {
        return Mathf.Max(0f, knockbackForce);
    }

    private void ClampVelocity()
    {
        if (rb == null)
        {
            return;
        }

        float safeMaxSpeed = Mathf.Max(0f, maxSpeed);
        if (safeMaxSpeed <= 0f)
        {
            return;
        }

        Vector2 velocity = rb.linearVelocity;
        if (velocity.sqrMagnitude > safeMaxSpeed * safeMaxSpeed)
        {
            rb.linearVelocity = velocity.normalized * safeMaxSpeed;
        }
    }

    private void StartControlLock()
    {
        ResolveChaseController();

        if (chaseController == null || !chaseController.enabled)
        {
            return;
        }

        float safeDuration = Mathf.Max(0f, controlLockDuration);
        if (safeDuration <= 0f)
        {
            return;
        }

        if (controlLockRoutine != null)
        {
            StopCoroutine(controlLockRoutine);
        }

        controlLockRoutine = StartCoroutine(ControlLockCoroutine(safeDuration));
    }

    private System.Collections.IEnumerator ControlLockCoroutine(float duration)
    {
        chaseController.enabled = false;
        yield return new WaitForSeconds(duration);
        if (chaseController != null)
        {
            chaseController.enabled = true;
        }

        controlLockRoutine = null;
    }

    private void ResolveChaseController()
    {
        if (chaseController != null)
        {
            return;
        }

        EnemyChaseAI localChase = GetComponent<EnemyChaseAI>();
        if (localChase != null)
        {
            chaseController = localChase;
            return;
        }

        localChase = GetComponentInParent<EnemyChaseAI>();
        if (localChase != null)
        {
            chaseController = localChase;
            return;
        }

        localChase = GetComponentInChildren<EnemyChaseAI>();
        if (localChase != null)
        {
            chaseController = localChase;
        }
    }
}
