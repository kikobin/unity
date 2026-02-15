using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float damageCooldown = 0.5f;

    private float nextDamageTime;

    private void Awake()
    {
        damagePerHit = Mathf.Max(1, damagePerHit);
        damageCooldown = Mathf.Max(0.05f, damageCooldown);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryApplyDamage(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryApplyDamage(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryApplyDamage(other);
    }

    private void TryApplyDamage(Collider2D targetCollider)
    {
        if (targetCollider == null || Time.time < nextDamageTime)
        {
            return;
        }

        PlayerHealth playerHealth = targetCollider.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = targetCollider.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(damagePerHit);
        nextDamageTime = Time.time + damageCooldown;
    }

    private void OnValidate()
    {
        damagePerHit = Mathf.Max(1, damagePerHit);
        damageCooldown = Mathf.Max(0.05f, damageCooldown);
    }
}
