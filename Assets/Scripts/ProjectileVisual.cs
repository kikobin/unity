using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    [Header("Scale Pulse")]
    [SerializeField] private float scalePulseAmount = 0.07f;
    [SerializeField] private float scalePulseSpeed = 10f;

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 60f;
    [SerializeField] private bool spinWithTravelDirection = true;

    private Vector3 baseScale = Vector3.one;
    private float spinDirection = 1f;
    private bool initialized;

    private void Awake()
    {
        baseScale = transform.localScale;
        scalePulseAmount = Mathf.Clamp(scalePulseAmount, 0f, 0.1f);
        scalePulseSpeed = Mathf.Clamp(scalePulseSpeed, 0f, 20f);
        spinSpeed = Mathf.Clamp(spinSpeed, 0f, 120f);
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * scalePulseSpeed) * scalePulseAmount;
        transform.localScale = baseScale * pulse;

        float deltaSpin = spinSpeed * spinDirection * Time.deltaTime;
        transform.Rotate(0f, 0f, deltaSpin);
    }

    public void Initialize(Vector2 moveDirection)
    {
        initialized = true;
        if (!spinWithTravelDirection)
        {
            spinDirection = 1f;
            return;
        }

        if (moveDirection.sqrMagnitude <= 0.0001f)
        {
            spinDirection = 1f;
            return;
        }

        spinDirection = moveDirection.x >= 0f ? 1f : -1f;
    }

    public void SetBaseScale(Vector3 newBaseScale)
    {
        if (newBaseScale.x <= 0f || newBaseScale.y <= 0f)
        {
            return;
        }

        baseScale = newBaseScale;
        transform.localScale = baseScale;
    }

    private void Start()
    {
        if (!initialized)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f)
            {
                Initialize(rb.linearVelocity.normalized);
                return;
            }

            Projectile2D projectile = GetComponent<Projectile2D>();
            if (projectile == null)
            {
                Debug.LogWarning("ProjectileVisual: Projectile2D is missing. Using default spin direction.", this);
            }

            Initialize(Vector2.right);
        }
    }
}
