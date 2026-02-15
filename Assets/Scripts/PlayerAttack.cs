using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Attack")]
    [SerializeField] private float fireCooldown = 0.25f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private int projectileDamage = 1;

    private float nextFireTime;
    private Vector2 lastShootDirection = Vector2.right;

    public event Action Fired;
    public event Action<Vector2> FiredWithDirection;
    public Vector2 LastShootDirection => lastShootDirection;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            TryFire();
        }
    }

    public bool TryFire()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            return false;
        }

        if (Time.time < nextFireTime)
        {
            return false;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerAttack: Projectile Prefab is not assigned.", this);
            return false;
        }

        if (firePoint == null)
        {
            Debug.LogError("PlayerAttack: Fire Point is not assigned.", this);
            return false;
        }

        Vector2 shootDirection = ResolveShootDirection();
        if (shootDirection.sqrMagnitude <= 0.0001f)
        {
            shootDirection = ResolveFallbackDirection();
        }

        lastShootDirection = shootDirection.normalized;
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile2D projectile = projectileInstance.GetComponent<Projectile2D>();
        if (projectile == null)
        {
            Debug.LogError("PlayerAttack: Projectile prefab does not have Projectile2D component.", projectileInstance);
            Destroy(projectileInstance);
            return false;
        }

        projectile.Initialize(shootDirection, projectileSpeed, projectileDamage, projectileLifetime);
        ProjectileVisual projectileVisual = projectileInstance.GetComponent<ProjectileVisual>();
        if (projectileVisual != null)
        {
            projectileVisual.Initialize(lastShootDirection);
        }

        nextFireTime = Time.time + fireCooldown;
        Fired?.Invoke();
        FiredWithDirection?.Invoke(lastShootDirection);
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlaySfx(SfxType.Shoot);
        }
        return true;
    }

    private Vector2 ResolveShootDirection()
    {
        Vector2 fallbackDirection = ResolveFallbackDirection();
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return fallbackDirection;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        bool isMouseScreenPositionValid =
            mouseScreenPosition.x >= 0f &&
            mouseScreenPosition.y >= 0f &&
            mouseScreenPosition.x <= Screen.width &&
            mouseScreenPosition.y <= Screen.height;

        if (!isMouseScreenPositionValid)
        {
            return fallbackDirection;
        }

        float zDistanceToFirePoint = Mathf.Abs(mainCamera.transform.position.z - firePoint.position.z);
        mouseScreenPosition.z = zDistanceToFirePoint;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 directionFromMouse = (Vector2)(mouseWorldPosition - firePoint.position);

        if (directionFromMouse.sqrMagnitude > 0.0001f)
        {
            return directionFromMouse.normalized;
        }

        return fallbackDirection;
    }

    private Vector2 ResolveFallbackDirection()
    {
        if (playerController != null)
        {
            return playerController.SafeLastMoveDirection;
        }

        return Vector2.right;
    }
}
