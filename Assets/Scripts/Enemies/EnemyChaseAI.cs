using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float targetRefreshInterval = 0.5f;

    private float nextTargetRefreshTime;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        moveSpeed = Mathf.Max(0f, moveSpeed);
        targetRefreshInterval = Mathf.Max(0.1f, targetRefreshInterval);
    }

    private void FixedUpdate()
    {
        RefreshTargetIfNeeded();

        if (rb == null || playerTarget == null)
        {
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = playerTarget.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 nextPosition = currentPosition + direction.normalized * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);
    }

    private void RefreshTargetIfNeeded()
    {
        if (playerTarget != null)
        {
            return;
        }

        if (Time.time < nextTargetRefreshTime)
        {
            return;
        }

        nextTargetRefreshTime = Time.time + targetRefreshInterval;

        GameObject playerByTag = GameObject.FindGameObjectWithTag("Player");
        if (playerByTag != null)
        {
            playerTarget = playerByTag.transform;
            return;
        }

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerTarget = playerHealth.transform;
        }
    }
}
