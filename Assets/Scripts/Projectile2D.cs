using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.right;
    private bool isInitialized;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (!isInitialized)
        {
            Initialize(transform.right, speed, damage, lifetime);
        }
    }

    private void Update()
    {
        if (rb == null)
        {
            transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        }
    }

    public void Initialize(Vector2 direction, float newSpeed, int newDamage, float newLifetime)
    {
        moveDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        speed = Mathf.Max(0f, newSpeed);
        damage = Mathf.Max(0, newDamage);
        lifetime = Mathf.Max(0.05f, newLifetime);
        isInitialized = true;

        transform.right = moveDirection;

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth == null)
        {
            return;
        }

        EnemyKnockback enemyKnockback = other.GetComponent<EnemyKnockback>();
        if (enemyKnockback == null)
        {
            enemyKnockback = other.GetComponentInParent<EnemyKnockback>();
        }

        hasHit = true;
        enemyHealth.TakeDamage(damage);

        if (enemyKnockback != null)
        {
            enemyKnockback.ApplyKnockback(transform.position, enemyKnockback.GetKnockbackForce());
        }

        Destroy(gameObject);
    }
}
